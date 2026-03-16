using static UnityEngine.XR.ARSubsystems.XRResultStatus;

namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// The result of an operation to load an anchor from persistent storage.
    /// </summary>
    public struct XRLoadAnchorResult
    {
        /// <summary>
        /// The status of the completed load operation.
        /// If `resultStatus.IsError()`, don't read the <see cref="xrAnchor"/> value.
        /// </summary>
        public XRResultStatus resultStatus;

        /// <summary>
        /// The persistent anchor GUID of the anchor requested to be loaded.
        /// </summary>
        public SerializableGuid savedAnchorGuid;

        /// <summary>
        /// If `resultStatus.IsSuccess()`, the anchor that was loaded.
        /// Otherwise, <see cref="XRAnchor.defaultValue">XRAnchor.defaultValue</see>.
        /// </summary>
        public XRAnchor xrAnchor;

        /// <summary>
        /// Construct an instance.
        /// </summary>
        /// <param name="resultStatus">The status of the load operation.</param>
        /// <param name="savedAnchorGuid">The persistent anchor GUID of the anchor requested to be loaded.</param>
        /// <param name="xrAnchor">If `resultStatus.IsSuccess()`, the anchor that was loaded.
        /// Otherwise, <see cref="XRAnchor.defaultValue">XRAnchor.defaultValue</see>.</param>
        public XRLoadAnchorResult(XRResultStatus resultStatus, SerializableGuid savedAnchorGuid, XRAnchor xrAnchor)
        {
            this.resultStatus = resultStatus;
            this.savedAnchorGuid = savedAnchorGuid;
            this.xrAnchor = xrAnchor;
        }

        /// <summary>
        /// Get a default instance, initialized with <see cref="StatusCode.UnknownError">StatusCode.UnknownError</see>.
        /// </summary>
        public static XRLoadAnchorResult defaultValue =>
            new(new XRResultStatus(StatusCode.UnknownError), SerializableGuid.empty, XRAnchor.defaultValue);
    }
}
