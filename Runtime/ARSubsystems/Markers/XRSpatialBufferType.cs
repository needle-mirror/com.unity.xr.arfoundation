namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// Describes the type of data encoded in a spatial buffer for a detected marker.
    /// </summary>
    public enum XRSpatialBufferType
    {
        /// <summary>
        /// No encoded data is present for this marker.
        /// Used for marker types like ArUco and AprilTag, which do not encode data.
        /// </summary>
        None = 0,

        /// <summary>
        /// The spatial buffer exists, but the runtime has not yet decoded or identified the
        /// data's format or type.
        /// Use this when the buffer contents are not yet interpretable or available.
        /// </summary>
        Unknown = 1,

        /// <summary>
        /// The encoded data consists of a UTF-8 string, such as the text encoded by a QR code.
        /// </summary>
        String = 2,

        /// <summary>
        /// The encoded data consists of raw bytes, such as the binary data from a QR code.
        /// The interpretation of the bytes must be defined by the provider.
        /// </summary>
        Uint8 = 3,
    }
}
