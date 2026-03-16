using System;
using UnityEngine.SubsystemsImplementation;

namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// The descriptor of the <see cref="XRParticipantSubsystem"/> that shows which features are available on that XRSubsystem.
    /// </summary>
    /// <remarks>
    /// Use <see cref="Register{T}"/> to register a subsystem with the global <c>SubsystemManager</c>.
    /// </remarks>
    public sealed class XRParticipantSubsystemDescriptor :
        SubsystemDescriptorWithProvider<XRParticipantSubsystem, XRParticipantSubsystem.Provider>
    {
        /// <summary>
        /// The capabilities of a particular <see cref="XRParticipantSubsystem"/>. This is typically
        /// used to query a subsystem for capabilities that might vary by platform or implementation.
        /// </summary>
        [Flags]
        [Obsolete("XRParticipantSubsystemDescriptor.Capabilities has been deprecated in AR Foundation 6.0. Use XRParticipantSubsystemDescriptor.Cinfo instead of XRParticipantSubsystemDescriptor.Capabilities when registering a descriptor.", false)]
        public enum Capabilities
        {
            /// <summary>
            /// The <see cref="XRParticipantSubsystem"/> implementation has no
            /// additional capabilities other than the basic, required functionality.
            /// </summary>
            None = 0,
        }

        /// <summary>
        /// The capabilities provided by this implementation.
        /// </summary>
        [Obsolete("XRParticipantSubsystemDescriptor.Capabilities has been deprecated in AR Foundation 6.0. Use XRParticipantSubsystemDescriptor.Cinfo instead of XRParticipantSubsystemDescriptor.Capabilities when registering a descriptor.", false)]
        public Capabilities capabilities { get; private set; }

        /// <summary>
        /// This struct is an initializer for the creation of a <see cref="XRParticipantSubsystemDescriptor"/>.
        /// </summary>
        /// <remarks>
        /// Participant data provider should create a descriptor during <c>InitializeOnLoad</c> using
        /// the parameters here to specify which of the <c>XRParticipantSubsystem</c> features it supports.
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
            /// Specifies the <c>XRParticipantSubsystem</c>-derived type that forwards casted calls to its provider.
            /// </summary>
            /// <value>
            /// The type of the subsystem to use for instantiation. If null, <c>XRParticipantSubsystem</c> will be instantiated.
            /// </value>
            public Type subsystemTypeOverride { get; set; }

            /// <summary>
            /// Tests for equality.
            /// </summary>
            /// <param name="other">The other <see cref="Cinfo"/> to compare against.</param>
            /// <returns><see langword="true"/> if every field in <paramref name="other"/> is equal to this <see cref="Cinfo"/>. Otherwise, <see langword="false"/>.</returns>
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

        /// <summary>
        /// Register a provider implementation.
        /// This should only be used by subsystem implementors.
        /// </summary>
        /// <param name="subsystemId">The name of the specific subsystem implementation.</param>
        /// <param name="capabilities">The <see cref="Capabilities"/> of the specific subsystem implementation.</param>
        /// <typeparam name="T">The concrete type derived from <see cref="XRParticipantSubsystem"/> being registered.</typeparam>
        [Obsolete("XRParticipantSubsystemDescriptor.Register<T>(string subsystemId, Capabilities capabilities) has been deprecated in 6.0. Instead use XRParticipantSubsystemDescriptor.Register(XRParticipantSubsystemDescriptor.Cinfo cinfo)", false)]
        public static void Register<T>(string subsystemId, Capabilities capabilities)
            where T : XRParticipantSubsystem.Provider
        {
            Register(
                new XRParticipantSubsystemDescriptor.Cinfo()
                {
                    id = subsystemId,
                    providerType = typeof(T),
                    subsystemTypeOverride = null
                });
        }

        /// <summary>
        /// Register a provider implementation and subsystem override.
        /// This should only be used by subsystem implementors.
        /// </summary>
        /// <param name="subsystemId">The name of the specific subsystem implementation.</param>
        /// <param name="capabilities">The <see cref="Capabilities"/> of the specific subsystem implementation.</param>
        /// <typeparam name="TProvider">The concrete type of the provider being registered.</typeparam>
        /// <typeparam name="TSubsystemOverride">The concrete type of the subsystem being registered.</typeparam>
        [Obsolete("XRParticipantSubsystemDescriptor.Register<TProvider, TSubsystemOverride>(string subsystemId, Capabilities capabilities) has been deprecated in 6.0. Instead use XRParticipantSubsystemDescriptor.Register(XRParticipantSubsystemDescriptor.Cinfo cinfo)", false)]
        public static void Register<TProvider, TSubsystemOverride>(string subsystemId, Capabilities capabilities)
            where TProvider : XRParticipantSubsystem.Provider
            where TSubsystemOverride : XRParticipantSubsystem
        {
            Register(
                new XRParticipantSubsystemDescriptor.Cinfo()
                {
                    id = subsystemId,
                    providerType = typeof(TProvider),
                    subsystemTypeOverride = typeof(TSubsystemOverride)
                });
        }

        XRParticipantSubsystemDescriptor(Cinfo cinfo)
        {
            id = cinfo.id;
            providerType = cinfo.providerType;
            subsystemTypeOverride = cinfo.subsystemTypeOverride;
        }

        /// <summary>
        /// Registers a subsystem descriptor with the <c>SubsystemManager</c>.
        /// </summary>
        /// <param name="cinfo">Parameters describing the <see cref="XRParticipantSubsystem"/>.</param>
        public static void Register(Cinfo cinfo)
        {
            SubsystemDescriptorStore.RegisterDescriptor(new XRParticipantSubsystemDescriptor(cinfo));
        }
    }
}
