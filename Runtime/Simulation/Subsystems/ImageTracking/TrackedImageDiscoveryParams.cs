using System;

namespace UnityEngine.XR.Simulation
{
    /// <summary>
    /// User-accessible settings for the <see cref="SimulationImageTrackingSubsystem"/>.
    /// </summary>
    [Serializable]
    class TrackedImageDiscoveryParams
    {
        [SerializeField, Tooltip("The time in seconds between two tracking updates.")]
        float m_TrackingUpdateInterval = 0.1f;

        public float trackingUpdateInterval => m_TrackingUpdateInterval;
    }
}
