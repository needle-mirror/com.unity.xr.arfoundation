using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.Simulation
{
    /// <summary>
    /// Provides camera specifications and simulated EXIF data for a Camera GameObject for XR Simulation.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    [DisallowMultipleComponent]
    [AddComponentMenu("")]
    public class SimulatedExifData : MonoBehaviour
    {
        /// <summary>
        /// The lens aperture.
        /// </summary>
        [SerializeField]
        [Tooltip("The lens aperture. Default value comes from sample ARKit data.")]
        double m_ApertureValue = 1.356;

        /// <summary>
        /// The exposure bias.
        /// </summary>
        [SerializeField]
        [Tooltip("The exposure bias. Default value comes from sample ARKit data.")]
        float m_ExposureBiasValue = 0.0f;

        /// <summary>
        /// The lens focal length in millimeters.
        /// </summary>
        [SerializeField]
        [Tooltip("The lens focal length in millimeters. Default value comes from sample ARKit data.")]
        float m_FocalLength = 5.96f;

        /// <summary>
        /// The metering mode.
        /// </summary>
        [SerializeField]
        [Tooltip("The metering mode. Default value comes from sample ARKit data.")]
        XRCameraFrameExifDataMeteringMode m_MeteringMode = XRCameraFrameExifDataMeteringMode.Pattern;

        /// <summary>
        /// The photographic sensitivity (ISO).
        /// </summary>
        [SerializeField]
        [Tooltip("The photographic sensitivity (ISO). Default value comes from sample ARKit data.")]
        short m_PhotographicSensitivity = 400;

        /// <summary>
        /// Get the lens aperture.
        /// </summary>
        /// <value>The lens aperture.</value>
        public double apertureValue
        {
            get => m_ApertureValue;
            set => m_ApertureValue = value;
        }

        /// <summary>
        /// Get the exposure bias.
        /// </summary>
        /// <value>The exposure bias.</value>
        public float exposureBiasValue
        {
            get => m_ExposureBiasValue;
            set => m_ExposureBiasValue = value;
        }

        /// <summary>
        /// Get the lens focal length in millimeters.
        /// </summary>
        /// <value>The lens focal length in millimeters.</value>
        public float focalLength
        {
            get => m_FocalLength;
            set => m_FocalLength = value;
        }

        /// <summary>
        /// Get the metering mode.
        /// </summary>
        /// <value>The metering mode.</value>
        public XRCameraFrameExifDataMeteringMode meteringMode
        {
            get => m_MeteringMode;
            set => m_MeteringMode = value;
        }

        /// <summary>
        /// Get the photographic sensitivity (ISO).
        /// </summary>
        /// <value>The photographic sensitivity (ISO).</value>
        public short photographicSensitivity
        {
            get => m_PhotographicSensitivity;
            set => m_PhotographicSensitivity = value;
        }
    }
}
