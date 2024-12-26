namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// The result of an operation to erase an anchor from persistent storage.
    /// </summary>
    public struct XREraseAnchorResult
    {
        /// <summary>
        /// The status of the erase operation. If `resultStatus.IsError()`, the anchor was not erased.
        /// </summary>
        public XRResultStatus resultStatus;

        /// <summary>
        /// The persistent anchor GUID requested to be erased.
        /// </summary>
        public SerializableGuid savedAnchorGuid;
    }
}
