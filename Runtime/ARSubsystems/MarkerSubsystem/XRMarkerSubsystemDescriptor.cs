using System;
using Unity.XR.CoreUtils.Collections;
using UnityEngine.SubsystemsImplementation;

namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// Describes the capabilities of an <see cref="XRMarkerSubsystem"/> implementation, including supported marker types.
    /// </summary>
    /// <remarks>
    /// This descriptor is used by Unity's subsystem infrastructure to discover and instantiate subsystems at runtime.
    /// Providers should populate the supported marker types for their implementation.
    /// </remarks>
    public class XRMarkerSubsystemDescriptor
        : SubsystemDescriptorWithProvider<XRMarkerSubsystem, XRMarkerSubsystem.Provider>
    {
        /// <summary>
        /// Attempts to get a list of marker types supported by this subsystem implementation.
        /// </summary>
        /// <remarks>
        /// The list indicates which marker families (such as QR codes, MicroQR codes ArUco tags, or AprilTag)
        /// can be detected by this subsystem. Use this for capability checks and feature gating.
        ///
        /// This property returns a `Result` so you must check if the
        /// operation was successful before accessing the value with `Result.status.IsSuccess`. If successful,
        /// the `Result` contains a <see cref="ReadOnlyListSpan{XRMarkerType}"/> of the supported marker types.
        /// </remarks>
        public Result<ReadOnlyListSpan<XRMarkerType>> supportedMarkerTypes
        {
            get
            {
                if (m_SupportedMarkerTypesDelegate == null)
                {
                    var unsupportedStatus = new XRResultStatus(XRResultStatus.StatusCode.UnknownError);
                    return new Result<ReadOnlyListSpan<XRMarkerType>>(unsupportedStatus, new());
                }

                return m_SupportedMarkerTypesDelegate.Invoke();
            }
        }

        Func<Result<ReadOnlyListSpan<XRMarkerType>>> m_SupportedMarkerTypesDelegate;

        XRMarkerSubsystemDescriptor(Cinfo markerSubsystemCinfo)
        {
            id = markerSubsystemCinfo.id;
            providerType = markerSubsystemCinfo.providerType;
            subsystemTypeOverride = markerSubsystemCinfo.subsystemTypeOverride;
            m_SupportedMarkerTypesDelegate = markerSubsystemCinfo.supportedMarkerTypesDelegate;
        }

        /// <summary>
        /// Creates a new subsystem descriptor instance and registers it with the [SubsystemManager](xref:UnityEngine.SubsystemManager).
        /// </summary>
        /// <param name="cinfo">Construction info for the descriptor.</param>
        public static void Register(Cinfo cinfo)
        {
            if (string.IsNullOrEmpty(cinfo.id))
            {
                throw new ArgumentException("Cannot create marker subsystem descriptor because id is invalid",
                    nameof(cinfo));
            }

            if (cinfo.providerType == null
                || !cinfo.providerType.IsSubclassOf(typeof(XRMarkerSubsystem.Provider)))
            {
                throw new ArgumentException("Cannot create marker subsystem descriptor because providerType is invalid",
                    nameof(cinfo));
            }

            if (cinfo.subsystemTypeOverride == null || (cinfo.subsystemTypeOverride != typeof(XRMarkerSubsystem)
                && !cinfo.subsystemTypeOverride.IsSubclassOf(typeof(XRMarkerSubsystem))))
            {
                throw new ArgumentException("Cannot create marker subsystem descriptor because subsystemTypeOverride is invalid",
                    nameof(cinfo));
            }

            SubsystemDescriptorStore.RegisterDescriptor(new XRMarkerSubsystemDescriptor(cinfo));
        }

        /// <summary>
        /// Constructor parameters for the <see cref="XRMarkerSubsystemDescriptor"/>.
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
            /// The <see cref="XRMarkerSubsystem"/>-derived type to use for instantiation. The instantiated instance of
            /// this type will forward cast calls to its provider.
            /// </summary>
            /// <value>The subsystem implementation type.
            /// If <see langword="null"/>, <see cref="XRMarkerSubsystem"/> will be instantiated.</value>
            public Type subsystemTypeOverride { get; set; }

            /// <summary>
            /// Specifies the supported marker types for marker detection.
            /// </summary>
            public Func<Result<ReadOnlyListSpan<XRMarkerType>>> supportedMarkerTypesDelegate { get; set; }

            /// <summary>
            /// Tests for equality.
            /// </summary>
            /// <param name="other">The other <see cref="Cinfo"/> to compare against.</param>
            /// <returns>`true` if every field in <paramref name="other"/> is equal to this <see cref="Cinfo"/>.
            /// Otherwise, `false`.</returns>
            public bool Equals(Cinfo other)
            {
                return
                    id == other.id &&
                    providerType == other.providerType &&
                    subsystemTypeOverride == other.subsystemTypeOverride &&
                    supportedMarkerTypesDelegate == other.supportedMarkerTypesDelegate;
            }

            /// <summary>
            /// Tests for equality.
            /// </summary>
            /// <param name="obj">The `object` to compare against.</param>
            /// <returns>`true` if <paramref name="obj"/> is of type <see cref="Cinfo"/> and
            /// <see cref="Equals(Cinfo)"/> also returns `true`.
            /// Otherwise, `false`.</returns>
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
                return HashCode.Combine(id, providerType, subsystemTypeOverride, supportedMarkerTypesDelegate);
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
    }
}
