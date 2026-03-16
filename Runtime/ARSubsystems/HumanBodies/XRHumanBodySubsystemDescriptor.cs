using System;
using UnityEngine.SubsystemsImplementation;

namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// Constructor info for the <see cref="XRHumanBodySubsystemDescriptor"/>.
    /// </summary>
    [Obsolete("XRHumanBodySubsystemCinfo has been deprecated in AR Foundation version 6.0. Use XRHumanBodySubsystemDescriptor.Cinfo instead (UnityUpgradable) -> UnityEngine.XR.ARSubsystems.XRHumanBodySubsystemDescriptor/Cinfo", false)]
    public struct XRHumanBodySubsystemCinfo : IEquatable<XRHumanBodySubsystemCinfo>
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
        /// Specifies the <c>XRHumanBodySubsystem</c>-derived type that forwards casted calls to its provider.
        /// </summary>
        /// <value>
        /// The type of the subsystem to use for instantiation. If null, <c>XRHumanBodySubsystem</c> will be instantiated.
        /// </value>
        public Type subsystemTypeOverride { get; set; }

        /// <summary>
        /// Specifies if the current subsystem supports 2D human body pose estimation.
        /// </summary>
        /// <value>
        /// <c>true</c> if the current subsystem supports 2D human body pose estimation. Otherwise, <c>false</c>.
        /// </value>
        public bool supportsHumanBody2D { get; set; }

        /// <summary>
        /// Specifies if the current subsystem supports 3D human body pose estimation.
        /// </summary>
        /// <value>
        /// <c>true</c> if the current subsystem supports 3D human body pose estimation. Otherwise, <c>false</c>.
        /// </value>
        public bool supportsHumanBody3D { get; set; }

        /// <summary>
        /// Specifies if the current subsystem supports 3D human body scale estimation.
        /// </summary>
        /// <value>
        /// <c>true</c> if the current subsystem supports 3D human body scale estimation. Otherwise, <c>false</c>.
        /// </value>
        public bool supportsHumanBody3DScaleEstimation { get; set; }

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <param name="other">The other <see cref="XRHumanBodySubsystemCinfo"/> to compare against.</param>
        /// <returns>`True` if every field in <paramref name="other"/> is equal to this <see cref="XRHumanBodySubsystemCinfo"/>, otherwise false.</returns>
        public bool Equals(XRHumanBodySubsystemCinfo other)
        {
            return
                ReferenceEquals(id, other.id)
                && ReferenceEquals(providerType, other.providerType)
                && ReferenceEquals(subsystemTypeOverride, subsystemTypeOverride)
                && supportsHumanBody2D.Equals(other.supportsHumanBody2D)
                && supportsHumanBody3D.Equals(other.supportsHumanBody3D)
                && supportsHumanBody3DScaleEstimation.Equals(other.supportsHumanBody3DScaleEstimation);
        }

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <param name="obj">The `object` to compare against.</param>
        /// <returns>`True` if <paramref name="obj"/> is of type <see cref="XRHumanBodySubsystemCinfo"/> and
        /// <see cref="Equals(XRHumanBodySubsystemCinfo)"/> also returns `true`; otherwise `false`.</returns>
        public override bool Equals(System.Object obj)
        {
            return ((obj is XRHumanBodySubsystemCinfo) && Equals((XRHumanBodySubsystemCinfo)obj));
        }

        /// <summary>
        /// Tests for equality. Same as <see cref="Equals(XRHumanBodySubsystemCinfo)"/>.
        /// </summary>
        /// <param name="lhs">The left-hand side of the comparison.</param>
        /// <param name="rhs">The right-hand side of the comparison.</param>
        /// <returns>`True` if <paramref name="lhs"/> is equal to <paramref name="rhs"/>, otherwise `false`.</returns>
        public static bool operator ==(XRHumanBodySubsystemCinfo lhs, XRHumanBodySubsystemCinfo rhs)
        {
            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Tests for inequality. Same as `!`<see cref="Equals(XRHumanBodySubsystemCinfo)"/>.
        /// </summary>
        /// <param name="lhs">The left-hand side of the comparison.</param>
        /// <param name="rhs">The right-hand side of the comparison.</param>
        /// <returns>`True` if <paramref name="lhs"/> is not equal to <paramref name="rhs"/>, otherwise `false`.</returns>
        public static bool operator !=(XRHumanBodySubsystemCinfo lhs, XRHumanBodySubsystemCinfo rhs)
        {
            return !lhs.Equals(rhs);
        }

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
                hashCode = (hashCode * 486187739) + supportsHumanBody2D.GetHashCode();
                hashCode = (hashCode * 486187739) + supportsHumanBody3D.GetHashCode();
                hashCode = (hashCode * 486187739) + supportsHumanBody3DScaleEstimation.GetHashCode();
            }
            return hashCode;
        }
    }

    /// <summary>
    /// The descriptor for the <see cref="XRHumanBodySubsystem"/>.
    /// </summary>
    public class XRHumanBodySubsystemDescriptor : SubsystemDescriptorWithProvider<XRHumanBodySubsystem, XRHumanBodySubsystem.Provider>
    {
        /// <summary>
        /// Specifies if the current subsystem supports 2D human body pose estimation.
        /// </summary>
        /// <value>
        /// <c>true</c> if the current subsystem supports 2D human body pose estimation. Otherwise, <c>false</c>.
        /// </value>
        public bool supportsHumanBody2D { get; private set; }

        /// <summary>
        /// Specifies if the current subsystem supports 3D human body pose estimation.
        /// </summary>
        /// <value>
        /// <c>true</c> if the current subsystem supports 3D human body pose estimation. Otherwise, <c>false</c>.
        /// </value>
        public bool supportsHumanBody3D { get; private set; }

        /// <summary>
        /// Specifies if the current subsystem supports 3D human body scale estimation.
        /// </summary>
        /// <value>
        /// <c>true</c> if the current subsystem supports 3D human body scale estimation. Otherwise, <c>false</c>.
        /// </value>
        public bool supportsHumanBody3DScaleEstimation { get; private set; }

        /// <summary>
        /// Constructor info for the <see cref="XRHumanBodySubsystemDescriptor"/>.
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
            /// Specifies the <c>XRHumanBodySubsystem</c>-derived type that forwards casted calls to its provider.
            /// </summary>
            /// <value>
            /// The type of the subsystem to use for instantiation. If null, <c>XRHumanBodySubsystem</c> will be instantiated.
            /// </value>
            public Type subsystemTypeOverride { get; set; }

            /// <summary>
            /// Specifies if the current subsystem supports 2D human body pose estimation.
            /// </summary>
            /// <value>
            /// <c>true</c> if the current subsystem supports 2D human body pose estimation. Otherwise, <c>false</c>.
            /// </value>
            public bool supportsHumanBody2D { get; set; }

            /// <summary>
            /// Specifies if the current subsystem supports 3D human body pose estimation.
            /// </summary>
            /// <value>
            /// <c>true</c> if the current subsystem supports 3D human body pose estimation. Otherwise, <c>false</c>.
            /// </value>
            public bool supportsHumanBody3D { get; set; }

            /// <summary>
            /// Specifies if the current subsystem supports 3D human body scale estimation.
            /// </summary>
            /// <value>
            /// <c>true</c> if the current subsystem supports 3D human body scale estimation. Otherwise, <c>false</c>.
            /// </value>
            public bool supportsHumanBody3DScaleEstimation { get; set; }

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
                    ReferenceEquals(subsystemTypeOverride, subsystemTypeOverride) &&
                    supportsHumanBody2D.Equals(other.supportsHumanBody2D) &&
                    supportsHumanBody3D.Equals(other.supportsHumanBody3D) &&
                    supportsHumanBody3DScaleEstimation.Equals(other.supportsHumanBody3DScaleEstimation);
            }

            /// <summary>
            /// Tests for equality.
            /// </summary>
            /// <param name="obj">The `object` to compare against.</param>
            /// <returns>`True` if <paramref name="obj"/> is of type <see cref="Cinfo"/> and
            /// <see cref="Equals(Cinfo)"/> also returns `true`; otherwise `false`.</returns>
            public override bool Equals(System.Object obj)
            {
                return ((obj is Cinfo) && Equals((Cinfo)obj));
            }

            /// <summary>
            /// Tests for equality. Same as <see cref="Equals(Cinfo)"/>.
            /// </summary>
            /// <param name="lhs">The left-hand side of the comparison.</param>
            /// <param name="rhs">The right-hand side of the comparison.</param>
            /// <returns>`True` if <paramref name="lhs"/> is equal to <paramref name="rhs"/>, otherwise `false`.</returns>
            public static bool operator ==(Cinfo lhs, Cinfo rhs)
            {
                return lhs.Equals(rhs);
            }

            /// <summary>
            /// Tests for inequality. Same as `!`<see cref="Equals(Cinfo)"/>.
            /// </summary>
            /// <param name="lhs">The left-hand side of the comparison.</param>
            /// <param name="rhs">The right-hand side of the comparison.</param>
            /// <returns>`True` if <paramref name="lhs"/> is not equal to <paramref name="rhs"/>, otherwise `false`.</returns>
            public static bool operator !=(Cinfo lhs, Cinfo rhs)
            {
                return !lhs.Equals(rhs);
            }

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
                    hashCode = (hashCode * 486187739) + supportsHumanBody2D.GetHashCode();
                    hashCode = (hashCode * 486187739) + supportsHumanBody3D.GetHashCode();
                    hashCode = (hashCode * 486187739) + supportsHumanBody3DScaleEstimation.GetHashCode();
                }
                return hashCode;
            }
        }

        /// <summary>
        /// Registers a subsystem implementation based on the given subystem parameters, and validates that the
        /// <see cref="Cinfo.id"/> and <see cref="Cinfo.providerType"/>
        /// properties are specified.
        /// </summary>
        /// <param name='cinfo'>The parameters required to initialize the descriptor.</param>
        /// <exception cref="ArgumentException">Thrown when the values specified in the
        /// <paramref name="cinfo"/> parameter are invalid. Typically, this happens when
        /// required parameters are <see langword="null"/> or empty or types that do not derive from the required base class.
        /// </exception>
        public static void Register(Cinfo cinfo)
        {
            if (String.IsNullOrEmpty(cinfo.id))
            {
                throw new ArgumentException("Cannot create human body subsystem descriptor because id is invalid",
                                            nameof(cinfo));
            }

            if (cinfo.providerType == null
                || !cinfo.providerType.IsSubclassOf(typeof(XRHumanBodySubsystem.Provider)))
            {
                throw new ArgumentException("Cannot create human body subsystem descriptor because providerType is invalid",
                                            nameof(cinfo));
            }

            if (cinfo.subsystemTypeOverride != null
                && !cinfo.subsystemTypeOverride.IsSubclassOf(typeof(XRHumanBodySubsystem)))
            {
                throw new ArgumentException("Cannot create human body subsystem descriptor because subsystemTypeOverride is invalid",
                                            nameof(cinfo));
            }

            SubsystemDescriptorStore.RegisterDescriptor(new XRHumanBodySubsystemDescriptor(cinfo));
        }

        XRHumanBodySubsystemDescriptor(Cinfo humanBodySubsystemCinfo)
        {
            id = humanBodySubsystemCinfo.id;
            providerType = humanBodySubsystemCinfo.providerType;
            subsystemTypeOverride = humanBodySubsystemCinfo.subsystemTypeOverride;
            supportsHumanBody2D = humanBodySubsystemCinfo.supportsHumanBody2D;
            supportsHumanBody3D = humanBodySubsystemCinfo.supportsHumanBody3D;
            supportsHumanBody3DScaleEstimation = humanBodySubsystemCinfo.supportsHumanBody3DScaleEstimation;
        }
    }
}
