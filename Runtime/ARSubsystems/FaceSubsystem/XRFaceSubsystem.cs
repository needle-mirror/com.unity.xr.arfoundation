using System;
using Unity.Collections;
using UnityEngine.SubsystemsImplementation;

namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// An abstract class that provides a generic API for low-level face tracking features.
    /// </summary>
    /// <remarks>
    /// Use this class to access face tracking features in your app via accessing the generic API.
    /// You can also extend it to provide an implementation of a provider which provides the face tracking data
    /// to the higher-level code.
    /// </remarks>
    public class XRFaceSubsystem
        : TrackingSubsystem<XRFace, XRFaceSubsystem, XRFaceSubsystemDescriptor, XRFaceSubsystem.Provider>
    {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        ValidationUtility<XRFace> m_ValidationUtility = new();
#endif

        /// <summary>
        /// Constructs a face subsystem. Do not invoke directly; call <c>Create</c> on the <see cref="XRFaceSubsystemDescriptor"/> instead.
        /// </summary>
        public XRFaceSubsystem() { }

        /// <summary>
        /// Get or set the maximum number of faces to track simultaneously.
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if the requested maximum face count is less than
        /// one. To stop face tracking, call <see cref="SubsystemWithProvider.Stop()"/>.</exception>
        /// <exception cref="System.NotSupportedException">Thrown if the requested maximum face count is greater than
        /// one but the subsystem does not support tracking multiple faces.</exception>
        public int requestedMaximumFaceCount
        {
            get => provider.requestedMaximumFaceCount;
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException("value", "Must track at least one face. Call Stop() if you wish to stop face tracking.");
                provider.requestedMaximumFaceCount = value;
            }
        }

        /// <summary>
        /// Get the maximum number of faces the provider will simultaneously track.
        /// </summary>
        public int currentMaximumFaceCount => provider.currentMaximumFaceCount;

        /// <summary>
        /// Get the number of faces the subsystem is able to track simultaneously in its current configuration.
        /// </summary>
        public int supportedFaceCount => provider.supportedFaceCount;

        /// <summary>
        /// Get the changes to faces (added, updated, and removed) since the last call to <see cref="GetChanges(Allocator)"/>.
        /// </summary>
        /// <param name="allocator">An <c>Allocator</c> to use when allocating the returned <c>NativeArray</c>s.</param>
        /// <returns>
        /// <see cref="TrackableChanges{T}"/> describing the faces that have been added, updated, and removed
        /// since the last call to <see cref="GetChanges(Allocator)"/>. The caller owns the memory allocated with <c>Allocator</c>.
        /// </returns>
        public override TrackableChanges<XRFace> GetChanges(Allocator allocator)
        {
            var changes = provider.GetChanges(XRFace.defaultValue, allocator);
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            m_ValidationUtility.ValidateAndDisposeIfThrown(changes);
#endif
            return changes;
        }

        /// <summary>
        /// Get the mesh data associated with the face with <paramref name="faceId"/>. The <paramref name="faceMesh"/>
        /// is reused if it is the correct size; otherwise, it is disposed and reallocated using <paramref name="allocator"/>.
        /// </summary>
        /// <param name="faceId">The <see cref="TrackableId"/> for a <see cref="XRFace"/>.</param>
        /// <param name="allocator">The allocator to use for the returned data if a resize is necessary. Must be <c>Allocator.TempJob</c> or <c>Allocator.Persistent</c>.</param>
        /// <param name="faceMesh">The container for the mesh data to either re-use or re-allocate.</param>
        /// <exception cref="System.InvalidOperationException">Thrown if <paramref name="allocator"/> is <c>Allocator.Temp</c></exception>
        /// <exception cref="System.InvalidOperationException">Thrown if <paramref name="allocator"/> is <c>Allocator.None</c></exception>
        public virtual void GetFaceMesh(TrackableId faceId, Allocator allocator, ref XRFaceMesh faceMesh)
        {
            if (allocator == Allocator.Temp)
                throw new InvalidOperationException("Allocator.Temp is not supported. Use Allocator.TempJob if you wish to use a temporary allocator.");

            if (allocator == Allocator.None)
                throw new InvalidOperationException("Allocator.None is not a valid allocator.");

            using (new ScopedProfiler("GetFaceMesh"))
                provider.GetFaceMesh(faceId, allocator, ref faceMesh);
        }

        /// <summary>
        /// Attempts to get the blend shapes associated with the given face.
        /// </summary>
        /// <param name="faceId">The <see cref="TrackableId"/> associated with the <see cref="XRFace"/> to query.</param>
        /// <param name="allocator">The allocator to use for the returned blend shape native array.</param>
        /// <returns>A result of a new native array, allocated with the requested allocation strategy, describing
        /// the blend shapes for the face. You own the returned native array and are responsible for calling <c>Dispose</c>
        /// on it if you have opted for the `Allocator.Persistent` strategy.</returns>
        public Result<NativeArray<XRFaceBlendShape>> TryGetBlendShapes(
   TrackableId faceId, Allocator allocator)
        {
            return provider.TryGetBlendShapes(faceId, allocator);
        }

        /// <summary>
        /// Class to be implemented by an implementor of the <see cref="XRFaceSubsystem"/>.
        /// </summary>
        public abstract class Provider : SubsystemProvider<XRFaceSubsystem>
        {
            /// <summary>
            /// Get the mesh data associated with the face with <paramref name="faceId"/>. The <paramref name="faceMesh"/>
            /// should be reused if it is the correct size; otherwise, its arrays should be reallocated with <paramref name="allocator"/>.
            /// Use <see cref="XRFaceMesh.Resize"/> to resize the containers for face mesh data.
            /// </summary>
            /// <param name="faceId">The <see cref="TrackableId"/> for a <see cref="XRFace"/>.</param>
            /// <param name="allocator">The allocator to use for the returned data if a resize is necessary.</param>
            /// <param name="faceMesh">The container for the mesh data to either re-use or re-allocate.</param>
            /// <example>
            /// <code>
            /// var vertices = faceMesh.vertices;
            /// CreateOrResizeNativeArrayIfNecessary(numVertices, allocator, ref vertices);
            ///
            /// ...
            ///
            /// faceMesh.Assign(new XRFaceMesh
            /// {
            ///     vertices = vertices,
            ///     indices = ...
            /// });
            /// </code>
            /// </example>
            public virtual void GetFaceMesh(TrackableId faceId, Allocator allocator, ref XRFaceMesh faceMesh)
            {
                faceMesh.Dispose();
                faceMesh = default;
            }

            /// <summary>
            /// Get the changes to faces (added, updated, and removed) since the last call to <see cref="GetChanges(XRFace,Allocator)"/>.
            /// </summary>
            /// <param name="defaultFace">
            /// The default face. This should be used to initialize the returned <c>NativeArray</c>s for backwards compatibility.
            /// See <see cref="TrackableChanges{T}.TrackableChanges(void*, int, void*, int, void*, int, T, int, Unity.Collections.Allocator)"/>.
            /// </param>
            /// <param name="allocator">An <c>Allocator</c> to use when allocating the returned <c>NativeArray</c>s.</param>
            /// <returns>
            /// <see cref="TrackableChanges{T}"/> describing the faces that have been added, updated, and removed
            /// since the last call to <see cref="GetChanges(XRFace,Allocator)"/>. The changes should be allocated using
            /// <paramref name="allocator"/>.
            /// </returns>
            public abstract TrackableChanges<XRFace> GetChanges(XRFace defaultFace, Allocator allocator);

            /// <summary>
            /// Attempts to get the blend shapes associated with the face with the given ID.
            /// Blend shapes describe facial features on a scale of 0..1. For example, how closed is the left eye, how open is the mouth, etc.
            /// See <see cref="XRFaceBlendShape"/> for more details.
            /// </summary>
            /// <param name="faceId">The <see cref="TrackableId"/> associated with the <see cref="XRFace"/> to query.</param>
            /// <param name="allocator">The allocator to use for the returned blend shape <c>NativeArray</c>.</param>
            /// <returns>A <see cref="Result"/> of a new <c>NativeArray</c> allocated with <paramref name="allocator"/> describing
            /// the blend shapes for the face with the given ID. The caller owns the
            /// <c>NativeArray</c> and is responsible for calling <c>Dispose</c> on it.</returns>
            public virtual Result<NativeArray<XRFaceBlendShape>> TryGetBlendShapes(
   TrackableId faceId, Allocator allocator)
            {
                throw new NotSupportedException("Blend Shapes are not supported.");
            }

            /// <summary>
            /// Should return the maximum number of faces the subsystem is able to track simultaneously.
            /// Defaults to 1.
            /// </summary>
            public virtual int supportedFaceCount => 1;

            /// <summary>
            /// Get or set the maximum number of faces the subsystem should attempt to track simultaneously.
            /// Defaults to 1.
            /// </summary>
            /// <exception cref="System.NotSupportedException">Thrown if the requested maximum face count is greater than one, but the subsystem does not support tracking multiple faces.</exception>
            public virtual int requestedMaximumFaceCount
            {
                get => m_RequestedMaximumFaceCount;
                set => m_RequestedMaximumFaceCount = value;
            }
            int m_RequestedMaximumFaceCount = 1;

            /// <summary>
            /// Gets the maximum number of faces the provider will track.
            /// </summary>
            public virtual int currentMaximumFaceCount => 1;
        }
    }
}
