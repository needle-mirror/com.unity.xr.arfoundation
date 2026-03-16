using System;
using UnityEngine.SubsystemsImplementation;

namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// Describes features of an <see cref="XRObjectTrackingSubsystem"/>.
    /// </summary>
    /// <remarks>
    /// Enumerate available subsystems with <c>SubsystemManager.GetSubsystemDescriptors</c> and instantiate one by calling
    /// <c>Create</c> on one of the descriptors.
    /// Subsystem implementors can register their subsystem with
    /// <see cref="XRObjectTrackingSubsystem.Register{T}(string, XRObjectTrackingSubsystemDescriptor.Capabilities)"/>.
    /// </remarks>
    public class XRObjectTrackingSubsystemDescriptor :
        SubsystemDescriptorWithProvider<XRObjectTrackingSubsystem, XRObjectTrackingSubsystem.Provider>
    {
        /// <summary>
        /// Describes the capabilities of an <see cref="XRObjectTrackingSubsystem"/> implementation.
        /// </summary>
        [Obsolete("XRObjectTrackingSubsystemDescriptor.Capabilities has been deprecated in AR Foundation 6.0. Use XRObjectTrackingSubsystemDescriptor.Cinfo instead of XRObjectTrackingSubsystemDescriptor.Capabilities when registering a descriptor.", false)]
        public Capabilities capabilities { get; private set; }

        /// <summary>
        /// Describes the capabilities of an <see cref="XRObjectTrackingSubsystem"/> implementation.
        /// </summary>
        [Obsolete("XRObjectTrackingSubsystemDescriptor.Capabilities has been deprecated in AR Foundation 6.0. Use XRObjectTrackingSubsystemDescriptor.Cinfo instead of XRObjectTrackingSubsystemDescriptor.Capabilities when registering a descriptor.", false)]
        public struct Capabilities : IEquatable<Capabilities>
        {
            /// <summary>
            /// Tests for equality.
            /// </summary>
            /// <param name="other">The other <see cref="Capabilities"/> to compare against.</param>
            /// <returns>`True` if every field in <paramref name="other"/> is equal to this <see cref="Capabilities"/>, otherwise `false`.</returns>
            public bool Equals(Capabilities other) => true;

            /// <summary>
            /// Tests for equality.
            /// </summary>
            /// <param name="obj">The `object` to compare against.</param>
            /// <returns>`True` if <paramref name="obj"/> is of type <see cref="Capabilities"/> and
            /// <see cref="Equals(Capabilities)"/> also returns `true`; otherwise `false`.</returns>
            public override bool Equals(object obj) => (obj is Capabilities capabilities) && Equals(capabilities);

            /// <summary>
            /// Generates a hash suitable for use with containers like `HashSet` and `Dictionary`.
            /// </summary>
            /// <returns>A hash code generated from this object's fields.</returns>
            public override int GetHashCode() => 0;

            /// <summary>
            /// Tests for equality. Same as <see cref="Equals(Capabilities)"/>.
            /// </summary>
            /// <param name="lhs">The left-hand side of the comparison.</param>
            /// <param name="rhs">The right-hand side of the comparison.</param>
            /// <returns>`True` if <paramref name="lhs"/> is equal to <paramref name="rhs"/>, otherwise `false`.</returns>
            public static bool operator ==(Capabilities lhs, Capabilities rhs) => lhs.Equals(rhs);

            /// <summary>
            /// Tests for inequality. Same as `!`<see cref="Equals(Capabilities)"/>.
            /// </summary>
            /// <param name="lhs">The left-hand side of the comparison.</param>
            /// <param name="rhs">The right-hand side of the comparison.</param>
            /// <returns>`True` if <paramref name="lhs"/> is not equal to <paramref name="rhs"/>, otherwise `false`.</returns>
            public static bool operator !=(Capabilities lhs, Capabilities rhs) => !lhs.Equals(rhs);
        }

        /// <summary>
        /// This struct is an initializer for the creation of a <see cref="XRObjectTrackingSubsystemDescriptor"/>.
        /// </summary>
        /// <remarks>
        /// Object tracking data provider should create a descriptor during <c>InitializeOnLoad</c> using
        /// the parameters here to specify which of the <c>XRObjectTrackingSubsystem</c> features it supports.
        /// </remarks>
        public struct Cinfo : IEquatable<Cinfo>
        {
            /// <summary>
            /// The string identifier for a specific implementation.
            /// </summary>
            public string id { get; set; }

            /// <summary>
            /// Specifies the provider implementation type to use for instantiation.
            /// </summary>
            /// <value>
            /// The provider implementation type to use for instantiation.
            /// </value>
            public Type providerType { get; set; }

            /// <summary>
            /// Specifies the <c>XRObjectTrackingSubsystem</c>-derived type that forwards casted calls to its provider.
            /// </summary>
            /// <value>
            /// The type of the subsystem to use for instantiation. If null, <c>XRObjectTrackingSubsystem</c> will be instantiated.
            /// </value>
            public Type subsystemTypeOverride { get; set; }

            /// <summary>
            /// Tests for equality.
            /// </summary>
            /// <param name="other">The other <see cref="Cinfo"/> to compare against.</param>
            /// <returns>`True` if every field in <paramref name="other"/> is equal to this <see cref="Cinfo"/>, otherwise false.</returns>
            public bool Equals(Cinfo other)
            {
                return
                    id == other.id &&
                    providerType == other.providerType &&
                    subsystemTypeOverride == other.subsystemTypeOverride;
            }

            /// <summary>
            /// Tests for equality.
            /// </summary>
            /// <param name="obj">The `object` to compare against.</param>
            /// <returns>`True` if <paramref name="obj"/> is of type <see cref="Cinfo"/> and
            /// <see cref="Equals(Cinfo)"/> also returns `true`; otherwise `false`.</returns>
            public override bool Equals(object obj) => (obj is Cinfo) && Equals((Cinfo)obj);

            /// <summary>
            /// Generates a hash suitable for use with containers like `HashSet` and `Dictionary`.
            /// </summary>
            /// <returns>A hash code generated from this object's fields.</returns>
            public override int GetHashCode()
            {
                unchecked
                {
                    int hashCode = HashCodeUtil.ReferenceHash(id);
                    hashCode = 486187739 * hashCode + HashCodeUtil.ReferenceHash(providerType);
                    hashCode = 486187739 * hashCode + HashCodeUtil.ReferenceHash(subsystemTypeOverride);
                    return hashCode;
                }
            }

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
        }

        XRObjectTrackingSubsystemDescriptor(Cinfo cinfo)
        {
            id = cinfo.id;
            providerType = cinfo.providerType;
            subsystemTypeOverride = cinfo.subsystemTypeOverride;
        }

        /// <summary>
        /// Registers a subsystem descriptor with the <c>SubsystemManager</c>.
        /// </summary>
        /// <param name="cinfo">Parameters describing the <see cref="XRObjectTrackingSubsystem"/>.</param>
        public static void Register(Cinfo cinfo)
        {
            SubsystemDescriptorStore.RegisterDescriptor(new XRObjectTrackingSubsystemDescriptor(cinfo));
        }
    }
}
