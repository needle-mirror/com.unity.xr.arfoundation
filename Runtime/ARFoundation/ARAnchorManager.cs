using System;
using System.Collections.Generic;
using System.Threading;
using Unity.Collections;
using UnityEngine.Serialization;
using UnityEngine.XR.ARSubsystems;
using Unity.XR.CoreUtils;
using SerializableGuid = UnityEngine.XR.ARSubsystems.SerializableGuid;

namespace UnityEngine.XR.ARFoundation
{
    /// <summary>
    /// A [trackable manager](xref:arfoundation-managers#trackables-and-trackable-managers) that enables you to add and
    /// track anchors. Add this component to your XR Origin GameObject to enable anchor tracking in your app.
    /// </summary>
    /// <remarks>
    /// An anchor is a pose (position and rotation) in the physical environment that is tracked by an XR device.
    /// Anchors are updated as the device refines its understanding of the environment, allowing you to reliably place
    /// mixed reality content at a physical pose.
    ///
    /// Related information: <a href="xref:arfoundation-anchors">Anchors</a>
    /// </remarks>
    [DefaultExecutionOrder(ARUpdateOrder.k_AnchorManager)]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(XROrigin))]
    [HelpURL(typeof(ARAnchorManager))]
    public sealed class ARAnchorManager : ARTrackableManager<
        XRAnchorSubsystem,
        XRAnchorSubsystemDescriptor,
        XRAnchorSubsystem.Provider,
        XRAnchor,
        ARAnchor>
    {
        UnityEngine.Pool.ObjectPool<AwaitableCompletionSource<Result<ARAnchor>>> m_AnchorCompletionSources = new(
            createFunc: () => new AwaitableCompletionSource<Result<ARAnchor>>(),
            actionOnGet: null,
            actionOnRelease: null,
            actionOnDestroy: null,
            collectionCheck: false,
            defaultCapacity: 8,
            maxSize: 1024);

        [SerializeField]
        [Tooltip("If not null, this prefab is instantiated for each detected 3D bounding box.")]
        [FormerlySerializedAs("m_ReferencePointPrefab")]
        GameObject m_AnchorPrefab;

        /// <summary>
        /// This prefab will be instantiated for each <see cref="ARAnchor"/>. May be <see langword="null"/>.
        /// </summary>
        /// <remarks>
        /// The purpose of this property is to extend the functionality of <see cref="ARAnchor"/>s.
        /// It is not the recommended way to instantiate content associated with an <see cref="ARAnchor"/>.
        /// </remarks>
        public GameObject anchorPrefab
        {
            get => m_AnchorPrefab;
            set => m_AnchorPrefab = value;
        }

        /// <summary>
        /// Invoked once per frame to communicate changes: new anchors, updates to existing
        /// anchors, and removed anchors.
        /// </summary>
        [Obsolete("anchorsChanged has been deprecated in AR Foundation version 6.0. Use trackablesChanged instead.", false)]
        public event Action<ARAnchorsChangedEventArgs> anchorsChanged;

        internal bool TryAddAnchor(ARAnchor anchor)
        {
            if (!CanBeAddedToSubsystem(anchor))
                return false;

            var t = anchor.transform;
            var sessionRelativePose = origin.TrackablesParent.InverseTransformPose(new Pose(t.position, t.rotation));

            // Add the anchor to the XRAnchorSubsystem
            if (subsystem.TryAddAnchor(sessionRelativePose, out var sessionRelativeData))
            {
                CreateTrackableFromExisting(anchor, sessionRelativeData);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Attempts to create a new anchor at the given <paramref name="pose"/>.
        /// </summary>
        /// <example>
        /// <para>Use this API with C# async/await syntax as shown below:</para>
        /// <code>
        ///     var result = await TryAddAnchorAsync(pose);
        ///     if (result.status.IsSuccess())
        ///         DoSomethingWith(result.value);
        /// </code>
        /// </example>
        /// <param name="pose">The pose, in Unity world space, of the anchor.</param>
        /// <returns>The result of the async operation.</returns>
        public async Awaitable<Result<ARAnchor>> TryAddAnchorAsync(Pose pose)
        {
            var completionSource = m_AnchorCompletionSources.Get();
            var sessionRelativePose = origin.TrackablesParent.InverseTransformPose(pose);
            var subsystemResult = await subsystem.TryAddAnchorAsync(sessionRelativePose);

            completionSource.SetResult(new Result<ARAnchor>(
                subsystemResult.status, CreateTrackableImmediate(subsystemResult.value)));

            var resultAwaitable = completionSource.Awaitable;

            completionSource.Reset();
            m_AnchorCompletionSources.Release(completionSource);
            return await resultAwaitable;
        }

        /// <summary>
        /// Attempts to create a new anchor that is attached to an existing <see cref="ARPlane"/>.
        /// </summary>
        /// <param name="plane">The <see cref="ARPlane"/> to which to attach.</param>
        /// <param name="pose">The initial pose, in Unity world space, of the anchor.</param>
        /// <returns>A new <see cref="ARAnchor"/> if successful, otherwise <see langword="null"/>.</returns>
        public ARAnchor AttachAnchor(ARPlane plane, Pose pose)
        {
            if (!enabled)
                throw new InvalidOperationException("Cannot create an anchor from a disabled anchor manager.");

            if (subsystem == null)
                throw new InvalidOperationException("Anchor manager has no subsystem. Enable the manager first.");

            if (plane == null)
                throw new ArgumentNullException(nameof(plane));

            var sessionRelativePose = origin.TrackablesParent.InverseTransformPose(pose);
            if (subsystem.TryAttachAnchor(plane.trackableId, sessionRelativePose, out var sessionRelativeData))
            {
                return CreateTrackableImmediate(sessionRelativeData);
            }

            return null;
        }

        /// <summary>
        /// Attempts to remove an anchor.
        /// </summary>
        /// <param name="anchor">The <see cref="ARAnchor"/> to remove.</param>
        /// <returns><see langword="True"/> if successful, otherwise <see langword="False"/></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="anchor"/> is `null`</exception>
        public bool TryRemoveAnchor(ARAnchor anchor)
        {
            if (!enabled)
                throw new InvalidOperationException("Cannot remove an anchor from a disabled anchor manager.");

            if (anchor == null)
                throw new ArgumentNullException(nameof(anchor));

            if (subsystem == null)
                return false;

            if (subsystem.TryRemoveAnchor(anchor.trackableId))
            {
                anchor.pending = false;
                DestroyPendingTrackable(anchor.trackableId);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the <see cref="ARAnchor"/> with given <paramref name="trackableId"/>, or <see langword="null"/> if
        /// no such anchor exists.
        /// </summary>
        /// <param name="trackableId">The <see cref="TrackableId"/> of the <see cref="ARAnchor"/> to retrieve.</param>
        /// <returns>The <see cref="ARAnchor"/> or <see langword="null"/>.</returns>
        public ARAnchor GetAnchor(TrackableId trackableId)
        {
            if (m_Trackables.TryGetValue(trackableId, out var anchor))
                return anchor;

            return null;
        }

        /// <summary>
        /// Get the prefab to instantiate for each <see cref="ARAnchor"/>.
        /// </summary>
        /// <returns>The prefab to instantiate for each <see cref="ARAnchor"/>.</returns>
        protected override GameObject GetPrefab() => m_AnchorPrefab;

        /// <summary>
        /// The name to assign to the GameObject instantiated for each <see cref="ARAnchor"/>.
        /// </summary>
        protected override string gameObjectName => "Anchor";

        /// <summary>
        /// Attempts to persistently save the given anchor so that it can be loaded in a future AR session. Use the
        /// `SerializableGuid` returned by this method as an input parameter to <see cref="TryLoadAnchorAsync"/> or
        /// <see cref="TryEraseAnchorAsync"/>.
        /// </summary>
        /// <param name="anchor">The anchor to save. You can create an anchor using <see cref="TryAddAnchorAsync"/>.</param>
        /// <param name="cancellationToken">An optional `CancellationToken` that you can use to cancel the operation
        /// in progress if the loaded provider <see cref="XRAnchorSubsystemDescriptor.supportsAsyncCancellation"/>.</param>
        /// <returns>The result of the async operation, containing a new persistent anchor GUID if the operation
        /// succeeded. You are responsible to <see langword="await"/> this result.</returns>
        /// <seealso cref="XRAnchorSubsystemDescriptor.supportsSaveAnchor"/>
        public Awaitable<Result<SerializableGuid>> TrySaveAnchorAsync(
            ARAnchor anchor, CancellationToken cancellationToken = default)
        {
            if (anchor == null)
                throw new ArgumentNullException(nameof(anchor));

            return subsystem.TrySaveAnchorAsync(anchor.trackableId, cancellationToken);
        }

        /// <summary>
        /// Attempts to load an anchor given its persistent anchor GUID.
        /// </summary>
        /// <param name="savedAnchorGuid">A persistent anchor GUID created by <see cref="TrySaveAnchorAsync"/>.</param>
        /// <param name="cancellationToken">An optional `CancellationToken` that you can use to cancel the operation
        /// in progress if the loaded provider <see cref="XRAnchorSubsystemDescriptor.supportsAsyncCancellation"/>.</param>
        /// <returns>The result of the async operation, containing the newly added anchor if the operation succeeded.
        /// You are responsible to <see langword="await"/> this result.</returns>
        /// <seealso cref="XRAnchorSubsystemDescriptor.supportsLoadAnchor"/>
        public async Awaitable<Result<ARAnchor>> TryLoadAnchorAsync(
            SerializableGuid savedAnchorGuid, CancellationToken cancellationToken = default)
        {
            var completionSource = m_AnchorCompletionSources.Get();
            var subsystemResult = await subsystem.TryLoadAnchorAsync(savedAnchorGuid, cancellationToken);

            completionSource.SetResult(new Result<ARAnchor>(
                subsystemResult.status,
                subsystemResult.status.IsSuccess() ? CreateTrackableImmediate(subsystemResult.value) : null));

            var resultAwaitable = completionSource.Awaitable;

            completionSource.Reset();
            m_AnchorCompletionSources.Release(completionSource);
            return await resultAwaitable;
        }

        /// <summary>
        /// Attempts to erase the persistent saved data associated with an anchor given its persistent anchor GUID.
        /// </summary>
        /// <param name="savedAnchorGuid">A persistent anchor GUID created by <see cref="TrySaveAnchorAsync"/>.</param>
        /// <param name="cancellationToken">An optional `CancellationToken` that you can use to cancel the operation
        /// in progress if the loaded provider <see cref="XRAnchorSubsystemDescriptor.supportsAsyncCancellation"/>.</param>
        /// <returns>The result of the async operation. You are responsible to <see langword="await"/> this result.</returns>
        /// <seealso cref="XRAnchorSubsystemDescriptor.supportsEraseAnchor"/>
        public Awaitable<XRResultStatus> TryEraseAnchorAsync(
            SerializableGuid savedAnchorGuid, CancellationToken cancellationToken = default)
        {
            return subsystem.TryEraseAnchorAsync(savedAnchorGuid, cancellationToken);
        }

        /// <summary>
        /// Attempts to get a `NativeArray` containing all saved persistent anchor GUIDs.
        /// </summary>
        /// <param name="allocator">The allocation strategy to use for the resulting `NativeArray`.</param>
        /// <param name="cancellationToken">An optional `CancellationToken` that you can use to cancel the operation
        /// in progress if the loaded provider <see cref="XRAnchorSubsystemDescriptor.supportsAsyncCancellation"/>.</param>
        /// <returns>The result of the async operation, containing a `NativeArray` of persistent anchor GUIDs
        /// allocated with the given <paramref name="allocator"/> if the operation succeeded.
        /// You are responsible to <see langword="await"/> this result.</returns>
        /// <seealso cref="XRAnchorSubsystemDescriptor.supportsGetSavedAnchorIds"/>
        public Awaitable<Result<NativeArray<SerializableGuid>>> TryGetSavedAnchorIdsAsync(
            Allocator allocator, CancellationToken cancellationToken = default)
        {
            return subsystem.TryGetSavedAnchorIdsAsync(allocator, cancellationToken);
        }

        /// <summary>
        /// Invoked when the base class detects trackable changes.
        /// </summary>
        /// <param name="added">The list of added anchors.</param>
        /// <param name="updated">The list of updated anchors.</param>
        /// <param name="removed">The list of removed anchors.</param>
        [Obsolete("OnTrackablesChanged() has been deprecated in AR Foundation version 6.0.", false)]
        protected override void OnTrackablesChanged(
            List<ARAnchor> added,
            List<ARAnchor> updated,
            List<ARAnchor> removed)
        {
            if (anchorsChanged != null)
            {
                using (new ScopedProfiler("OnAnchorsChanged"))
                {
                    anchorsChanged?.Invoke(new ARAnchorsChangedEventArgs(
                        added,
                        updated,
                        removed));
                }
            }
        }
    }
}
