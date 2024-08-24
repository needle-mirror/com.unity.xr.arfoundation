using System;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation
{
    /// <summary>
    /// Represents a tracked object in the physical environment.
    /// </summary>
    /// <remarks>
    /// Related information: <a href="xref:arfoundation-object-tracking">AR Tracked Object Manager component</a>
    /// </remarks>
    [DefaultExecutionOrder(ARUpdateOrder.k_TrackedObject)]
    [DisallowMultipleComponent]
    [HelpURL(typeof(ARTrackedObject))]
    public class ARTrackedObject : ARTrackable<XRTrackedObject, ARTrackedObject>
    {
        /// <summary>
        /// The reference object which was used to detect this object in the environment.
        /// </summary>
        public XRReferenceObject referenceObject { get; internal set; }
    }
}
