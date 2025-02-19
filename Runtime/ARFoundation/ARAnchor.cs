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
                    Debug.LogWarning($"{nameof(ARAnchor)} component on {name} has failed to add itself to the anchor subsystem, and will be disabled. To avoid this possibility, you should use ARAnchorManager.{nameof(ARAnchorManager.TryAddAnchorAsync)} instead of adding the AR Anchor component to GameObjects at runtime.");
                    enabled = false;
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
