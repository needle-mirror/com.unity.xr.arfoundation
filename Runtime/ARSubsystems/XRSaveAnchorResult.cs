namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// The result of an operation to save an anchor to persistent storage.
    /// </summary>
    public struct XRSaveAnchorResult
    {
        /// <summary>
        /// The status of save operation. If `resultStatus.IsError()`, do not read the <see cref="savedAnchorGuid"/> value.
        /// </summary>
        public XRResultStatus resultStatus;

        /// <summary>
        /// The `TrackableId` of the anchor requested to be saved.
        /// </summary>
        public TrackableId trackableId;

        /// <summary>
        /// If successfully saved, the anchor's persistent anchor GUID. Otherwise, `default`.
        /// </summary>
        public SerializableGuid savedAnchorGuid;
    }
}
