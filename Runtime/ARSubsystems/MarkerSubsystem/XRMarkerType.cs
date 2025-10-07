namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// Identifies the type or family of a detected visual marker recognized by the runtime.
    /// </summary>
    /// <remarks>
    /// <b>Note:</b> These values match the <c>XrSpatialCapabilityEXT</c> enum defined by the
    /// <a href="https://registry.khronos.org/OpenXR/specs/1.0/html/xrspec.html#XR_EXT_spatial_entity">
    /// OpenXR XR_EXT_spatial_entity extension</a>.
    /// </remarks>
    public enum XRMarkerType
    {
        /// <summary>
        /// No marker type. This value is used when marker tracking is unsupported.
        /// </summary>
        None = 0,

        /// <summary>
        /// The marker is a standard QR (Quick Response) code as defined by ISO/IEC 18004.
        /// </summary>
        QRCode = 1000743000,

        /// <summary>
        /// The marker is a Micro QR code (smaller version of QR code for encoding less data in a smaller footprint).
        /// </summary>
        MicroQRCode = 1000743001,

        /// <summary>
        /// The marker is an ArUco fiducial marker used for camera pose estimation.
        /// </summary>
        ArUco = 1000743002,

        /// <summary>
        /// The marker is an AprilTag, a type of fiducial marker often used in robotics and computer vision for localization.
        /// </summary>
        AprilTag = 1000743003
    }
}
