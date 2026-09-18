using System;
using UnityEngine.SubsystemsImplementation;

namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// Indicates the capabilities supported by a provider of the <see cref="XRPlaneSubsystem"/>. Provider
    /// implementations must derive from <see cref="XRPlaneSubsystem.Provider"/> and may override virtual class members.
    /// </summary>
    public class XRPlaneSubsystemDescriptor : SubsystemDescriptorWithProvider<XRPlaneSubsystem, XRPlaneSubsystem.Provider>
    {
        bool m_SupportsBoundaryVerticesObsolete;
        bool m_SupportsClassificationObsolete;
        Func<bool> m_SupportsBoundaryVerticesDelegate;
        Func<bool> m_SupportsClassificationDelegate;
        Func<bool> m_SupportsPlaneSubsumptionDelegate;

        /// <summary>
        /// Indicates whether the provider implementation supports the detection of horizontal planes,
        /// such as the floor.
        /// If `false`, <see cref="BoundedPlane"/> trackables returned by
        /// <see cref="XRPlaneSubsystem.GetChanges">XRPlaneSubsystem.GetChanges</see> must not have their
        /// <see cref="BoundedPlane.alignment"/> values set to either <see cref="PlaneAlignment.HorizontalDown"/> or
        /// <see cref="PlaneAlignment.HorizontalUp"/>.
        /// </summary>
        /// <value>`true` if the implementation supports horizontal plane detection.
        /// Otherwise, `false`.</value>
        public bool supportsHorizontalPlaneDetection { get; }

        /// <summary>
        /// Indicates whether the provider implementation supports the detection of vertical planes, such as walls.
        /// If `false`, <see cref="BoundedPlane"/> trackables returned by
        /// <see cref="XRPlaneSubsystem.GetChanges">XRPlaneSubsystem.GetChanges</see> must not have their
        /// <see cref="BoundedPlane.alignment"/> value set to <see cref="PlaneAlignment.Vertical"/>.
        /// </summary>
        /// <value>`true` if the implementation supports vertical plane detection.
        /// Otherwise, `false`.</value>
        public bool supportsVerticalPlaneDetection { get; }

        /// <summary>
        /// Indicates whether the provider implementation supports the detection of planes that are aligned with
        /// neither the horizontal nor vertical axes.
        /// If `false`, <see cref="BoundedPlane"/> trackables returned by
        /// <see cref="XRPlaneSubsystem.GetChanges">XRPlaneSubsystem.GetChanges</see> must not have their
        /// <see cref="BoundedPlane.alignment"/> value set to <see cref="PlaneAlignment.NotAxisAligned"/>.
        /// </summary>
        /// <value>`true` if the implementation supports the detection of planes oriented at arbitrary angles.
        /// Otherwise, `false`.</value>
        public bool supportsArbitraryPlaneDetection { get; }

        /// <summary>
        /// Indicates whether the provider implementation supports boundary vertices for its planes.
        /// If `false`, <see cref="XRPlaneSubsystem.GetBoundary">XRPlaneSubsystem.GetBoundary</see>
        /// must throw a <see cref="NotSupportedException"/>.
        /// </summary>
        /// <value>`true` if the implementation supports boundary vertices for its planes.
        /// Otherwise, `false`.</value>
        public bool supportsBoundaryVertices
            => m_SupportsBoundaryVerticesDelegate?.Invoke() ?? m_SupportsBoundaryVerticesObsolete;

        /// <summary>
        /// Indicates whether the provider implementation can provide a value for
        /// <see cref="BoundedPlane.classifications">BoundedPlane.classifications</see>. If `false`, all
        /// planes returned by <see cref="XRPlaneSubsystem.GetChanges">XRPlaneSubsystem.GetChanges</see> will have
        /// <c>classifications</c> value of <see cref="PlaneClassifications.None"/>.
        /// </summary>
        /// <value>`true` if the implementation supports plane classifications.
        /// Otherwise, `false`.</value>
        public bool supportsClassification
            => m_SupportsClassificationDelegate?.Invoke() ?? m_SupportsClassificationObsolete;

        /// <summary>
        /// Indicates whether the provider implementation supports the ability to absorb one plane
        /// into another while plane detection is enabled, if the runtime determines that both planes
        /// represent the same surface. If `false`, <see cref="BoundedPlane"/> trackables returned by
        /// <see cref="XRPlaneSubsystem.GetChanges">XRPlaneSubsystem.GetChanges</see>
        /// must have their `subsumedById` properties set to `TrackableId.invalidId`.
        /// </summary>
        /// <value>`true` if the implementation supports plane subsumption.
        /// Otherwise, `false`.</value>
        public bool supportsPlaneSubsumption
            => m_SupportsPlaneSubsumptionDelegate?.Invoke() ?? false;

        /// <summary>
        /// Contains the parameters necessary to construct a new <see cref="XRPlaneSubsystemDescriptor"/> instance.
        /// </summary>
        public struct Cinfo : IEquatable<Cinfo>
        {
            /// <summary>
            /// The unique identifier of the provider implementation. No specific format is required.
            /// </summary>
            public string id { get; set; }

            /// <summary>
            /// The provider implementation type to use for instantiation.
            /// </summary>
            /// <value>The provider implementation type.</value>
            public Type providerType { get; set; }

            /// <summary>
            /// The <see cref="XRPlaneSubsystem"/>-derived type to use for instantiation. The instantiated instance of
            /// this type will forward casted calls to its provider.
            /// </summary>
            /// <value>The subsystem implementation type.
            /// If <see langword="null"/>, <see cref="XRPlaneSubsystem"/> will be instantiated.</value>
            public Type subsystemTypeOverride { get; set; }

            /// <summary>
            /// Indicates whether the provider implementation supports the detection of horizontal planes,
            /// such as the floor.
            /// If `false`, <see cref="BoundedPlane"/> trackables returned by
            /// <see cref="XRPlaneSubsystem.GetChanges">XRPlaneSubsystem.GetChanges</see> must not have their
            /// <see cref="BoundedPlane.alignment"/> value set to either <see cref="PlaneAlignment.HorizontalDown"/> or
            /// <see cref="PlaneAlignment.HorizontalUp"/>.
            /// </summary>
            /// <value>`true` if the implementation supports horizontal plane detection.
            /// Otherwise, `false`.</value>
            public bool supportsHorizontalPlaneDetection { get; set; }

            /// <summary>
            /// Indicates whether the provider implementation supports the detection of vertical planes, such as walls.
            /// If `false`, <see cref="BoundedPlane"/> trackables returned by
            /// <see cref="XRPlaneSubsystem.GetChanges">XRPlaneSubsystem.GetChanges</see> must not have their
            /// <see cref="BoundedPlane.alignment"/> value set to <see cref="PlaneAlignment.Vertical"/>.
            /// </summary>
            /// <value>`true` if the implementation supports vertical plane detection.
            /// Otherwise, `false`.</value>
            public bool supportsVerticalPlaneDetection { get; set; }

            /// <summary>
            /// Indicates whether the provider implementation supports the detection of planes that are aligned with
            /// neither the horizontal nor vertical axes.
            /// If `false`, <see cref="BoundedPlane"/> trackables returned by
            /// <see cref="XRPlaneSubsystem.GetChanges">XRPlaneSubsystem.GetChanges</see> must not have their
            /// <see cref="BoundedPlane.alignment"/> value set to <see cref="PlaneAlignment.NotAxisAligned"/>.
            /// </summary>
            /// <value>`true` if the implementation supports the detection of planes oriented at arbitrary angles.
            /// Otherwise, `false`.</value>
            public bool supportsArbitraryPlaneDetection { get; set; }

            /// <summary>
            /// Indicates whether the provider implementation supports boundary vertices for its planes.
            /// If `false`, <see cref="XRPlaneSubsystem.GetBoundary">XRPlaneSubsystem.GetBoundary</see>
            /// must throw a <see cref="NotSupportedException"/>.
            /// </summary>
            /// <value>`true` if the implementation supports boundary vertices for its planes.
            /// Otherwise, `false`.</value>
            [Obsolete("supportsBoundaryVertices is deprecated in AR Foundation 6.5.0-pre.1. Use supportsBoundaryVerticesDelegate instead.")]
            public bool supportsBoundaryVertices { get; set; }

            /// <summary>
            /// Indicates whether the provider implementation can provide a value for
            /// <see cref="BoundedPlane.classifications">BoundedPlane.classifications</see>. If `false`, all
            /// planes returned by <see cref="XRPlaneSubsystem.GetChanges">XRPlaneSubsystem.GetChanges</see> will have a
            /// <c>classifications</c> value of <see cref="PlaneClassifications.None"/>.
            /// </summary>
            /// <value>`true` if the implementation supports plane classification.
            /// Otherwise, `false`.</value>
            [Obsolete("supportsClassification is deprecated in AR Foundation 6.5.0-pre.1. Use supportsClassificationDelegate instead.")]
            public bool supportsClassification { get; set; }

            /// <summary>
            /// Indicates whether the provider implementation supports boundary vertices for its planes.
            /// If `false`, <see cref="XRPlaneSubsystem.GetBoundary">XRPlaneSubsystem.GetBoundary</see>
            /// must throw a <see cref="NotSupportedException"/>.
            /// </summary>
            /// <value>`true` if the implementation supports boundary vertices for its planes.
            /// Otherwise, `false`.</value>
            public Func<bool> supportsBoundaryVerticesDelegate { get; set; }

            /// <summary>
            /// Indicates whether the provider implementation can provide a value for
            /// <see cref="BoundedPlane.classifications">BoundedPlane.classifications</see>. If `false`, all
            /// planes returned by <see cref="XRPlaneSubsystem.GetChanges">XRPlaneSubsystem.GetChanges</see> will have a
            /// <c>classifications</c> value of <see cref="PlaneClassifications.None"/>.
            /// </summary>
            /// <value>`true` if the implementation supports plane classification.
            /// Otherwise, `false`.</value>
            public Func<bool> supportsClassificationDelegate { get; set; }

            /// <summary>
            /// Indicates whether the provider implementation supports the ability to absorb one plane
            /// into another while plane detection is enabled, if the runtime determines that both planes
            /// represent the same surface. If `false`, <see cref="BoundedPlane"/> trackables returned by
            /// <see cref="XRPlaneSubsystem.GetChanges">XRPlaneSubsystem.GetChanges</see>
            /// must have their `subsumedById` properties set to `TrackableId.invalidId`.
            /// </summary>
            /// <value>`true` if the implementation supports plane subsumption.
            /// Otherwise, `false`.</value>
            public Func<bool> supportsPlaneSubsumptionDelegate { get; set; }

            /// <summary>
            /// Generates a hash suitable for use with containers like `HashSet` and `Dictionary`.
            /// </summary>
            /// <returns>A hash code generated from this object's fields.</returns>
            public override int GetHashCode()
            {
                var hashCode = new HashCode();
                hashCode.Add(id);
                hashCode.Add(providerType);
                hashCode.Add(subsystemTypeOverride);
                hashCode.Add(supportsHorizontalPlaneDetection);
                hashCode.Add(supportsVerticalPlaneDetection);
                hashCode.Add(supportsArbitraryPlaneDetection);
#pragma warning disable CS0618 // Type or member is obsolete
                hashCode.Add(supportsBoundaryVertices);
                hashCode.Add(supportsClassification);
#pragma warning restore CS0618 // Type or member is obsolete
                hashCode.Add(supportsBoundaryVerticesDelegate);
                hashCode.Add(supportsClassificationDelegate);
                hashCode.Add(supportsPlaneSubsumptionDelegate);
                return hashCode.ToHashCode();
            }

            /// <summary>
            /// Tests for equality.
            /// </summary>
            /// <param name="other">The other <see cref="Cinfo"/> to compare against.</param>
            /// <returns>`true` if every field in <paramref name="other"/> is equal to this instance.
            /// Otherwise, `false`.</returns>
            public bool Equals(Cinfo other)
            {
                return
                    ReferenceEquals(id, other.id) &&
                    ReferenceEquals(providerType, other.providerType) &&
                    ReferenceEquals(subsystemTypeOverride, other.subsystemTypeOverride) &&
                    supportsHorizontalPlaneDetection == other.supportsHorizontalPlaneDetection &&
                    supportsVerticalPlaneDetection == other.supportsVerticalPlaneDetection &&
                    supportsArbitraryPlaneDetection == other.supportsArbitraryPlaneDetection &&
                    supportsClassificationDelegate == other.supportsClassificationDelegate &&
                    supportsBoundaryVerticesDelegate == other.supportsBoundaryVerticesDelegate &&
                    supportsPlaneSubsumptionDelegate == other.supportsPlaneSubsumptionDelegate &&
#pragma warning disable CS0618 // Type or member is obsolete
                    supportsBoundaryVertices == other.supportsBoundaryVertices &&
                    supportsClassification == other.supportsClassification;
#pragma warning restore CS0618 // Type or member is obsolete

            }

            /// <summary>
            /// Tests for equality.
            /// </summary>
            /// <param name="obj">The <c>object</c> to compare against.</param>
            /// <returns>`true` if <paramref name="obj"/> is of type <see cref="Cinfo"/> and
            /// <see cref="Equals(Cinfo)"/> also returns `true`.
            /// Otherwise, `false`.</returns>
            public override bool Equals(object obj)
            {
                if (!(obj is Cinfo))
                    return false;

                return Equals((Cinfo)obj);
            }

            /// <summary>
            /// Tests for equality. Equivalent to <see cref="Equals(Cinfo)"/>.
            /// </summary>
            /// <param name="lhs">The left-hand side of the comparison.</param>
            /// <param name="rhs">The right-hand side of the comparison.</param>
            /// <returns>`true` if <paramref name="lhs"/> is equal to <paramref name="rhs"/>.
            /// Otherwise, `false`.</returns>
            public static bool operator ==(Cinfo lhs, Cinfo rhs) => lhs.Equals(rhs);

            /// <summary>
            /// Tests for inequality. Equivalent to `!`<see cref="Equals(Cinfo)"/>.
            /// </summary>
            /// <param name="lhs">The left-hand side of the comparison.</param>
            /// <param name="rhs">The right-hand side of the comparison.</param>
            /// <returns>`true` if <paramref name="lhs"/> is not equal to <paramref name="rhs"/>.
            /// Otherwise, `false`.</returns>
            public static bool operator !=(Cinfo lhs, Cinfo rhs) => !lhs.Equals(rhs);
        }

        /// <summary>
        /// Creates a new subsystem descriptor instance and registers it with the [SubsystemManager](xref:UnityEngine.SubsystemManager).
        /// </summary>
        /// <param name="cinfo">Construction info for the descriptor.</param>
        [Obsolete("Create(Cinfo) has been deprecated in AR Foundation version 6.0. Use Register(Cinfo) instead (UnityUpgradable) -> Register(*)", false)]
        public static void Create(Cinfo cinfo)
        {
            Register(cinfo);
        }

        /// <summary>
        /// Creates a new subsystem descriptor instance and registers it with the [SubsystemManager](xref:UnityEngine.SubsystemManager).
        /// </summary>
        /// <param name="cinfo">Construction info for the descriptor.</param>
        public static void Register(Cinfo cinfo)
        {
            SubsystemDescriptorStore.RegisterDescriptor(new XRPlaneSubsystemDescriptor(cinfo));
        }

        XRPlaneSubsystemDescriptor(Cinfo cinfo)
        {
            id = cinfo.id;
            providerType = cinfo.providerType;
            subsystemTypeOverride = cinfo.subsystemTypeOverride;
            supportsHorizontalPlaneDetection = cinfo.supportsHorizontalPlaneDetection;
            supportsVerticalPlaneDetection = cinfo.supportsVerticalPlaneDetection;
            supportsArbitraryPlaneDetection = cinfo.supportsArbitraryPlaneDetection;
#pragma warning disable CS0618 // Type or member is obsolete
            m_SupportsBoundaryVerticesObsolete = cinfo.supportsBoundaryVertices;
            m_SupportsClassificationObsolete = cinfo.supportsClassification;
#pragma warning restore CS0618 // Type or member is obsolete
            m_SupportsBoundaryVerticesDelegate = cinfo.supportsBoundaryVerticesDelegate;
            m_SupportsClassificationDelegate = cinfo.supportsClassificationDelegate;
            m_SupportsPlaneSubsumptionDelegate = cinfo.supportsPlaneSubsumptionDelegate;
        }
    }
}
