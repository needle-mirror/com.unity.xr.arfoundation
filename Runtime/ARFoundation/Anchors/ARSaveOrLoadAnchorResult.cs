using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation
{
    /// <summary>
    /// Represents the results, per anchor, of a save or load operation involving a batch of anchors.
    /// </summary>
    public struct ARSaveOrLoadAnchorResult
    {
        /// <summary>
        /// The status of the operation. Do not read the output if `resultStatus.IsError()`.
        /// </summary>
        public XRResultStatus resultStatus;

        /// <summary>
        /// The persistent anchor GUID, used as output for the save operation and input for the load operation.
        /// </summary>
        public SerializableGuid savedAnchorGuid;

        /// <summary>
        /// The anchor, used as input for the save operation and output for the load operation.
        /// </summary>
        public ARAnchor anchor;
    }
}
