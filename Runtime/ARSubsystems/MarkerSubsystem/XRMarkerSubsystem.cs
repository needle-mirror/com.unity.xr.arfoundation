using System;
using Unity.Collections;
using UnityEngine.SubsystemsImplementation;

namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// A subsystem that detects and tracks visual markers in the environment (e.g., QR, ArUco, AprilTag).
    /// </summary>
    /// <remarks>
    /// This base class must be implemented by providers that supply actual marker detection and tracking.
    /// </remarks>
    public class XRMarkerSubsystem :
        TrackingSubsystem<XRMarker, XRMarkerSubsystem, XRMarkerSubsystemDescriptor, XRMarkerSubsystem.Provider>
    {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        ValidationUtility<XRMarker> m_ValidationUtility = new();
#endif

        /// <summary>
        /// Do not invoke this constructor directly. Use Unity's subsystem infrastructure to acquire instances.
        /// </summary>
        public XRMarkerSubsystem() { }

        /// <summary>
        /// Gets the changes (added, updated, and removed markers) since the last call to this method.
        /// </summary>
        /// <param name="allocator">The allocator used to allocate memory for the changes.</param>
        /// <returns>The set of changes since the last call to this method.</returns>
        public override TrackableChanges<XRMarker> GetChanges(Allocator allocator)
        {
            var changes = provider.GetChanges(XRMarker.defaultValue, allocator);
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            m_ValidationUtility.ValidateAndDisposeIfThrown(changes);
#endif
            return changes;
        }

        /// <summary>
        /// Attempts to get the encoded data of a specific marker, decoded as a string.
        /// </summary>
        /// <remarks>
        /// To determine if a marker's encoded data is available as a string, you can check its
        /// <see cref="XRMarker.dataBuffer"/> property. If the <see cref="XRSpatialBuffer.bufferType"/> is
        /// <see cref="XRSpatialBufferType.String"/>, this method should be used to retrieve the encoded data.
        ///
        /// Alternatively, if you expect string data (for example, you are scanning QR codes with URLs), you can
        /// call this method directly and check the <see cref="Result{T}.status"/> to see if it was successful.
        /// </remarks>
        /// <param name="dataBuffer">The spatial buffer of the marker required to retrieve the encoded data.</param>
        /// <returns>
        /// A `Result` which contains the decoded string if successful.
        /// </returns>
        public Result<string> TryGetStringData(XRSpatialBuffer dataBuffer)
        {
            return provider.TryGetStringData(dataBuffer);
        }

        /// <summary>
        /// Attempts to get the encoded data of a specific marker, decoded as a <see cref="NativeArray{Byte}"/>.
        /// </summary>
        /// <remarks>
        /// This method is useful for performance sensitive applications as it avoids managed memory allocations.
        /// The caller is responsible for disposing the returned <see cref="NativeArray{Byte}"/> for persistent allocators.
        /// </remarks>
        /// <param name="dataBuffer">The spatial buffer of the marker required to retrieve the encoded data.</param>
        /// <param name="allocator">The allocator to use for the returned <see cref="NativeArray{Byte}"/>.</param>
        /// <returns>A `Result` containing the raw bytes in a <see cref="NativeArray{Byte}"/> if the operation is
        /// successful.</returns>
        public Result<NativeArray<byte>> TryGetBytesData(XRSpatialBuffer dataBuffer, Allocator allocator)
        {
            return provider.TryGetBytesData(dataBuffer, allocator);
        }

        /// <summary>
        /// Attempts to get the encoded data of a specific marker, decoded as a byte array.
        /// </summary>
        /// <remarks>
        /// To determine if a marker's encoded data is available as binary data, you can check its
        /// <see cref="XRMarker.dataBuffer"/> property. If the <see cref="XRSpatialBuffer.bufferType"/> is
        /// <see cref="XRSpatialBufferType.Uint8"/>, this method should be used to retrieve the encoded data.
        ///
        /// This method allocates a new managed `byte[]` on each successful call. For performance critical applications
        /// that need to avoid garbage collection, consider using
        /// (<see cref="TryGetBytesData(XRSpatialBuffer, Allocator)"/>).
        /// </remarks>
        /// <param name="dataBuffer">The spatial buffer of the marker required to retrieve the encoded data.</param>
        /// <returns>
        /// A `Result` which contains the raw byte array if successful.
        /// </returns>
        public Result<byte[]> TryGetBytesData(XRSpatialBuffer dataBuffer)
        {
            return provider.TryGetBytesData(dataBuffer);
        }

        /// <summary>
        /// The provider interface to be implemented by XR platform plugin packages.
        /// </summary>
        public abstract class Provider : SubsystemProvider<XRMarkerSubsystem>
        {
            /// <summary>
            /// Gets the changes (added, updated, and removed markers) since the last call.
            /// </summary>
            /// <param name="defaultMarker">The default marker instance (used for memory layout).</param>
            /// <param name="allocator">The allocator used for the returned data.</param>
            /// <returns>A <see cref="TrackableChanges{XRMarker}"/> describing changes since the last call.</returns>
            public abstract TrackableChanges<XRMarker> GetChanges(XRMarker defaultMarker, Allocator allocator);

            /// <summary>
            /// Attempts to get the encoded data of a specific marker, decoded as a string.
            /// </summary>
            /// <remarks>
            /// To determine if a marker's encoded data is available as a string, you can check its
            /// <see cref="XRMarker.dataBuffer"/> property. If the <see cref="XRSpatialBuffer.bufferType"/> is
            /// <see cref="XRSpatialBufferType.String"/>, this method should be used to retrieve the encoded data.
            ///
            /// Alternatively, if you expect string data (for example, you are scanning QR codes with URLs), you can
            /// call this method directly and check the <see cref="Result{T}.status"/> to see if it was successful.
            /// </remarks>
            /// <param name="dataBuffer">The spatial buffer of the marker required to retrieve the encoded data.</param>
            /// <returns>
            /// A `Result` which contains the decoded string if successful.
            /// </returns>
            public abstract Result<string> TryGetStringData(XRSpatialBuffer dataBuffer);

            /// <summary>
            /// Attempts to get the encoded data of a specific marker, decoded as a <see cref="NativeArray{Byte}"/>.
            /// </summary>
            /// <remarks>
            /// This method is useful for performance sensitive applications as it avoids managed memory allocations.
            /// The caller is responsible for disposing the returned <see cref="NativeArray{Byte}"/> for persistent allocators.
            /// </remarks>
            /// <param name="dataBuffer">The spatial buffer of the marker required to retrieve the encoded data.</param>
            /// <param name="allocator">The allocator to use for the returned <see cref="NativeArray{Byte}"/>.</param>
            /// <returns>A `Result` containing the raw bytes in a <see cref="NativeArray{Byte}"/> if the operation is
            /// successful.</returns>
            public abstract Result<NativeArray<byte>> TryGetBytesData(XRSpatialBuffer dataBuffer, Allocator allocator);

            /// <summary>
            /// Attempts to get the encoded data of a specific marker, decoded as a byte array.
            /// </summary>
            /// <remarks>
            /// To determine if a marker's encoded data is available as binary data, you can check its
            /// <see cref="XRMarker.dataBuffer"/> property. If the <see cref="XRSpatialBuffer.bufferType"/> is
            /// <see cref="XRSpatialBufferType.Uint8"/>, this method should be used to retrieve the encoded data.
            ///
            /// This method allocates a new managed `byte[]` on each successful call. For performance critical applications
            /// that need to avoid garbage collection, consider using
            /// (<see cref="TryGetBytesData(XRSpatialBuffer, Allocator)"/>) directly.
            /// </remarks>
            /// <param name="dataBuffer">The spatial buffer of the marker required to retrieve the encoded data.</param>
            /// <returns>
            /// A `Result` which contains the raw byte array if successful.
            /// </returns>
            public abstract Result<byte[]> TryGetBytesData(XRSpatialBuffer dataBuffer);
        }
    }
}
