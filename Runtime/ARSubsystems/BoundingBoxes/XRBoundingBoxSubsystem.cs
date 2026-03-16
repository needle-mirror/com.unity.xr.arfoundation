using System;
using Unity.Collections;
using UnityEngine.SubsystemsImplementation;

namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// This subsystem provides information regarding the detection of 3D bounding boxes in the physical environment.
    /// </summary>
    /// <remarks>
    /// This is a base class with an abstract provider type to be implemented by provider plug-in packages.
    /// This class itself does not implement bounding box detection.
    /// </remarks>
    public class XRBoundingBoxSubsystem :
        TrackingSubsystem<XRBoundingBox, XRBoundingBoxSubsystem, XRBoundingBoxSubsystemDescriptor, XRBoundingBoxSubsystem.Provider>
    {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        ValidationUtility<XRBoundingBox> m_ValidationUtility = new();
#endif

        /// <summary>
        /// Do not invoke this constructor directly.
        /// </summary>
        /// <remarks>
        /// If you are implementing your own custom subsystem [Lifecycle management](xref:xr-plug-in-management-provider#lifecycle-management),
        /// use the [SubsystemManager](xref:UnityEngine.SubsystemManager)
        /// to enumerate the available <see cref="XRBoundingBoxSubsystemDescriptor"/>s, then call
        /// <see cref="XRBoundingBoxSubsystemDescriptor.Register">XRBoundingBoxSubsystemDescriptor.Register()</see> on the desired descriptor.
        /// </remarks>
        public XRBoundingBoxSubsystem() { }

        /// <summary>
        /// Gets a <see cref="TrackableChanges{T}"/> struct containing any changes to detected bounding boxes since the last
        /// time you called this method. You are responsible to <see cref="TrackableChanges{T}.Dispose"/> the returned
        /// <c>TrackableChanges</c> instance.
        /// </summary>
        /// <param name="allocator">An <c>Allocator</c> to use for the returned <c>NativeArray</c>s.</param>
        /// <returns>The <see cref="TrackableChanges{T}"/>.</returns>
        /// <remarks>
        /// The <see cref="TrackableChanges{T}"/> struct returned by this method contains separate
        /// <see cref="NativeArray{T}"/> objects for the added, updated, and removed bounding boxes. These arrays are created
        /// using the <see cref="Allocator"/> type specified by <paramref name="allocator"/>.
        /// </remarks>
        public override TrackableChanges<XRBoundingBox> GetChanges(Allocator allocator)
        {
            var changes = provider.GetChanges(default, allocator);
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            m_ValidationUtility.ValidateAndDisposeIfThrown(changes);
#endif
            return changes;
        }

        /// <summary>
        /// The provider API for <see cref="XRBoundingBoxSubsystem"/>-derived classes to implement.
        /// </summary>
        public abstract class Provider : SubsystemProvider<XRBoundingBoxSubsystem>
        {
            /// <summary>
            /// Gets a <see cref="TrackableChanges{T}"/> struct containing any changes to detected bounding boxes since the last
            /// time you called this method. You are responsible to <see cref="TrackableChanges{T}.Dispose"/> the returned
            /// <c>TrackableChanges</c> instance.
            /// </summary>
            /// <param name="defaultXRBoundingBox">The default bounding box. You should use this to initialize the returned
            ///   <see cref="TrackableChanges{T}"/> instance by passing it to the constructor
            ///   <see cref="TrackableChanges{T}(int,int,int,Allocator,T)"/>.
            /// </param>
            /// <param name="allocator">An <c>Allocator</c> to use when allocating the returned <c>NativeArray</c>s.</param>
            /// <returns>The changes to bounding boxes since the last call to this method.</returns>
            public abstract TrackableChanges<XRBoundingBox> GetChanges(XRBoundingBox defaultXRBoundingBox, Allocator allocator);
        }
    }
}
