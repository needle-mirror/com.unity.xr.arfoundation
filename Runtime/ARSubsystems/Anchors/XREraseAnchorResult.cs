using static UnityEngine.XR.ARSubsystems.XRResultStatus;

namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// The result of an operation to erase an anchor from persistent storage.
    /// </summary>
    public struct XREraseAnchorResult
    {
        /// <summary>
        /// The status of the completed erase operation. If `resultStatus.IsError()`, the anchor wasn't erased.
        /// </summary>
        public XRResultStatus resultStatus;

        /// <summary>
        /// The persistent anchor GUID requested to be erased.
        /// </summary>
        public SerializableGuid savedAnchorGuid;

        /// <summary>
        /// Construct an instance.
        /// </summary>
        /// <param name="resultStatus">The status of the completed erase operation.</param>
        /// <param name="savedAnchorGuid">The persistent anchor GUID requested to be erased.</param>
        public XREraseAnchorResult(XRResultStatus resultStatus, SerializableGuid savedAnchorGuid)
        {
            this.resultStatus = resultStatus;
            this.savedAnchorGuid = savedAnchorGuid;
        }

        /// <summary>
        /// Get a default instance, initialized with <see cref="StatusCode.UnknownError">StatusCode.UnknownError</see>.
        /// </summary>
        public static XREraseAnchorResult defaultValue =>
            new(new XRResultStatus(StatusCode.UnknownError), SerializableGuid.empty);
    }
}
