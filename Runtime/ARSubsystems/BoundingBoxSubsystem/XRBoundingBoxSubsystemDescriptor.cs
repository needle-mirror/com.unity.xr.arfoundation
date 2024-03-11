using System;
using UnityEngine.SubsystemsImplementation;

namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// Indicates the capabilities supported by a provider of the <see cref="XRBoundingBoxSubsystem"/>. Provider
    /// implementations must derive from <see cref="XRBoundingBoxSubsystem.Provider"/> and may override virtual class members.
    /// </summary>
    public class XRBoundingBoxSubsystemDescriptor : SubsystemDescriptorWithProvider<XRBoundingBoxSubsystem, XRBoundingBoxSubsystem.Provider>
    {
        /// <summary>
        /// Indicates whether the provider implementation can provide a value for
        /// <see cref="XRBoundingBox.classifications">XRBoundingBox.classifications</see>. If <see langword="false"/>, all
        /// bounding boxes returned by <see cref="XRBoundingBoxSubsystem.GetChanges">XRBoundingBoxSubsystem.GetChanges</see> will have
        /// <c>classifications</c> value of <see cref="BoundingBoxClassifications.None"/>.
        /// </summary>
        /// <value><see langword="true"/> if the implementation supports 3D bounding box classifications.
        /// Otherwise, <see langword="false"/>.</value>
        public bool supportsClassifications { get; }

        /// <summary>
        /// Creates a new subsystem descriptor instance and registers it with the [SubsystemManager](xref:UnityEngine.SubsystemManager).
        /// </summary>
        /// <param name="cinfo">Construction info for the descriptor.</param>
        public static void Register(Cinfo cinfo)
        {
            SubsystemDescriptorStore.RegisterDescriptor(new XRBoundingBoxSubsystemDescriptor(cinfo));
        }

        XRBoundingBoxSubsystemDescriptor(Cinfo cinfo)
        {
            id = cinfo.id;
            providerType = cinfo.providerType;
            subsystemTypeOverride = cinfo.subsystemTypeOverride;
            supportsClassifications = cinfo.supportsClassification;
        }

        /// <summary>
        /// Contains the parameters necessary to construct a new <see cref="XRBoundingBoxSubsystemDescriptor"/> instance.
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
            /// The <see cref="XRBoundingBoxSubsystem"/>-derived type to use for instantiation. The instantiated instance of
            /// this type will forward casted calls to its provider.
            /// </summary>
            /// <value>The subsystem implementation type.
            /// If <see langword="null"/>, <see cref="XRBoundingBoxSubsystem"/> will be instantiated.</value>
            public Type subsystemTypeOverride { get; set; }

            /// <summary>
            /// Indicates whether the provider implementation can provide a value for
            /// <see cref="XRBoundingBox.classifications">XRBoundingBox.classifications</see>. If <see langword="false"/>, all
            /// bounding boxes returned by <see cref="XRBoundingBoxSubsystem.GetChanges">XRBoundingBoxSubsystem.GetChanges</see> will have a
            /// <c>classifications</c> value of <see cref="BoundingBoxClassifications.None"/>.
            /// </summary>
            /// <value><see langword="true"/> if the implementation supports 3D bounding box classification.
            /// Otherwise, <see langword="false"/>.</value>
            public bool supportsClassification { get; set; }

            /// <summary>
            /// Tests for equality.
            /// </summary>
            /// <param name="other">The other <see cref="Cinfo"/> to compare against.</param>
            /// <returns><see langword="true"/> if every field in <paramref name="other"/> is equal to this instance.
            /// Otherwise, <see langword="false"/>.</returns>
            public bool Equals(Cinfo other)
            {
                return
                    ReferenceEquals(id, other.id) &&
                    ReferenceEquals(providerType, other.providerType) &&
                    ReferenceEquals(subsystemTypeOverride, other.subsystemTypeOverride) &&
                    supportsClassification == other.supportsClassification;
            }

            /// <summary>
            /// Tests for equality.
            /// </summary>
            /// <param name="obj">The other <see cref="Cinfo"/> to compare against.</param>
            /// <returns><see langword="true"/> if every field in <paramref name="other"/> is equal to this instance.
            /// Otherwise, <see langword="false"/>.</returns>
            public override bool Equals(object obj)
            {
                return obj is Cinfo other && Equals(other);
            }

            /// <summary>
            /// Generates a hash suitable for use with containers like `HashSet` and `Dictionary`.
            /// </summary>
            /// <returns>A hash code generated from this object's fields.</returns>
            public override int GetHashCode()
            {
                unchecked
                {
                    int hashCode = HashCodeUtil.ReferenceHash(id);
                    hashCode = (hashCode * 486187739) + HashCodeUtil.ReferenceHash(providerType);
                    hashCode = (hashCode * 486187739) + HashCodeUtil.ReferenceHash(subsystemTypeOverride);
                    hashCode = (hashCode * 486187739) + supportsClassification.GetHashCode();
                    return hashCode;
                }
            }

            /// <summary>
            /// Tests for equality. Equivalent to <see cref="Equals(Cinfo)"/>.
            /// </summary>
            /// <param name="lhs">The left-hand side of the comparison.</param>
            /// <param name="rhs">The right-hand side of the comparison.</param>
            /// <returns><see langword="true"/> if <paramref name="lhs"/> is equal to <paramref name="rhs"/>.
            /// Otherwise, <see langword="false"/>.</returns>
            public static bool operator ==(Cinfo lhs, Cinfo rhs) => lhs.Equals(rhs);

            /// <summary>
            /// Tests for inequality. Equivalent to `!`<see cref="Equals(Cinfo)"/>.
            /// </summary>
            /// <param name="lhs">The left-hand side of the comparison.</param>
            /// <param name="rhs">The right-hand side of the comparison.</param>
            /// <returns><see langword="true"/> if <paramref name="lhs"/> is not equal to <paramref name="rhs"/>.
            /// Otherwise, <see langword="false"/>.</returns>
            public static bool operator !=(Cinfo lhs, Cinfo rhs) => !lhs.Equals(rhs);
        }
    }
}
