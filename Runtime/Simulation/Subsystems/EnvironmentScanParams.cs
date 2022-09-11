using System;

namespace UnityEngine.XR.Simulation
{
    [Serializable]
    class EnvironmentScanParams
    {
        [SerializeField]
        [Tooltip("The time in seconds between two environment scans.")]
        float m_MinimumRescanTime = 0.15f;

        [SerializeField]
        [Tooltip("The minimum distance in meters a camera should move before the next environment scan.")]
        float m_DeltaCameraDistanceToRescan = 0.025f;

        [SerializeField]
        [Tooltip("Minimum change of angle in degrees of the camera before the next environment scan.")]
        float m_DeltaCameraAngleToRescan = 4f;

        [SerializeField]
        [Tooltip("Total number of rays to project per scan.")]
        int m_RaysPerCast = 10;

        [SerializeField]
        [Tooltip("Maximum distance in meters from the camera after which the points will not be detected.")]
        float m_MaximumHitDistance = 2f;

        [SerializeField]
        [Tooltip("The points will not be detected between camera and this distance in meters.")]
        float m_MinimumHitDistance = 0.05f;

        public float minimumRescanTime
        {
            get => m_MinimumRescanTime;
            set => m_MinimumRescanTime = value;
        }

        public float deltaCameraDistanceToRescan
        {
            get => m_DeltaCameraDistanceToRescan;
            set => m_DeltaCameraDistanceToRescan = value;
        }

        public float deltaCameraAngleToRescan
        {
            get => m_DeltaCameraAngleToRescan;
            set => m_DeltaCameraAngleToRescan = value;
        }

        public int raysPerCast
        {
            get => m_RaysPerCast;
            set => m_RaysPerCast = value;
        }

        public float maximumHitDistance
        {
            get => m_MaximumHitDistance;
            set => m_MaximumHitDistance = value;
        }

        public float minimumHitDistance
        {
            get => m_MinimumHitDistance;
            set => m_MinimumHitDistance = value;
        }
    }
}
