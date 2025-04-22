using static UnityEngine.XR.ARSubsystems.XRResultStatus;

namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// The result of an operation to save an anchor to persistent storage.
    /// </summary>
    public struct XRSaveAnchorResult
    {
        /// <summary>
        /// The status of the completed save operation.
        /// If `resultStatus.IsError()`, don't read the <see cref="savedAnchorGuid"/> value.
        /// </summary>
        public XRResultStatus resultStatus;

        /// <summary>
        /// The `TrackableId` of the anchor requested to be saved.
        /// </summary>
        public TrackableId trackableId;

        /// <summary>
        /// If `resultStatus.IsSuccess()`, the anchor's persistent anchor GUID.
        /// Otherwise, <see cref="SerializableGuid.empty">SerializableGuid.empty</see>.
        /// </summary>
        public SerializableGuid savedAnchorGuid;

        /// <summary>
        /// Construct an instance.
        /// </summary>
        /// <param name="resultStatus">The status of the completed save operation.</param>
        /// <param name="trackableId">The `TrackableId` of the anchor requested to be saved.</param>
        /// <param name="savedAnchorGuid">If `resultStatus.IsSuccess()`, the anchor's persistent anchor GUID.
        /// Otherwise, <see cref="SerializableGuid.empty">SerializableGuid.empty</see>.</param>
        public XRSaveAnchorResult(XRResultStatus resultStatus, TrackableId trackableId, SerializableGuid savedAnchorGuid)
        {
            this.resultStatus = resultStatus;
            this.trackableId = trackableId;
            this.savedAnchorGuid = savedAnchorGuid;
        }

        /// <summary>
        /// Get a default instance, initialized with <see cref="StatusCode.UnknownError">StatusCode.UnknownError</see>.
        /// </summary>
        public static XRSaveAnchorResult defaultValue =>
            new(new XRResultStatus(StatusCode.UnknownError), TrackableId.invalidId, SerializableGuid.empty);
    }
}
