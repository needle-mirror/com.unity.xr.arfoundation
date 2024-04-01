using System;
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
        /// If you are implementing your own custom subsystem [Lifecycle management](xref:xr-plug-in-management-provider#lifecycle-management),
        /// use the [SubsystemManager](xref:UnityEngine.SubsystemManager)
        /// to enumerate the available <see cref="XRAnchorSubsystemDescriptor"/>s, then call
        /// <see cref="XRAnchorSubsystemDescriptor.Register">XRAnchorSubsystemDescriptor.Register()</see> on the desired descriptor.
        /// </remarks>
        public XRAnchorSubsystem() { }

        /// <summary>
        /// Get the changes to anchors (added, updated, and removed) since the last call to this method.
        /// </summary>
        /// <param name="allocator">An allocator to use for the <c>NativeArray</c>s in <see cref="TrackableChanges{T}"/>.</param>
        /// <returns>Changes since the last call to this method.</returns>
        public override TrackableChanges<XRAnchor> GetChanges(Allocator allocator)
        {
            if (!running)
                throw new InvalidOperationException($"Can't call {nameof(GetChanges)} without \"Start\"ing the anchor subsystem!");

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
            /// <see cref="TrackableChanges{T}(int,int,int,Allocator,T)"/>.
            /// </param>
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
