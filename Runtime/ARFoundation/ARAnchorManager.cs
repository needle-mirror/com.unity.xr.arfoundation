using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.Collections;
using UnityEngine.Serialization;
using UnityEngine.XR.ARSubsystems;
using Unity.XR.CoreUtils;
using Unity.XR.CoreUtils.Collections;
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
    [AddComponentMenu("XR/AR Foundation/AR Anchor Manager")]
    [HelpURL("features/anchors/aranchormanager")]
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

        static readonly Pool.ObjectPool<Dictionary<TrackableId, ARAnchor>> s_AnchorByTrackableIdMaps =
            new(
                createFunc: () => new Dictionary<TrackableId, ARAnchor>(),
                actionOnGet: null,
                actionOnRelease: null,
                actionOnDestroy: null,
                collectionCheck: false,
                defaultCapacity: 8,
                maxSize: 1024);

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
        /// `SerializableGuid` returned by this method as an input to <see cref="TryLoadAnchorAsync"/> or
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
        /// Attempts to persistently save the given anchors so that they can be loaded in a future AR session. Use the
        /// <paramref name="outputSaveAnchorResults"/> from this method as an input to <see cref="TryLoadAnchorsAsync"/> or
        /// <see cref="TryEraseAnchorsAsync"/>.
        /// </summary>
        /// <param name="anchorsToSave">The anchors to save. You can create an anchor using <see cref="TryAddAnchorAsync"/>.</param>
        /// <param name="outputSaveAnchorResults">The list that will be cleared and then populated with results.</param>
        /// <param name="cancellationToken">An optional `CancellationToken` that you can use to cancel the operation
        /// in progress if the loaded provider <see cref="XRAnchorSubsystemDescriptor.supportsAsyncCancellation"/>.</param>
        /// <returns>The result of the async operation. You are responsible to <see langword="await"/> this result.</returns>
        /// <seealso cref="XRAnchorSubsystemDescriptor.supportsSaveAnchor"/>
        /// <exception cref="NotSupportedException"> Thrown if
        /// <see cref="XRAnchorSubsystemDescriptor.supportsSaveAnchor"/> is false for this provider.</exception>
        /// <exception cref="ArgumentNullException">Thrown if either <paramref name="anchorsToSave"/> or <paramref name="outputSaveAnchorResults"/> is null.</exception>
        public async Awaitable TrySaveAnchorsAsync(
            IEnumerable<ARAnchor> anchorsToSave,
            List<ARSaveOrLoadAnchorResult> outputSaveAnchorResults,
            CancellationToken cancellationToken = default)
        {
            if (anchorsToSave == null)
                throw new ArgumentNullException(nameof(anchorsToSave));

            if (outputSaveAnchorResults == null)
                throw new ArgumentNullException(nameof(outputSaveAnchorResults));

            outputSaveAnchorResults.Clear();
            if (!anchorsToSave.Any())
                return;

            var anchorIds = new NativeArray<TrackableId>(anchorsToSave.Count(), Allocator.Temp);
            var anchorsByTrackableId = s_AnchorByTrackableIdMaps.Get();
            var index = 0;
            foreach (var anchor in anchorsToSave)
            {
                anchorIds[index] = anchor.trackableId;
                anchorsByTrackableId.Add(anchor.trackableId, anchor);
                index += 1;
            }

            var xrSaveAnchorResults = await subsystem.TrySaveAnchorsAsync(
                anchorIds,
                Allocator.Temp,
                cancellationToken);

            for (var i = 0; i < xrSaveAnchorResults.Length; i += 1)
            {
                var saveAnchorResult = new ARSaveOrLoadAnchorResult
                {
                    resultStatus =  xrSaveAnchorResults[i].resultStatus,
                    anchor = anchorsByTrackableId[xrSaveAnchorResults[i].trackableId],
                    savedAnchorGuid = xrSaveAnchorResults[i].savedAnchorGuid,
                };

                outputSaveAnchorResults.Add(saveAnchorResult);
            }

            anchorsByTrackableId.Clear();
            s_AnchorByTrackableIdMaps.Release(anchorsByTrackableId);
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
        /// Attempts to load a batch of anchors given their persistent anchor GUIDs.
        /// </summary>
        /// <param name="savedAnchorGuidsToLoad">The persistent anchor GUIDs created by <see cref="TrySaveAnchorsAsync"/> or
        /// <see cref="TrySaveAnchorAsync"/>.</param>
        /// <param name="outputLoadAnchorResults">The list that will be populated with
        /// results as the runtime loads the anchors.</param>
        /// <param name="incrementalResultsCallback">A callback method that will be called when any requested
        /// anchors are loaded. This callback is invoked at least once if any anchors are successfully
        /// loaded, and possibly multiple times before the async operation completes. Pass a `null` argument for this
        /// parameter to ignore it.</param>
        /// <param name="cancellationToken">An optional `CancellationToken` that you can use to cancel the operation
        /// in progress if the loaded provider <see cref="XRAnchorSubsystemDescriptor.supportsAsyncCancellation"/>.</param>
        /// <returns>The result of the async operation. You are responsible to <see langword="await"/> this result.</returns>
        /// <exception cref="NotSupportedException"> Thrown if
        /// <see cref="XRAnchorSubsystemDescriptor.supportsLoadAnchor"/> is false for this provider.</exception>
        /// <exception cref="ArgumentNullException">Thrown if the `NativeArray` of anchor GUIDs to load is empty.</exception>
        /// <exception cref="ArgumentNullException">Thrown if the `IEnumerable` of saved anchor GUIDS is null or the output list for `LoadAnchorResult`s is null.</exception>
        /// <remarks>
        /// The order in which anchors are loaded may not match the enumeration order of <paramref name="savedAnchorGuidsToLoad"/>. To check
        /// if an anchor loaded successfully, check the <see cref="ARSaveOrLoadAnchorResult.resultStatus"/>.
        /// </remarks>
        public async Awaitable TryLoadAnchorsAsync(
            IEnumerable<SerializableGuid> savedAnchorGuidsToLoad,
            List<ARSaveOrLoadAnchorResult> outputLoadAnchorResults,
            Action<ReadOnlyListSpan<ARSaveOrLoadAnchorResult>> incrementalResultsCallback,
            CancellationToken cancellationToken = default)
        {
            if (savedAnchorGuidsToLoad == null)
                throw new ArgumentNullException(nameof(savedAnchorGuidsToLoad));

            if (outputLoadAnchorResults == null)
                throw new ArgumentNullException(nameof(outputLoadAnchorResults));

            if (!savedAnchorGuidsToLoad.Any())
                return;

            var savedAnchorGuids = new NativeArray<SerializableGuid>(
                savedAnchorGuidsToLoad.Count(),
                Allocator.Persistent);

            var index = 0;
            foreach (var savedAnchorGuid in savedAnchorGuidsToLoad)
            {
                savedAnchorGuids[index] = savedAnchorGuid;
                index += 1;
            }

            var completed = 0;
            var finalLoadAnchorResults = await subsystem.TryLoadAnchorsAsync(
                savedAnchorGuids,
                Allocator.Temp,
                xrLoadAnchorResults =>
                {
                    foreach (var xrLoadAnchorResult in xrLoadAnchorResults)
                    {
                        var loadAnchorResult = new ARSaveOrLoadAnchorResult
                        {
                            resultStatus = xrLoadAnchorResult.resultStatus,
                            savedAnchorGuid = xrLoadAnchorResult.savedAnchorGuid,
                            anchor = CreateTrackableImmediate(xrLoadAnchorResult.xrAnchor),
                        };

                        outputLoadAnchorResults.Add(loadAnchorResult);
                    }

                    var loadAnchorResults = new ReadOnlyListSpan<ARSaveOrLoadAnchorResult>(
                        outputLoadAnchorResults,
                        completed,
                        xrLoadAnchorResults.Length);

                    incrementalResultsCallback?.Invoke(loadAnchorResults);
                    completed = outputLoadAnchorResults.Count;
                },
                cancellationToken);

            for (var i = outputLoadAnchorResults.Count; i < finalLoadAnchorResults.Length; i += 1)
            {
                var failedAnchorResult = new ARSaveOrLoadAnchorResult
                {
                    resultStatus = finalLoadAnchorResults[i].resultStatus,
                    savedAnchorGuid = finalLoadAnchorResults[i].savedAnchorGuid,
                    anchor = null,
                };

                outputLoadAnchorResults.Add(failedAnchorResult);
            }

            savedAnchorGuids.Dispose();
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
        /// Attempts to erase the persistent saved data associated with a batch of anchors given their persistent anchor
        /// GUIDs.</summary>
        /// <param name="savedAnchorGuidsToErase">The persistent anchor GUIDs created by <see cref="TrySaveAnchorAsync"/> or
        /// <see cref="TrySaveAnchorsAsync"/>.</param>
        /// <param name="outputEraseAnchorResults">The output list that will be cleared and populated with `EraseAnchorResult`s.</param>
        /// <param name="cancellationToken">An optional `CancellationToken` that you can use to cancel the operation
        /// in progress if the loaded provider <see cref="XRAnchorSubsystemDescriptor.supportsAsyncCancellation"/>.</param>
        /// <returns>The result of the async operation. You are responsible to <see langword="await"/> this result.</returns>
        /// <exception cref="NotSupportedException">Thrown if
        /// <see cref="XRAnchorSubsystemDescriptor.supportsEraseAnchor"/> is false for this provider.</exception>
        /// <exception cref="ArgumentNullException">Thrown if the `IEnumerable` of anchor GUIDs to erase is null or the output list of `EraseAnchorResult`s is null.</exception>
        public async Awaitable TryEraseAnchorsAsync(
            IEnumerable<SerializableGuid> savedAnchorGuidsToErase,
            List<XREraseAnchorResult> outputEraseAnchorResults,
            CancellationToken cancellationToken = default)
        {
            if (savedAnchorGuidsToErase == null)
                throw new ArgumentNullException(nameof(savedAnchorGuidsToErase));

            if (outputEraseAnchorResults == null)
                throw new ArgumentNullException(nameof(outputEraseAnchorResults));

            if (!savedAnchorGuidsToErase.Any())
                return;

            var nativeSavedAnchorGuids = new NativeArray<SerializableGuid>(
                savedAnchorGuidsToErase.Count(),
                Allocator.Persistent);

            var index = 0;
            foreach (var savedAnchorGuid in savedAnchorGuidsToErase)
            {
                nativeSavedAnchorGuids[index] = savedAnchorGuid;
                index += 1;
            }

            var eraseAnchorResults = await subsystem.TryEraseAnchorsAsync(
                nativeSavedAnchorGuids,
                Allocator.Temp,
                cancellationToken);

            outputEraseAnchorResults.Clear();
            foreach (var eraseAnchorResult in eraseAnchorResults)
            {
                outputEraseAnchorResults.Add(eraseAnchorResult);
            }

            nativeSavedAnchorGuids.Dispose();
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
