namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// The result of an operation to load an anchor from persistent storage.
    /// </summary>
    public struct XRLoadAnchorResult
    {
        /// <summary>
        /// The status of the load operation. If `resultStatus.IsError()`, do not read the <see cref="xrAnchor"/> value.
        /// </summary>
        public XRResultStatus resultStatus;

        /// <summary>
        /// The persistent anchor GUID of the anchor requested to be loaded.
        /// </summary>
        public SerializableGuid savedAnchorGuid;

        /// <summary>
        /// If successfully loaded, the anchor that was loaded. Otherwise `default`.
        /// </summary>
        public XRAnchor xrAnchor;
    }
}
