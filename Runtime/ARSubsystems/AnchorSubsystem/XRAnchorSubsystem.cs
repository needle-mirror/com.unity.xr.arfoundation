using System;
using System.Collections.Generic;
using System.Threading;
using Unity.Collections;
using UnityEngine.SubsystemsImplementation;

namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// This subsystem provides information regarding anchors. An anchor is a pose (position and rotation) in the physical
    /// environment that is tracked by an XR device. Anchors are updated as the device refines its understanding of the
    /// environment, allowing you to reliably place virtual content at a real-world pose.
    /// </summary>
    /// <remarks>
    /// This is a base class with an abstract provider type to be implemented by provider plug-in packages.
    /// This class itself does not implement anchor tracking.
    /// </remarks>
    public class XRAnchorSubsystem
        : TrackingSubsystem<XRAnchor, XRAnchorSubsystem, XRAnchorSubsystemDescriptor, XRAnchorSubsystem.Provider>
    {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        ValidationUtility<XRAnchor> m_ValidationUtility = new();
#endif

        /// <summary>
        /// Do not invoke this constructor directly.
        /// </summary>
        /// <remarks>
        /// If you are implementing your own custom subsystem
        /// [Lifecycle management](xref:xr-plug-in-management-provider#lifecycle-management), use the
        /// [SubsystemManager](xref:UnityEngine.SubsystemManager) to enumerate the available
        /// <see cref="XRAnchorSubsystemDescriptor"/>s, then call
        /// <see cref="XRAnchorSubsystemDescriptor.Register">XRAnchorSubsystemDescriptor.Register()</see> on the
        /// desired descriptor.
        /// </remarks>
        public XRAnchorSubsystem() { }

        /// <summary>
        /// Get the changes to anchors (added, updated, and removed) since the last call to this method.
        /// </summary>
        /// <param name="allocator">An allocator to use for the <c>NativeArray</c>s in <see cref="TrackableChanges{T}"/>.</param>
        /// <returns>Changes since the last call to this method.</returns>
        public override TrackableChanges<XRAnchor> GetChanges(Allocator allocator)
        {
            var changes = provider.GetChanges(XRAnchor.defaultValue, allocator);
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            m_ValidationUtility.ValidateAndDisposeIfThrown(changes);
#endif
            return changes;
        }

        /// <summary>
        /// Attempts to create a new anchor at the given <paramref name="pose"/>.
        /// </summary>
        /// <param name="pose">The pose, in session space, of the new anchor.</param>
        /// <param name="anchor">The new anchor. Only valid if this method returns <see langword="true"/>.</param>
        /// <returns><see langword="true"/> if the output anchor was added. Otherwise, <see langword="false"/>.</returns>
        /// <seealso cref="XRAnchorSubsystemDescriptor.supportsSynchronousAdd"/>
        public bool TryAddAnchor(Pose pose, out XRAnchor anchor)
        {
            return provider.TryAddAnchor(pose, out anchor);
        }

        /// <summary>
        /// Attempts to create a new anchor at the given <paramref name="pose"/>.
        /// </summary>
        /// <param name="pose">The pose, in session space, of the new anchor.</param>
        /// <returns>The result of the async operation. You are responsible to <see langword="await"/> this result.</returns>
        public Awaitable<Result<XRAnchor>> TryAddAnchorAsync(Pose pose)
        {
            return provider.TryAddAnchorAsync(pose);
        }

        /// <summary>
        /// Attempts to create a new anchor "attached" to the trackable with id <paramref name="trackableToAffix"/>.
        /// The behavior of the anchor depends on the type of trackable to which this anchor is attached.
        /// </summary>
        /// <param name="trackableToAffix">The id of the trackable to which to attach.</param>
        /// <param name="pose">The pose, in session space, of the anchor to create.</param>
        /// <param name="anchor">The new anchor. Only valid if this method returns <see langword="true"/>.</param>
        /// <returns><see langword="true"/> if the new anchor was added. Otherwise, <see langword="false"/>.</returns>
        public bool TryAttachAnchor(TrackableId trackableToAffix, Pose pose, out XRAnchor anchor)
        {
            return provider.TryAttachAnchor(trackableToAffix, pose, out anchor);
        }

        /// <summary>
        /// Attempts to remove an existing anchor with <see cref="TrackableId"/> <paramref name="anchorId"/>.
        /// </summary>
        /// <param name="anchorId">The id of an existing anchor to remove.</param>
        /// <returns><see langword="true"/> if the anchor was removed. Otherwise, <see langword="false"/>.</returns>
        public bool TryRemoveAnchor(TrackableId anchorId)
        {
            return provider.TryRemoveAnchor(anchorId);
        }

        /// <summary>
        /// Attempts to persistently save the given anchor so that it can be loaded in a future AR session. Use the
        /// `SerializableGuid` returned by this method as an input parameter to <see cref="TryLoadAnchorAsync"/> or
        /// <see cref="TryEraseAnchorAsync"/>.
        /// </summary>
        /// <param name="anchorId">The TrackableId of the anchor to save.</param>
        /// <param name="cancellationToken">An optional `CancellationToken` that you can use to cancel the operation
        /// in progress if the loaded provider <see cref="XRAnchorSubsystemDescriptor.supportsAsyncCancellation"/>.</param>
        /// <returns>The result of the async operation, containing a new persistent anchor GUID if the operation
        /// succeeded. You are responsible to <see langword="await"/> this result.</returns>
        /// <seealso cref="XRAnchorSubsystemDescriptor.supportsSaveAnchor"/>
        public Awaitable<Result<SerializableGuid>> TrySaveAnchorAsync(
            TrackableId anchorId, CancellationToken cancellationToken = default)
        {
            return provider.TrySaveAnchorAsync(anchorId, cancellationToken);
        }

        /// <summary>
        /// Attempts to persistently save the given anchors so that they can be loaded in a future AR session. Use the
        /// `SerializableGuid`s returned from this method as an input to <see cref="TryLoadAnchorsAsync"/> or
        /// <see cref="TryEraseAnchorsAsync"/>.
        /// </summary>
        /// <param name="anchorIdsToSave">The `TrackableId`s of the anchors to save. You can create an anchor using <see cref="TryAddAnchorAsync"/>.</param>
        /// <param name="allocator">The allocator used for the returned `NativeArray` of saved anchor GUIDs.</param>
        /// <param name="cancellationToken">An optional `CancellationToken` that you can use to cancel the operation
        /// in progress if the loaded provider <see cref="XRAnchorSubsystemDescriptor.supportsAsyncCancellation"/>.</param>
        /// <returns>The result of the async operation. You are responsible to <see langword="await"/> this result.</returns>
        /// <exception cref="NotSupportedException">Thrown if
        /// <see cref="XRAnchorSubsystemDescriptor.supportsSaveAnchor"/> is false for this provider.</exception>
        public Awaitable<NativeArray<XRSaveAnchorResult>> TrySaveAnchorsAsync(
            NativeArray<TrackableId> anchorIdsToSave,
            Allocator allocator,
            CancellationToken cancellationToken = default)
        {
            return provider.TrySaveAnchorsAsync(
                anchorIdsToSave,
                allocator,
                cancellationToken);
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
        public Awaitable<Result<XRAnchor>> TryLoadAnchorAsync(
            SerializableGuid savedAnchorGuid, CancellationToken cancellationToken = default)
        {
            return provider.TryLoadAnchorAsync(savedAnchorGuid, cancellationToken);
        }

        /// <summary>
        /// Attempts to load a batch of anchors given their persistent anchor GUIDs.
        /// </summary>
        /// <param name="savedAnchorGuidsToLoad">The persistent anchor GUIDs to load that were created by
        /// <see cref="TrySaveAnchorsAsync"/> or <see cref="TrySaveAnchorAsync"/>.</param>
        /// <param name="allocator">The allocator used for the returned `NativeArray`.</param>
        /// <param name="incrementalResultsCallback">A callback method that will be called when any requested
        /// anchors are loaded. Pass a `null` argument for this parameter to ignore it. The `NativeArray` passed to you
        /// uses `Allocation.Temp`.</param>
        /// <param name="cancellationToken">An optional `CancellationToken` that you can use to cancel the operation
        /// in progress if the loaded provider <see cref="XRAnchorSubsystemDescriptor.supportsAsyncCancellation"/>.</param>
        /// <returns>The result of the async operation. You are responsible to
        /// <see langword="await"/> this result.</returns>
        /// <exception cref="NotSupportedException">Thrown if
        /// <see cref="XRAnchorSubsystemDescriptor.supportsLoadAnchor"/> is false for this provider.</exception>
        /// <remarks>
        /// The order in which anchors are loaded may not match the enumeration order of
        /// <paramref name="savedAnchorGuidsToLoad"/>. To check if an anchor loaded successfully, check the
        /// <see cref="XRLoadAnchorResult.resultStatus"/>.
        /// </remarks>
        public Awaitable<NativeArray<XRLoadAnchorResult>> TryLoadAnchorsAsync(
            NativeArray<SerializableGuid> savedAnchorGuidsToLoad,
            Allocator allocator,
            Action<NativeArray<XRLoadAnchorResult>> incrementalResultsCallback,
            CancellationToken cancellationToken = default)
        {
            return provider.TryLoadAnchorsAsync(
                savedAnchorGuidsToLoad,
                allocator,
                incrementalResultsCallback,
                cancellationToken);
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
            return provider.TryEraseAnchorAsync(savedAnchorGuid, cancellationToken);
        }

        /// <summary>
        /// Attempts to erase the persistent saved data associated with a batch of anchors given their persistent anchor
        /// GUIDs.</summary>
        /// <param name="savedAnchorGuids">The persistent anchor GUIDs created by <see cref="TrySaveAnchorAsync"/> or
        /// <see cref="TrySaveAnchorsAsync"/>.</param>
        /// <param name="allocator">The allocator of the returned `NativeArray`.</param>
        /// <param name="cancellationToken">An optional `CancellationToken` that you can use to cancel the operation
        /// in progress if the loaded provider <see cref="XRAnchorSubsystemDescriptor.supportsAsyncCancellation"/>.</param>
        /// <returns>The result of the async operation. You are responsible to <see langword="await"/> this result.</returns>
        /// <exception cref="NotSupportedException">Thrown if
        /// <see cref="XRAnchorSubsystemDescriptor.supportsEraseAnchor"/> is false for this provider.</exception>
        public Awaitable<NativeArray<XREraseAnchorResult>> TryEraseAnchorsAsync(
            NativeArray<SerializableGuid> savedAnchorGuids,
            Allocator allocator,
            CancellationToken cancellationToken = default)
        {
            return provider.TryEraseAnchorsAsync(savedAnchorGuids, allocator, cancellationToken);
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
            return provider.TryGetSavedAnchorIdsAsync(allocator, cancellationToken);
        }

        /// <summary>
        /// An abstract class to be implemented by providers of this subsystem.
        /// </summary>
        public abstract class Provider : SubsystemProvider<XRAnchorSubsystem>
        {
            static readonly Pool.ObjectPool<Dictionary<TrackableId, Awaitable<Result<SerializableGuid>>>> s_SaveAwaitablesMaps =
                new(
                    createFunc: () => new Dictionary<TrackableId, Awaitable<Result<SerializableGuid>>>(),
                    actionOnGet: null,
                    actionOnRelease: null,
                    actionOnDestroy: null,
                    collectionCheck: false,
                    defaultCapacity: 2,
                    maxSize: 1024);

            static readonly Pool.ObjectPool<Dictionary<SerializableGuid, Awaitable<Result<XRAnchor>>>> s_LoadAwaitablesMaps =
                new(
                    createFunc: () => new Dictionary<SerializableGuid, Awaitable<Result<XRAnchor>>>(),
                    actionOnGet: null,
                    actionOnRelease: null,
                    actionOnDestroy: null,
                    collectionCheck: false,
                    defaultCapacity: 2,
                    maxSize: 1024);

            static readonly Pool.ObjectPool<List<XRLoadAnchorResult>> s_AccumulatedXRLoadAnchorResultLists =
                new(
                    createFunc: () => new List<XRLoadAnchorResult>(),
                    actionOnGet: null,
                    actionOnRelease: null,
                    actionOnDestroy: null,
                    collectionCheck: false,
                    defaultCapacity: 2,
                    maxSize: 1024);

            static readonly Pool.ObjectPool<Dictionary<SerializableGuid, Awaitable<XRResultStatus>>> s_EraseAwaitablesMaps =
                new(
                    createFunc: () => new Dictionary<SerializableGuid, Awaitable<XRResultStatus>>(),
                    actionOnGet: null,
                    actionOnRelease: null,
                    actionOnDestroy: null,
                    collectionCheck: false,
                    defaultCapacity: 2,
                    maxSize: 1024);

            static readonly Pool.ObjectPool<List<XREraseAnchorResult>> s_EraseAnchorResultLists =
                new(
                    createFunc: () => new List<XREraseAnchorResult>(),
                    actionOnGet: null,
                    actionOnRelease: null,
                    actionOnDestroy: null,
                    collectionCheck: false,
                    defaultCapacity: 2,
                    maxSize: 1024);

            /// <summary>
            /// Reusable completion source to return results of <see cref="TryAddAnchor"/> to <see cref="TryAddAnchorAsync"/>.
            /// </summary>
            static AwaitableCompletionSource<Result<XRAnchor>> s_SynchronousAnchorCompletionSource = new();

            /// <summary>
            /// Gets a <see cref="TrackableChanges{T}"/> struct containing any changes to anchors since the last
            /// time you called this method. You are responsible to <see cref="TrackableChanges{T}.Dispose"/> the returned
            /// <c>TrackableChanges</c> instance.
            /// </summary>
            /// <param name="defaultAnchor">The default anchor. You should use this to initialize the returned
            /// <see cref="TrackableChanges{T}"/> instance by passing it to the constructor
            /// <see cref="TrackableChanges{T}(int,int,int,Allocator,T)"/>.</param>
            /// <param name="allocator">An <c>Allocator</c> to use when allocating the returned <c>NativeArray</c>s.</param>
            /// <returns>The changes to anchors since the last call to this method.</returns>
            public abstract TrackableChanges<XRAnchor> GetChanges(XRAnchor defaultAnchor, Allocator allocator);

            /// <summary>
            /// Attempts to create a new anchor at the given <paramref name="pose"/>.
            /// </summary>
            /// <param name="pose">The pose, in session space, of the new anchor.</param>
            /// <param name="anchor">The output anchor. Valid only if this method returns <see langword="true"/>.</param>
            /// <returns><see langword="true"/> if the new anchor was added. Otherwise, <see langword="false"/>.</returns>
            /// <seealso cref="XRAnchorSubsystemDescriptor.supportsSynchronousAdd"/>
            public virtual bool TryAddAnchor(Pose pose, out XRAnchor anchor)
            {
                anchor = default;
                return false;
            }

            /// <summary>
            /// Attempts to create a new anchor at the given <paramref name="pose"/>.
            /// </summary>
            /// <param name="pose">The pose, in session space, of the new anchor.</param>
            /// <returns>The result of the async operation, containing the newly added anchor if the operation succeeded.
            /// You are responsible to <see langword="await"/> this result.</returns>
            /// <remarks>
            /// The default implementation of this method will attempt to create and return a result using
            /// the class's implementation of <see cref="TryAddAnchor"/>.
            /// </remarks>
            public virtual Awaitable<Result<XRAnchor>> TryAddAnchorAsync(Pose pose)
            {
                using (new ScopedProfiler("XRAnchorSubsystem.TryAddAnchorAsync"))
                {
                    var wasSuccessful = TryAddAnchor(pose, out var anchor);
                    return AwaitableUtils<Result<XRAnchor>>.FromResult(
                        s_SynchronousAnchorCompletionSource, new Result<XRAnchor>(wasSuccessful, anchor));
                }
            }

            /// <summary>
            /// Attempts to create a new anchor "attached" to the trackable with id <paramref name="trackableToAffix"/>.
            /// The behavior of the anchor depends on the type of trackable to which this anchor is attached.
            /// </summary>
            /// <param name="trackableToAffix">The id of the trackable to which to attach.</param>
            /// <param name="pose">The pose, in session space, of the anchor to create.</param>
            /// <param name="anchor">The new anchor. Only valid if this method returns <see langword="true"/>.</param>
            /// <returns><see langword="true"/> if the new anchor was added. Otherwise, <see langword="false"/>.</returns>
            public virtual bool TryAttachAnchor(
                TrackableId trackableToAffix,
                Pose pose,
                out XRAnchor anchor)
            {
                anchor = default;
                return false;
            }

            /// <summary>
            /// Attempts to remove an existing anchor with <see cref="TrackableId"/> <paramref name="anchorId"/>.
            /// </summary>
            /// <param name="anchorId">The id of an existing anchor to remove.</param>
            /// <returns><see langword="true"/> if the anchor was removed. Otherwise, <see langword="false"/>.</returns>
            public virtual bool TryRemoveAnchor(TrackableId anchorId) => false;

            /// <summary>
            /// Attempts to persistently save the given anchor so that it can be loaded in a future AR session. Use the
            /// `SerializableGuid` returned by this method as an input parameter to <see cref="TryLoadAnchorAsync"/> or
            /// <see cref="TryEraseAnchorAsync"/>.
            /// </summary>
            /// <param name="anchorId">The TrackableId of the anchor to save.</param>
            /// <param name="cancellationToken">An optional `CancellationToken` that you can use to cancel the operation
            /// in progress if the loaded provider <see cref="XRAnchorSubsystemDescriptor.supportsAsyncCancellation"/>.</param>
            /// <returns>The result of the async operation, containing a new persistent anchor GUID if the operation
            /// succeeded. You are responsible to <see langword="await"/> this result.</returns>
            /// <seealso cref="XRAnchorSubsystemDescriptor.supportsSaveAnchor"/>
            public virtual Awaitable<Result<SerializableGuid>> TrySaveAnchorAsync(
                TrackableId anchorId, CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException(
                    $"Check the value of {nameof(XRAnchorSubsystemDescriptor.supportsSaveAnchor)} before you call this API.");
            }

            /// <summary>
            /// Attempts to persistently save the given anchors so that they can be loaded in a future AR session. Use the
            /// `SerializableGuid`s returned from this method as an input to <see cref="TryLoadAnchorsAsync"/> or
            /// <see cref="TryEraseAnchorsAsync"/>.
            /// </summary>
            /// <param name="anchorIds">The `TrackableId`s of the anchors to save. You can create an anchor using
            /// <see cref="TryAddAnchorAsync"/>.</param>
            /// <param name="allocator">The allocator used for the returned `NativeArray` of saved anchor GUIDs.</param>
            /// <param name="cancellationToken">An optional `CancellationToken` that you can use to cancel the operation
            /// in progress if the loaded provider <see cref="XRAnchorSubsystemDescriptor.supportsAsyncCancellation"/>.
            /// </param>
            /// <returns>The result of the async operation, You are responsible to <see langword="await"/> this result.</returns>
            /// <exception cref="ArgumentException">Thrown if the `NativeArray` of saved anchor Ids to save has been disposed.</exception>
            /// <exception cref="NotSupportedException">Thrown if
            /// <see cref="XRAnchorSubsystemDescriptor.supportsSaveAnchor"/> is false for this provider.</exception>
            public virtual async Awaitable<NativeArray<XRSaveAnchorResult>> TrySaveAnchorsAsync(
                NativeArray<TrackableId> anchorIds,
                Allocator allocator,
                CancellationToken cancellationToken = default)
            {
                if (!anchorIds.IsCreated)
                    throw new ArgumentException(nameof(anchorIds));

                if (anchorIds.Length == 0)
                    return default;

                var awaitableByTrackableId = s_SaveAwaitablesMaps.Get();
                for (var i = 0; i < anchorIds.Length; i += 1)
                {
                    awaitableByTrackableId.Add(
                        anchorIds[i],
                        TrySaveAnchorAsync(anchorIds[i], cancellationToken));
                }

                var saveAnchorResults = new NativeArray<XRSaveAnchorResult>(
                    awaitableByTrackableId.Count,
                    Allocator.Persistent);

                var index = 0;
                foreach (var (trackableId, awaitable) in awaitableByTrackableId)
                {
                    var result = await awaitable;
                    var saveAnchorResult = new XRSaveAnchorResult
                    {
                        resultStatus = result.status,
                        trackableId = trackableId,
                        savedAnchorGuid = result.value
                    };

                    saveAnchorResults[index] = saveAnchorResult;
                    index += 1;
                }

                awaitableByTrackableId.Clear();
                s_SaveAwaitablesMaps.Release(awaitableByTrackableId);

                if (allocator != Allocator.Persistent)
                {
                    var finalSaveAnchorResults = new NativeArray<XRSaveAnchorResult>(saveAnchorResults.Length, allocator);
                    NativeArray<XRSaveAnchorResult>.Copy(saveAnchorResults, finalSaveAnchorResults);
                    saveAnchorResults.Dispose();
                    return finalSaveAnchorResults;
                }

                return saveAnchorResults;
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
            public virtual Awaitable<Result<XRAnchor>> TryLoadAnchorAsync(
                SerializableGuid savedAnchorGuid, CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException(
                    $"Check the value of {nameof(XRAnchorSubsystemDescriptor.supportsLoadAnchor)} before you call this API.");
            }

            /// <summary>
            /// Attempts to load a batch of anchors given their persistent anchor GUIDs.
            /// </summary>
            /// <param name="savedAnchorGuids">The persistent anchor GUIDs to load that were created by
            /// <see cref="TrySaveAnchorsAsync"/> or <see cref="TrySaveAnchorAsync"/>.</param>
            /// <param name="allocator">The allocator used for the returned `NativeArray` of `XRAnchors`.</param>
            /// <param name="incrementalResultsCallback">A callback method that will be called when any requested
            /// anchors are loaded. The `NativeArray` parameter will use `Allocation.Temp`. Pass a `null` argument for
            /// this parameter to ignore it.</param>
            /// <param name="cancellationToken">An optional `CancellationToken` that you can use to cancel the operation
            /// in progress if the loaded provider <see cref="XRAnchorSubsystemDescriptor.supportsAsyncCancellation"/>.</param>
            /// <returns>The result of the async operation. You are responsible to
            /// <see langword="await"/> this result.</returns>
            /// <exception cref="ArgumentException">Thrown if the `NativeArray` of saved anchor GUIDs to load has been disposed.</exception>
            /// <exception cref="NotSupportedException">Thrown if
            /// <see cref="XRAnchorSubsystemDescriptor.supportsLoadAnchor"/> is false for this provider.</exception>
            /// <remarks>
            /// The order anchors are loaded in is not guaranteed to match the order they were requested in.
            /// To check if an anchor loaded successfully, check the <see cref="XRLoadAnchorResult.resultStatus"/>.
            /// Anchors reported by incremental results will always have a successful `resultStatus`.
            /// </remarks>
            public virtual async Awaitable<NativeArray<XRLoadAnchorResult>> TryLoadAnchorsAsync(
                NativeArray<SerializableGuid> savedAnchorGuids,
                Allocator allocator,
                Action<NativeArray<XRLoadAnchorResult>> incrementalResultsCallback,
                CancellationToken cancellationToken = default)
            {
                if (!savedAnchorGuids.IsCreated)
                    throw new ArgumentException(nameof(savedAnchorGuids));

                if (savedAnchorGuids.Length == 0)
                    return default;

                var awaitableBySavedAnchorGuid = s_LoadAwaitablesMaps.Get();
                var accumulatedResults = s_AccumulatedXRLoadAnchorResultLists.Get();
                for (var i = 0; i < savedAnchorGuids.Length; i += 1)
                {
                    var awaitable = TryLoadAnchorAsync(savedAnchorGuids[i], cancellationToken);
                    awaitableBySavedAnchorGuid.Add(savedAnchorGuids[i], awaitable);
                }

                if (incrementalResultsCallback == null)
                {
                    foreach (var (savedAnchorGuid, awaitable) in awaitableBySavedAnchorGuid)
                    {
                        var result = await awaitable;
                        var loadAnchorResult = new XRLoadAnchorResult
                        {
                            resultStatus = result.status,
                            savedAnchorGuid = savedAnchorGuid,
                            xrAnchor = result.value
                        };

                        accumulatedResults.Add(loadAnchorResult);
                    }
                }
                else
                {
                    var completedCount = 0;
                    foreach (var (savedAnchorGuid, awaitable) in awaitableBySavedAnchorGuid)
                    {
                        awaitable.GetAwaiter().OnCompleted(() =>
                            OnIncrementalLoadComplete(
                                awaitable,
                                savedAnchorGuid,
                                incrementalResultsCallback,
                                in accumulatedResults,
                                ref completedCount));
                    }

                    while (completedCount < savedAnchorGuids.Length)
                    {
                        await Awaitable.NextFrameAsync();
                    }
                }

                var finalLoadAnchorResults = new NativeArray<XRLoadAnchorResult>(accumulatedResults.Count, allocator);
                for (var i = 0; i < accumulatedResults.Count; i += 1)
                {
                    finalLoadAnchorResults[i] = accumulatedResults[i];
                }

                awaitableBySavedAnchorGuid.Clear();
                s_LoadAwaitablesMaps.Release(awaitableBySavedAnchorGuid);
                accumulatedResults.Clear();
                s_AccumulatedXRLoadAnchorResultLists.Release(accumulatedResults);
                return finalLoadAnchorResults;
            }

            void OnIncrementalLoadComplete(
                Awaitable<Result<XRAnchor>> awaitable,
                SerializableGuid savedAnchorGuid,
                Action<NativeArray<XRLoadAnchorResult>> incrementalResultsCallback,
                in List<XRLoadAnchorResult> accumulatedResults,
                ref int completedCount)
            {
                var result = awaitable.GetAwaiter().GetResult();
                var loadAnchorResult = new XRLoadAnchorResult
                {
                    resultStatus = result.status,
                    savedAnchorGuid = savedAnchorGuid,
                    xrAnchor = result.value
                };

                accumulatedResults.Add(loadAnchorResult);

                var tempLoadAnchorResults = new NativeArray<XRLoadAnchorResult>(1, Allocator.Temp);
                tempLoadAnchorResults[0] = loadAnchorResult;
                incrementalResultsCallback.Invoke(tempLoadAnchorResults);
                completedCount += 1;
            }

            /// <summary>
            /// Attempts to erase the persistent saved data associated with an anchor given its persistent anchor GUID.
            /// </summary>
            /// <param name="savedAnchorGuid">A persistent anchor GUID created by <see cref="TrySaveAnchorAsync"/>.</param>
            /// <param name="cancellationToken">An optional `CancellationToken` that you can use to cancel the operation
            /// in progress if the loaded provider <see cref="XRAnchorSubsystemDescriptor.supportsAsyncCancellation"/>.</param>
            /// <returns>The result of the async operation. You are responsible to <see langword="await"/> this result.</returns>
            /// <seealso cref="XRAnchorSubsystemDescriptor.supportsEraseAnchor"/>
            public virtual Awaitable<XRResultStatus> TryEraseAnchorAsync(
                SerializableGuid savedAnchorGuid, CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException(
                    $"Check the value of {nameof(XRAnchorSubsystemDescriptor.supportsEraseAnchor)} before you call this API.");
            }

            /// <summary>
            /// Attempts to erase the persistent saved data associated with a batch of anchors given their persistent
            /// anchor GUIDs. </summary>
            /// <param name="savedAnchorGuids">The persistent anchor GUIDs created by <see cref="TrySaveAnchorAsync"/>
            /// or <see cref="TrySaveAnchorsAsync"/>.</param>
            /// <param name="allocator">The allocation strategy of the returned `NativeArray`.</param>
            /// <param name="cancellationToken">An optional `CancellationToken` that you can use to cancel the operation
            /// in progress if the loaded provider <see cref="XRAnchorSubsystemDescriptor.supportsAsyncCancellation"/>.</param>
            /// <returns>The result of the async operation. You are responsible to <see langword="await"/> this result.</returns>
            /// <exception cref="ArgumentException">Thrown if the `NativeArray` of saved anchor GUIDs to erase has been disposed.</exception>
            /// <exception cref="NotSupportedException">Thrown if
            /// <see cref="XRAnchorSubsystemDescriptor.supportsEraseAnchor"/> is false for this provider.</exception>
            /// <exception cref="ArgumentException">Thrown if the `NativeArray` of saved anchor GUIDs to erase is empty.</exception>
            public virtual async Awaitable<NativeArray<XREraseAnchorResult>> TryEraseAnchorsAsync(
                NativeArray<SerializableGuid> savedAnchorGuids,
                Allocator allocator,
                CancellationToken cancellationToken = default)
            {
                if (!savedAnchorGuids.IsCreated)
                    throw new ArgumentException(nameof(savedAnchorGuids));

                var awaitableBySavedAnchorGuid = s_EraseAwaitablesMaps.Get();
                for (var i = 0; i < savedAnchorGuids.Length; i += 1)
                {
                    awaitableBySavedAnchorGuid.Add(
                        savedAnchorGuids[i],
                        TryEraseAnchorAsync(savedAnchorGuids[i], cancellationToken));
                }

                var tempEraseAnchorResults = s_EraseAnchorResultLists.Get();
                foreach (var (savedAnchorGuid, awaitable) in awaitableBySavedAnchorGuid)
                {
                    var resultStatus = await awaitable;
                    var eraseAnchorResult = new XREraseAnchorResult
                    {
                        resultStatus = resultStatus,
                        savedAnchorGuid = savedAnchorGuid,
                    };

                    tempEraseAnchorResults.Add(eraseAnchorResult);
                }

                var finalEraseAnchorResults = new NativeArray<XREraseAnchorResult>(tempEraseAnchorResults.Count, allocator);
                for (var i = 0; i < tempEraseAnchorResults.Count; i += 1)
                {
                    finalEraseAnchorResults[i] = tempEraseAnchorResults[i];
                }

                awaitableBySavedAnchorGuid.Clear();
                s_EraseAwaitablesMaps.Release(awaitableBySavedAnchorGuid);
                tempEraseAnchorResults.Clear();
                s_EraseAnchorResultLists.Release(tempEraseAnchorResults);
                return finalEraseAnchorResults;
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
            public virtual Awaitable<Result<NativeArray<SerializableGuid>>> TryGetSavedAnchorIdsAsync(
                Allocator allocator, CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException(
                    $"Check the value of {nameof(XRAnchorSubsystemDescriptor.supportsGetSavedAnchorIds)} before you call this API.");
            }
        }
    }
}
