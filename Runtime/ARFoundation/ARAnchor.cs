using System;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation
{
    /// <summary>
    /// Represents an Anchor tracked by an XR device.
    /// </summary>
    /// <remarks>
    /// An anchor is a pose in the physical environment that is tracked by an XR device.
    /// As the device refines its understanding of the environment, anchors will be
    /// updated, helping you to keep virtual content connected to a real-world position and orientation.
    /// </remarks>
    [DefaultExecutionOrder(ARUpdateOrder.k_Anchor)]
    [DisallowMultipleComponent]
    [HelpURL(typeof(ARAnchor))]
    public sealed class ARAnchor : ARTrackable<XRAnchor, ARAnchor>
    {
        /// <summary>
        /// Get the session identifier from which this anchor originated.
        /// </summary>
        public Guid sessionId => sessionRelativeData.sessionId;

        void OnEnable()
        {
            if (ARAnchorManager.instance is ARAnchorManager manager)
            {
                if (sessionRelativeData.trackableId == TrackableId.invalidId && !manager.TryAddAnchor(this))
                {
                    gameObject.SetActive(false);
                }
            }
            else
            {
                pending = true;
            }
        }

        void OnDisable()
        {
            if (ARAnchorManager.instance is ARAnchorManager manager)
            {
                manager.TryRemoveAnchor(this);
            }
        }
    }
}
