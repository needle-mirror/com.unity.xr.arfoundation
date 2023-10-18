using System;
using System.Collections.Generic;
using UnityEngine.Serialization;
using UnityEngine.XR.ARSubsystems;
using Unity.XR.CoreUtils;

namespace UnityEngine.XR.ARFoundation
{
    /// <summary>
    /// A [trackable manager](xref:arfoundation-managers#trackables-and-trackable-managers) that enables you to add and
    /// track anchors. Add this component to your XR Origin GameObject to enable anchor tracking in your app.
    /// </summary>
    /// <remarks>
    /// An anchor is a pose (position and rotation) in the physical environment that is tracked by an XR device.
    /// Anchors are updated as the device refines its understanding of the environment, allowing you to reliably place
    /// virtual content at a physical pose.
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
        UnityEngine.Pool.ObjectPool<AwaitableCompletionSource<Result<ARAnchor>>> m_AsyncCompletionSources = new(
            createFunc: () => new AwaitableCompletionSource<Result<ARAnchor>>(),
            actionOnGet: null,
            actionOnRelease: null,
            actionOnDestroy: null,
            collectionCheck: false,
            defaultCapacity: 8,
            maxSize: 1024);

        [SerializeField]
        [Tooltip("If not null, instantiates this prefab for each instantiated anchor.")]
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
        /// Use this API with C# async/await syntax as shown below:
        /// <code>
        ///     var result = await TryAddAnchorAsync(pose);
        ///     if (result.TryGetResult(out var anchor))
        ///         DoSomethingWith(anchor);
        /// </code>
        /// </example>
        /// <param name="pose">The pose, in Unity world space, of the anchor.</param>
        /// <returns>The result of the async operation.</returns>
        public async Awaitable<Result<ARAnchor>> TryAddAnchorAsync(Pose pose)
        {
            var completionSource = m_AsyncCompletionSources.Get();
            var sessionRelativePose = origin.TrackablesParent.InverseTransformPose(pose);
            var subsystemResult = await subsystem.TryAddAnchorAsync(sessionRelativePose);

            var wasSuccessful = subsystemResult.TryGetResult(out var sessionRelativeData);
            completionSource.SetResult(new Result<ARAnchor>(wasSuccessful, CreateTrackableImmediate(sessionRelativeData)));
            var resultAwaitable = completionSource.Awaitable;

            completionSource.Reset();
            m_AsyncCompletionSources.Release(completionSource);
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

        internal bool TryRemoveAnchor(ARAnchor anchor)
        {
            if (anchor == null)
                throw new ArgumentNullException(nameof(anchor));

            if (subsystem == null)
                return false;

            if (subsystem.TryRemoveAnchor(anchor.trackableId))
            {
                if (m_PendingAdds.ContainsKey(anchor.trackableId))
                {
                    m_PendingAdds.Remove(anchor.trackableId);
                    m_Trackables.Remove(anchor.trackableId);
                }

                anchor.pending = false;
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
