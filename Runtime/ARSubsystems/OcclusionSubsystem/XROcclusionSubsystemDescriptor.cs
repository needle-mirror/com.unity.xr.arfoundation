using System;
using UnityEngine.SubsystemsImplementation;

namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// Constructor parameters for the <see cref="XROcclusionSubsystemDescriptor"/>.
    /// </summary>
    [Obsolete("XROcclusionSubsystemCinfo has been deprecated in AR Foundation version 6.0. Use XROcclusionSubsystemDescriptor.Cinfo instead (UnityUpgradable) -> UnityEngine.XR.ARSubsystems.XROcclusionSubsystemDescriptor/Cinfo", false)]
    public struct XROcclusionSubsystemCinfo : IEquatable<XROcclusionSubsystemCinfo>
    {
        /// <summary>
        /// Specifies an identifier for the provider implementation of the subsystem.
        /// </summary>
        /// <value>
        /// The identifier for the provider implementation of the subsystem.
        /// </value>
        public string id { get; set; }

        /// <summary>
        /// Specifies the provider implementation type to use for instantiation.
        /// </summary>
        /// <value>
        /// The provider implementation type to use for instantiation.
        /// </value>
        public Type providerType { get; set; }

        /// <summary>
        /// Specifies the <c>XRAnchorSubsystem</c>-derived type that forwards casted calls to its provider.
        /// </summary>
        /// <value>
        /// The type of the subsystem to use for instantiation. If null, <c>XRAnchorSubsystem</c> will be instantiated.
        /// </value>
        public Type subsystemTypeOverride { get; set; }

        /// <summary>
        /// Specifies whether a subsystem supports human segmentation stencil image.
        /// </summary>
        public Func<Supported> humanSegmentationStencilImageSupportedDelegate { get; set; }

        /// <summary>
        /// Specifies whether a subsystem supports human segmentation depth image.
        /// </summary>
        public Func<Supported> humanSegmentationDepthImageSupportedDelegate { get; set; }

        /// <summary>
        /// Specifies whether a subsystem supports temporal smoothing of the environment depth image.
        /// </summary>
        /// <value>A method delegate indicating support for temporal smoothing of the environment depth image.</value>
        public Func<Supported> environmentDepthTemporalSmoothingSupportedDelegate { get; set; }

        /// <summary>
        /// Query for whether the current subsystem supports environment depth image.
        /// </summary>
        public Func<Supported> environmentDepthImageSupportedDelegate { get; set; }

        /// <summary>
        /// Specifies if the current subsystem supports environment depth confidence image.
        /// </summary>
        public Func<Supported> environmentDepthConfidenceImageSupportedDelegate { get; set; }

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <param name="other">The other <see cref="XROcclusionSubsystemCinfo"/> to compare against.</param>
        /// <returns>`True` if every field in <paramref name="other"/> is equal to this <see cref="XROcclusionSubsystemCinfo"/>, otherwise false.</returns>
        public bool Equals(XROcclusionSubsystemCinfo other)
        {
            return
                ReferenceEquals(id, other.id)
                && ReferenceEquals(providerType, other.providerType)
                && ReferenceEquals(subsystemTypeOverride, other.subsystemTypeOverride)
                && humanSegmentationDepthImageSupportedDelegate == other.humanSegmentationDepthImageSupportedDelegate
                && humanSegmentationStencilImageSupportedDelegate == other.humanSegmentationStencilImageSupportedDelegate
                && environmentDepthImageSupportedDelegate == other.environmentDepthImageSupportedDelegate
                && environmentDepthTemporalSmoothingSupportedDelegate == other.environmentDepthTemporalSmoothingSupportedDelegate
                && environmentDepthConfidenceImageSupportedDelegate == other.environmentDepthConfidenceImageSupportedDelegate;
        }

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <param name="obj">The `object` to compare against.</param>
        /// <returns>`True` if <paramref name="obj"/> is of type <see cref="XROcclusionSubsystemCinfo"/> and
        /// <see cref="Equals(XROcclusionSubsystemCinfo)"/> also returns `true`; otherwise `false`.</returns>
        public override bool Equals(System.Object obj) => ((obj is XROcclusionSubsystemCinfo) && Equals((XROcclusionSubsystemCinfo)obj));

        /// <summary>
        /// Tests for equality. Same as <see cref="Equals(XROcclusionSubsystemCinfo)"/>.
        /// </summary>
        /// <param name="lhs">The left-hand side of the comparison.</param>
        /// <param name="rhs">The right-hand side of the comparison.</param>
        /// <returns>`True` if <paramref name="lhs"/> is equal to <paramref name="rhs"/>, otherwise `false`.</returns>
        public static bool operator ==(XROcclusionSubsystemCinfo lhs, XROcclusionSubsystemCinfo rhs) => lhs.Equals(rhs);

        /// <summary>
        /// Tests for inequality. Same as `!`<see cref="Equals(XROcclusionSubsystemCinfo)"/>.
        /// </summary>
        /// <param name="lhs">The left-hand side of the comparison.</param>
        /// <param name="rhs">The right-hand side of the comparison.</param>
        /// <returns>`True` if <paramref name="lhs"/> is not equal to <paramref name="rhs"/>, otherwise `false`.</returns>
        public static bool operator !=(XROcclusionSubsystemCinfo lhs, XROcclusionSubsystemCinfo rhs) => !lhs.Equals(rhs);

        /// <summary>
        /// Generates a hash suitable for use with containers like `HashSet` and `Dictionary`.
        /// </summary>
        /// <returns>A hash code generated from this object's fields.</returns>
        public override int GetHashCode()
        {
            int hashCode = 486187739;
            unchecked
            {
                hashCode = (hashCode * 486187739) + HashCodeUtil.ReferenceHash(id);
                hashCode = (hashCode * 486187739) + HashCodeUtil.ReferenceHash(providerType);
                hashCode = (hashCode * 486187739) + HashCodeUtil.ReferenceHash(subsystemTypeOverride);
                hashCode = HashCodeUtil.Combine(hashCode,
                    HashCodeUtil.ReferenceHash(humanSegmentationStencilImageSupportedDelegate),
                    HashCodeUtil.ReferenceHash(humanSegmentationDepthImageSupportedDelegate),
                    HashCodeUtil.ReferenceHash(environmentDepthImageSupportedDelegate),
                    HashCodeUtil.ReferenceHash(environmentDepthTemporalSmoothingSupportedDelegate),
                    HashCodeUtil.ReferenceHash(environmentDepthConfidenceImageSupportedDelegate));
            }
            return hashCode;
        }
    }

    /// <summary>
    /// Descriptor for the XROcclusionSubsystem.
    /// </summary>
    public class XROcclusionSubsystemDescriptor :
        SubsystemDescriptorWithProvider<XROcclusionSubsystem, XROcclusionSubsystem.Provider>
    {
        /// <summary>
        /// Query for whether environment depth is supported.
        /// </summary>
        Func<Supported> m_EnvironmentDepthImageSupportedDelegate;

        /// <summary>
        /// Query for whether environment depth confidence is supported.
        /// </summary>
        Func<Supported> m_EnvironmentDepthConfidenceImageSupportedDelegate;

        bool m_SupportsHumanSegmentationStencilImage;

        /// <summary>
        /// (Read Only) Whether a subsystem supports human segmentation stencil image.
        /// </summary>
        public Supported humanSegmentationStencilImageSupported
        {
            get
            {
                if (m_HumanSegmentationStencilImageSupportedDelegate != null)
                {
                    return m_HumanSegmentationStencilImageSupportedDelegate();
                }

                return m_SupportsHumanSegmentationStencilImage ? Supported.Supported : Supported.Unsupported;
            }
        }
        Func<Supported> m_HumanSegmentationStencilImageSupportedDelegate;

        bool m_SupportsHumanSegmentationDepthImage;

        /// <summary>
        /// (Read Only) Whether a subsystem supports human segmentation depth image.
        /// </summary>
        public Supported humanSegmentationDepthImageSupported
        {
            get
            {
                if (m_HumanSegmentationDepthImageSupportedDelegate != null)
                {
                    return m_HumanSegmentationDepthImageSupportedDelegate();
                }

                return m_SupportsHumanSegmentationDepthImage ? Supported.Supported : Supported.Unsupported;
            }
        }

        Func<Supported> m_HumanSegmentationDepthImageSupportedDelegate;

        /// <summary>
        /// (Read Only) Whether the subsystem supports environment depth image.
        /// </summary>
        /// <remarks>
        /// The supported status might take time to determine. If support is still being determined, the value will be <see cref="Supported.Unknown"/>.
        /// </remarks>
        public Supported environmentDepthImageSupported
        {
            get
            {
                if (m_EnvironmentDepthImageSupportedDelegate != null)
                {
                    return m_EnvironmentDepthImageSupportedDelegate();
                }

                return Supported.Unsupported;
            }
        }

        Func<Supported> m_EnvironmentDepthTemporalSmoothingSupportedDelegate;

        /// <summary>
        /// Whether temporal smoothing of the environment image is supported.
        /// </summary>
        /// <value>Read Only.</value>
        public Supported environmentDepthTemporalSmoothingSupported =>
            m_EnvironmentDepthTemporalSmoothingSupportedDelegate?.Invoke() ?? Supported.Unsupported;

        /// <summary>
        /// (Read Only) Whether the subsystem supports environment depth confidence image.
        /// </summary>
        /// <remarks>
        /// The supported status might take time to determine. If support is still being determined, the value will be <see cref="Supported.Unknown"/>.
        /// </remarks>
        public Supported environmentDepthConfidenceImageSupported
        {
            get
            {
                if (m_EnvironmentDepthConfidenceImageSupportedDelegate != null)
                {
                    return m_EnvironmentDepthConfidenceImageSupportedDelegate();
                }

                return Supported.Unsupported;
            }
        }

        /// <summary>
        /// Constructor parameters for the <see cref="XROcclusionSubsystemDescriptor"/>.
        /// </summary>
        public struct Cinfo : IEquatable<Cinfo>
        {
            /// <summary>
            /// Specifies an identifier for the provider implementation of the subsystem.
            /// </summary>
            /// <value>
            /// The identifier for the provider implementation of the subsystem.
            /// </value>
            public string id { get; set; }

            /// <summary>
            /// Specifies the provider implementation type to use for instantiation.
            /// </summary>
            /// <value>
            /// The provider implementation type to use for instantiation.
            /// </value>
            public Type providerType { get; set; }

            /// <summary>
            /// Specifies the <c>XRAnchorSubsystem</c>-derived type that forwards casted calls to its provider.
            /// </summary>
            /// <value>
            /// The type of the subsystem to use for instantiation. If null, <c>XRAnchorSubsystem</c> will be instantiated.
            /// </value>
            public Type subsystemTypeOverride { get; set; }

            /// <summary>
            /// Specifies whether a subsystem supports human segmentation stencil image.
            /// </summary>
            public Func<Supported> humanSegmentationStencilImageSupportedDelegate { get; set; }

            /// <summary>
            /// Specifies whether a subsystem supports human segmentation depth image.
            /// </summary>
            public Func<Supported> humanSegmentationDepthImageSupportedDelegate { get; set; }

            /// <summary>
            /// Specifies whether a subsystem supports temporal smoothing of the environment depth image.
            /// </summary>
            /// <value>A method delegate indicating support for temporal smoothing of the environment depth image.</value>
            public Func<Supported> environmentDepthTemporalSmoothingSupportedDelegate { get; set; }

            /// <summary>
            /// Query for whether the current subsystem supports environment depth image.
            /// </summary>
            public Func<Supported> environmentDepthImageSupportedDelegate { get; set; }

            /// <summary>
            /// Specifies if the current subsystem supports environment depth confidence image.
            /// </summary>
            public Func<Supported> environmentDepthConfidenceImageSupportedDelegate { get; set; }

            /// <summary>
            /// Tests for equality.
            /// </summary>
            /// <param name="other">The other <see cref="Cinfo"/> to compare against.</param>
            /// <returns>`True` if every field in <paramref name="other"/> is equal to this <see cref="Cinfo"/>, otherwise false.</returns>
            public bool Equals(Cinfo other)
            {
                return
                    ReferenceEquals(id, other.id) &&
                    ReferenceEquals(providerType, other.providerType) &&
                    ReferenceEquals(subsystemTypeOverride, other.subsystemTypeOverride) &&
                    humanSegmentationDepthImageSupportedDelegate == other.humanSegmentationDepthImageSupportedDelegate &&
                    humanSegmentationStencilImageSupportedDelegate == other.humanSegmentationStencilImageSupportedDelegate &&
                    environmentDepthImageSupportedDelegate == other.environmentDepthImageSupportedDelegate &&
                    environmentDepthTemporalSmoothingSupportedDelegate == other.environmentDepthTemporalSmoothingSupportedDelegate &&
                    environmentDepthConfidenceImageSupportedDelegate == other.environmentDepthConfidenceImageSupportedDelegate;
            }

            /// <summary>
            /// Tests for equality.
            /// </summary>
            /// <param name="obj">The `object` to compare against.</param>
            /// <returns>`True` if <paramref name="obj"/> is of type <see cref="Cinfo"/> and
            /// <see cref="Equals(Cinfo)"/> also returns `true`; otherwise `false`.</returns>
            public override bool Equals(System.Object obj) => ((obj is Cinfo) && Equals((Cinfo)obj));

            /// <summary>
            /// Tests for equality. Same as <see cref="Equals(Cinfo)"/>.
            /// </summary>
            /// <param name="lhs">The left-hand side of the comparison.</param>
            /// <param name="rhs">The right-hand side of the comparison.</param>
            /// <returns>`True` if <paramref name="lhs"/> is equal to <paramref name="rhs"/>, otherwise `false`.</returns>
            public static bool operator ==(Cinfo lhs, Cinfo rhs) => lhs.Equals(rhs);

            /// <summary>
            /// Tests for inequality. Same as `!`<see cref="Equals(Cinfo)"/>.
            /// </summary>
            /// <param name="lhs">The left-hand side of the comparison.</param>
            /// <param name="rhs">The right-hand side of the comparison.</param>
            /// <returns>`True` if <paramref name="lhs"/> is not equal to <paramref name="rhs"/>, otherwise `false`.</returns>
            public static bool operator !=(Cinfo lhs, Cinfo rhs) => !lhs.Equals(rhs);

            /// <summary>
            /// Generates a hash suitable for use with containers like `HashSet` and `Dictionary`.
            /// </summary>
            /// <returns>A hash code generated from this object's fields.</returns>
            public override int GetHashCode()
            {
                int hashCode = 486187739;
                unchecked
                {
                    hashCode = (hashCode * 486187739) + HashCodeUtil.ReferenceHash(id);
                    hashCode = (hashCode * 486187739) + HashCodeUtil.ReferenceHash(providerType);
                    hashCode = (hashCode * 486187739) + HashCodeUtil.ReferenceHash(subsystemTypeOverride);
                    hashCode = HashCodeUtil.Combine(hashCode,
                        HashCodeUtil.ReferenceHash(humanSegmentationStencilImageSupportedDelegate),
                        HashCodeUtil.ReferenceHash(humanSegmentationDepthImageSupportedDelegate),
                        HashCodeUtil.ReferenceHash(environmentDepthImageSupportedDelegate),
                        HashCodeUtil.ReferenceHash(environmentDepthTemporalSmoothingSupportedDelegate),
                        HashCodeUtil.ReferenceHash(environmentDepthConfidenceImageSupportedDelegate));
                }
                return hashCode;
            }
        }

        /// <summary>
        /// Creates the occlusion subsystem descriptor from the construction info.
        /// </summary>
        /// <param name="cinfo">The occlusion subsystem descriptor constructor information.</param>
        public static void Register(Cinfo cinfo)
        {
            if (string.IsNullOrEmpty(cinfo.id))
            {
                throw new ArgumentException("Cannot create occlusion subsystem descriptor because id is invalid",
                                            nameof(cinfo));
            }

            if (cinfo.providerType == null
                || !cinfo.providerType.IsSubclassOf(typeof(XROcclusionSubsystem.Provider)))
            {
                throw new ArgumentException("Cannot create occlusion subsystem descriptor because providerType is invalid",
                                            nameof(cinfo));
            }

            if (cinfo.subsystemTypeOverride == null
                || !cinfo.subsystemTypeOverride.IsSubclassOf(typeof(XROcclusionSubsystem)))
            {
                throw new ArgumentException("Cannot create occlusion subsystem descriptor because subsystemTypeOverride is invalid",
                                            nameof(cinfo));
            }

            SubsystemDescriptorStore.RegisterDescriptor(new XROcclusionSubsystemDescriptor(cinfo));
        }

        XROcclusionSubsystemDescriptor(Cinfo occlusionSubsystemCinfo)
        {
            id = occlusionSubsystemCinfo.id;
            providerType = occlusionSubsystemCinfo.providerType;
            subsystemTypeOverride = occlusionSubsystemCinfo.subsystemTypeOverride;
            m_EnvironmentDepthImageSupportedDelegate = occlusionSubsystemCinfo.environmentDepthImageSupportedDelegate;
            m_EnvironmentDepthConfidenceImageSupportedDelegate = occlusionSubsystemCinfo.environmentDepthConfidenceImageSupportedDelegate;
            m_HumanSegmentationStencilImageSupportedDelegate = occlusionSubsystemCinfo.humanSegmentationStencilImageSupportedDelegate;
            m_HumanSegmentationDepthImageSupportedDelegate = occlusionSubsystemCinfo.humanSegmentationDepthImageSupportedDelegate;
            m_EnvironmentDepthTemporalSmoothingSupportedDelegate = occlusionSubsystemCinfo.environmentDepthTemporalSmoothingSupportedDelegate;
        }
    }
}
