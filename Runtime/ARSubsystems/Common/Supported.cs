namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// Represents whether a capability is supported.
    /// </summary>
    public enum Supported
    {
        /// <summary>
        /// Support is unknown. This could be because support is still being determined.
        /// </summary>
        Unknown,

        /// <summary>
        /// The capability is not supported.
        /// </summary>
        Unsupported,

        /// <summary>
        /// The capability is supported.
        /// </summary>
        Supported,
    }

    /// <summary>
    /// Utility class for working with the <see cref="Supported"/> enum.
    /// </summary>
    public static class SupportedUtils
    {
        /// <summary>
        /// Create a `Supported` instance from a `bool`, assuming that the support status is known.
        /// </summary>
        /// <param name="isSupported">Indicates whether a capability is supported.</param>
        /// <returns>`Supported` if <paramref name="isSupported"/> is <see langword="true"/>. Otherwise, `Unsupported`.</returns>
        public static Supported FromBool(bool isSupported)
        {
            return isSupported switch
            {
                false => Supported.Unsupported,
                true => Supported.Supported
            };
        }
    }
}
