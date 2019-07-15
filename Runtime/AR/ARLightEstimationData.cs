using System;

namespace UnityEngine.XR.ARFoundation
{
    /// <summary>
    /// A structure for light estimation information provided by the AR device.
    /// </summary>
    public struct ARLightEstimationData : IEquatable<ARLightEstimationData>
    {
        /// <summary>
        /// An estimate for the average brightness in the scene.
        /// Use <c>averageBrightness.HasValue</c> to determine if this information is available.
        /// </summary>
        /// <remarks>
        /// <see cref="averageBrightness"/> may be null when light estimation is not enabled in the <see cref="ARSession"/>,
        /// or if a platform-specific error has occurred.
        /// </remarks>
        public float? averageBrightness
        {
            get
            {
                if (m_AverageBrightness.HasValue)
                    return m_AverageBrightness;
                else if (m_AverageIntensityInLumens.HasValue)
                    return ConvertLumensToBrightness(m_AverageIntensityInLumens.Value);

                return null;
            }
            set { m_AverageBrightness = value; }
        }

        /// <summary>
        /// An estimate for the average color temperature of the scene.
        /// Use <c>averageColorTemperature.HasValue</c> to determine if this information is available.
        /// </summary>
        /// <remarks>
        /// <see cref="averageColorTemperature"/> may be null when light estimation is not enabled in the <see cref="ARSession"/>,
        /// if the platform does not support it, or if a platform-specific error has occurred.
        /// </remarks>
        public float? averageColorTemperature { get; set; }

        /// <summary>
        /// The scaling factors used for color correction.
        /// The RGB scale factors are used to match the color of the light
        /// in the scene. The alpha channel value is platform-specific.
        /// </summary>
        /// <remarks>
        /// <see cref="colorCorrection"/> may be null when light estimation is not enabled in the <see cref="ARSession"/>,
        /// if the platform does not support it, or if a platform-specific error has occurred.
        /// </remarks>
        public Color? colorCorrection { get; set; }

        /// <summary>
        /// An estimate for the average intensity, in lumens, in the scene.
        /// Use <c>averageIntensityInLumens.HasValue</c> to determine if this information is available.
        /// </summary>
        /// <remarks>
        /// <see cref="averageIntensityInLumens"/> may be null when light estimation is not enabled in the <see cref="ARSession"/>,
        ///  or if a platform-specific error has occurred.
        /// </remarks>
        public float? averageIntensityInLumens
        {
            get
            {
                if (m_AverageIntensityInLumens.HasValue)
                    return m_AverageIntensityInLumens;
                else if (m_AverageBrightness.HasValue)
                    return ConvertBrightnessToLumens(m_AverageBrightness.Value);

                return null;
            }
            set { m_AverageIntensityInLumens = value; }
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return
                    ((averageBrightness.GetHashCode() * 486187739 +
                    averageColorTemperature.GetHashCode()) * 486187739 +
                    colorCorrection.GetHashCode()) * 486187739 +
                    averageIntensityInLumens.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ARLightEstimationData))
                return false;

            return Equals((ARLightEstimationData)obj);
        }

        public override string ToString()
        {
            return string.Format("(Avg. Brightness: {0}, Avg. Color Temperature: {1}, Color Correction: {2}, Avg. Intensity in Lumens: {3})",
                averageBrightness, averageColorTemperature, colorCorrection, averageIntensityInLumens);
        }

        public bool Equals(ARLightEstimationData other)
        {
            return
                (averageBrightness.Equals(other.averageBrightness)) &&
                (averageColorTemperature.Equals(other.averageColorTemperature)) &&
                (colorCorrection.Equals(other.colorCorrection)) &&
                (averageIntensityInLumens.Equals(other.averageIntensityInLumens)) ;
        }

        public static bool operator ==(ARLightEstimationData lhs, ARLightEstimationData rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(ARLightEstimationData lhs, ARLightEstimationData rhs)
        {
            return !lhs.Equals(rhs);
        }

        float ConvertBrightnessToLumens(float brightness)
        {
            return Mathf.Clamp(brightness*k_MaxLuminosity, 0f, k_MaxLuminosity);
        }

        float ConvertLumensToBrightness(float lumens)
        {
            return Mathf.Clamp(lumens/k_MaxLuminosity, 0f, 1f);
        }

        private float? m_AverageBrightness;
        private float? m_AverageIntensityInLumens;
        const float k_MaxLuminosity = 2000.0f;
    }
}
