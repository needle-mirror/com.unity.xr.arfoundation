using System;
using System.Threading;
using UnityEngine.SubsystemsImplementation;

namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// Indicates the capabilities supported by a provider of the <see cref="XRAnchorSubsystem"/>. Provider
    /// implementations must derive from <see cref="XRAnchorSubsystem.Provider"/> and may override virtual class members.
    /// </summary>
    public class XRAnchorSubsystemDescriptor :
        SubsystemDescriptorWithProvider<XRAnchorSubsystem, XRAnchorSubsystem.Provider>
    {
        /// <summary>
        /// Contains the parameters necessary to construct a new <see cref="XRAnchorSubsystemDescriptor"/> instance.
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
            /// The <see cref="XRAnchorSubsystem"/>-derived type to use for instantiation. The instantiated instance of
            /// this type will forward casted calls to its provider.
            /// </summary>
            /// <value>The subsystem implementation type.
            /// If <see langword="null"/>, <see cref="XRAnchorSubsystem"/> will be instantiated.</value>
            public Type subsystemTypeOverride { get; set; }

            /// <summary>
            /// Indicates whether the provider implementation supports attachments (that is, the ability to attach an anchor to a trackable).
            /// If <see langword="false"/>, <see cref="XRAnchorSubsystem.TryAttachAnchor">XRAnchorSubsystem.TryAttachAnchor</see>
            /// must always return <see langword="false"/>.
            /// </summary>
            public bool supportsTrackableAttachments { get; set; }

            /// <summary>
            /// Indicates whether the provider implementation supports synchronously adding anchors via
            /// <see cref="XRAnchorSubsystem.TryAddAnchor">XRAnchorSubsystem.TryAddAnchor</see>.
            /// If <see langword="false"/>, `TryAddAnchor` must always return false. In this case, use
            /// <see cref="XRAnchorSubsystem.TryAddAnchorAsync">XRAnchorSubsystem.TryAddAnchorAsync</see> instead.
            /// </summary>
            public bool supportsSynchronousAdd { get; set; }

            /// <summary>
            /// Indicates whether the provider implementation supports the ability to persistently save anchors via
            /// <see cref="XRAnchorSubsystem.TrySaveAnchorAsync">XRAnchorSubsystem.TrySaveAnchorAsync</see>.
            /// </summary>
            [Obsolete("supportsSaveAnchor is deprecated in AR Foundation 6.4.0. Use supportsSaveAnchorDelegate instead.")]
            public bool supportsSaveAnchor { get; set; }

            /// <summary>
            /// Indicates whether the provider implementation supports the ability to load persistently saved anchors via
            /// <see cref="XRAnchorSubsystem.TryLoadAnchorAsync">XRAnchorSubsystem.TryLoadAnchorAsync</see>.
            /// </summary>
            [Obsolete("supportsLoadAnchor is deprecated in AR Foundation 6.4.0. Use supportsLoadAnchorDelegate instead.")]
            public bool supportsLoadAnchor { get; set; }

            /// <summary>
            /// Indicates whether the provider implementation supports the ability to erase the persistent saved data
            /// associated with an anchor via
            /// <see cref="XRAnchorSubsystem.TryEraseAnchorAsync">XRAnchorSubsystem.TryEraseAnchorAsync</see>.
            /// </summary>
            [Obsolete("supportsEraseAnchor is deprecated in AR Foundation 6.4.0. Use supportsEraseAnchorDelegate instead.")]
            public bool supportsEraseAnchor { get; set; }

            /// <summary>
            /// Indicates whether the provider implementation supports the ability to get all saved persistent anchor GUIDs
            /// via <see cref="XRAnchorSubsystem.TryGetSavedAnchorIdsAsync">XRAnchorSubsystem.TryGetSavedAnchorIdsAsync</see>.
            /// </summary>
            [Obsolete("supportsGetSavedAnchorIds is deprecated in AR Foundation 6.4.0. Use supportsGetSavedAnchorIdsDelegate instead.")]
            public bool supportsGetSavedAnchorIds { get; set; }

            /// <summary>
            /// Indicates whether the provider implementation supports cancelling async operations in progress using
            /// <see cref="CancellationToken"/>s. If <see langword="false"/>, <see cref="XRAnchorSubsystem"/> APIs that
            /// take a `CancellationToken` as input will ignore the input cancellation token.
            /// </summary>
            public bool supportsAsyncCancellation { get; set; }

            /// <summary>
            /// Indicates whether the provider implementation supports the ability to persistently save anchors via
            /// <see cref="XRAnchorSubsystem.TrySaveAnchorAsync">XRAnchorSubsystem.TrySaveAnchorAsync</see>.
            /// </summary>
            public Func<bool> supportsSaveAnchorDelegate { get; set; }

            /// <summary>
            /// Indicates whether the provider implementation supports the ability to load persistently saved anchors via
            /// <see cref="XRAnchorSubsystem.TryLoadAnchorAsync">XRAnchorSubsystem.TryLoadAnchorAsync</see>.
            /// </summary>
            public Func<bool> supportsLoadAnchorDelegate { get; set; }

            /// <summary>
            /// Indicates whether the provider implementation supports the ability to erase the persistent saved data
            /// associated with an anchor via
            /// <see cref="XRAnchorSubsystem.TryEraseAnchorAsync">XRAnchorSubsystem.TryEraseAnchorAsync</see>.
            /// </summary>
            public Func<bool> supportsEraseAnchorDelegate { get; set; }

            /// <summary>
            /// Indicates whether the provider implementation supports the ability to get all saved persistent anchor GUIDs
            /// via <see cref="XRAnchorSubsystem.TryGetSavedAnchorIdsAsync">XRAnchorSubsystem.TryGetSavedAnchorIdsAsync</see>.
            /// </summary>
            public Func<bool> supportsGetSavedAnchorIdsDelegate { get; set; }

            /// <summary>
            /// Tests for equality.
            /// </summary>
            /// <param name="obj">The `object` to compare against.</param>
            /// <returns>`true` if <paramref name="obj"/> is of type <see cref="Cinfo"/> and
            /// <see cref="Equals(Cinfo)"/> also returns `true`. Otherwise, `false`.</returns>
            public override bool Equals(object obj) => (obj is Cinfo other) && Equals(other);

            /// <summary>
            /// Generates a hash suitable for use with containers such as
            /// <see cref="System.Collections.Generic.HashSet{T}">HashSet</see>
            /// and <see cref="System.Collections.Generic.Dictionary{T1, T2}">Dictionary</see>.
            /// </summary>
            /// <returns>The hash code.</returns>
            public override int GetHashCode()
            {
                var hashCode = new HashCode();
                hashCode.Add(id);
                hashCode.Add(providerType);
                hashCode.Add(subsystemTypeOverride);
                hashCode.Add(supportsTrackableAttachments);
                hashCode.Add(supportsSynchronousAdd);
                hashCode.Add(supportsAsyncCancellation);
#pragma warning disable CS0618 // Type or member is obsolete
                hashCode.Add(supportsSaveAnchor);
                hashCode.Add(supportsLoadAnchor);
                hashCode.Add(supportsEraseAnchor);
                hashCode.Add(supportsGetSavedAnchorIds);
#pragma warning restore CS0618 // Type or member is obsolete
                hashCode.Add(supportsSaveAnchorDelegate);
                hashCode.Add(supportsLoadAnchorDelegate);
                hashCode.Add(supportsEraseAnchorDelegate);
                hashCode.Add(supportsGetSavedAnchorIdsDelegate);
                return hashCode.ToHashCode();
            }

            /// <summary>
            /// Tests for equality.
            /// </summary>
            /// <param name="other">The other <see cref="Cinfo"/> to compare against.</param>
            /// <returns>`true` if every field in <paramref name="other"/> is equal to this <see cref="Cinfo"/>.
            /// Otherwise, `false`.</returns>
            public bool Equals(Cinfo other)
            {
                return
                    string.Equals(id, other.id)
                    && ReferenceEquals(providerType, other.providerType)
                    && ReferenceEquals(subsystemTypeOverride, other.subsystemTypeOverride)
                    && supportsTrackableAttachments == other.supportsTrackableAttachments
                    && supportsSynchronousAdd == other.supportsSynchronousAdd
#pragma warning disable CS0618 // Type or member is obsolete
                    && supportsSaveAnchor == other.supportsSaveAnchor
                    && supportsLoadAnchor == other.supportsLoadAnchor
                    && supportsEraseAnchor == other.supportsEraseAnchor
                    && supportsGetSavedAnchorIds == other.supportsGetSavedAnchorIds
#pragma warning restore CS0618 // Type or member is obsolete
                    && supportsSaveAnchorDelegate == other.supportsSaveAnchorDelegate
                    && supportsLoadAnchorDelegate == other.supportsLoadAnchorDelegate
                    && supportsEraseAnchorDelegate == other.supportsEraseAnchorDelegate
                    && supportsGetSavedAnchorIdsDelegate == other.supportsGetSavedAnchorIdsDelegate
                    && supportsAsyncCancellation == other.supportsAsyncCancellation;
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

        bool m_SupportsSaveAnchorObsolete;
        bool m_SupportsLoadAnchorObsolete;
        bool m_SupportsEraseAnchorObsolete;
        bool m_SupportsGetSavedAnchorIdsObsolete;
        Func<bool> m_SupportsSaveAnchorDelegate;
        Func<bool> m_SupportsLoadAnchorDelegate;
        Func<bool> m_SupportsEraseAnchorDelegate;
        Func<bool> m_SupportsGetSavedAnchorIdsDelegate;

        /// <summary>
        /// Indicates whether the provider implementation supports the ability to attach an anchor to a trackable via
        /// <see cref="XRAnchorSubsystem.TryAttachAnchor">XRAnchorSubsystem.TryAttachAnchor</see>.
        /// If `false`, `TryAttachAnchor` must always return `false`.
        /// </summary>
        public bool supportsTrackableAttachments { get; }

        /// <summary>
        /// Indicates whether the provider implementation supports the ability to synchronously add anchors via
        /// <see cref="XRAnchorSubsystem.TryAddAnchor">XRAnchorSubsystem.TryAddAnchor</see>.
        /// If `false`, `TryAddAnchor` must always return false. In this case, use
        /// <see cref="XRAnchorSubsystem.TryAddAnchorAsync">XRAnchorSubsystem.TryAddAnchorAsync</see> instead.
        /// </summary>
        public bool supportsSynchronousAdd { get; }

        /// <summary>
        /// Indicates whether the provider implementation supports the ability to persistently save anchors via
        /// <see cref="XRAnchorSubsystem.TrySaveAnchorAsync">XRAnchorSubsystem.TrySaveAnchorAsync</see>.
        /// </summary>
        /// <value>
        /// If `false`, `TrySaveAnchorAsync` will throw a <see cref="NotImplementedException"/>.
        /// </value>
        public bool supportsSaveAnchor
            => m_SupportsSaveAnchorDelegate?.Invoke() ?? m_SupportsSaveAnchorObsolete;

        /// <summary>
        /// Indicates whether the provider implementation supports the ability to load persistently saved anchors via
        /// <see cref="XRAnchorSubsystem.TryLoadAnchorAsync">XRAnchorSubsystem.TryLoadAnchorAsync</see>.
        /// </summary>
        /// <value>
        /// If `false`, `TryLoadAnchorAsync` will throw a <see cref="NotImplementedException"/>.
        /// </value>
        public bool supportsLoadAnchor
            => m_SupportsLoadAnchorDelegate?.Invoke() ?? m_SupportsLoadAnchorObsolete;

        /// <summary>
        /// Indicates whether the provider implementation supports the ability to erase the persistent saved data
        /// associated with an anchor via
        /// <see cref="XRAnchorSubsystem.TryEraseAnchorAsync">XRAnchorSubsystem.TryEraseAnchorAsync</see>.
        /// </summary>
        /// <value>
        /// If `false`, `TryEraseAnchorAsync` will throw a <see cref="NotImplementedException"/>.
        /// </value>
        public bool supportsEraseAnchor
            => m_SupportsEraseAnchorDelegate?.Invoke() ?? m_SupportsEraseAnchorObsolete;

        /// <summary>
        /// Indicates whether the provider implementation supports the ability to get all saved persistent anchor GUIDs
        /// via <see cref="XRAnchorSubsystem.TryGetSavedAnchorIdsAsync">XRAnchorSubsystem.TryGetSavedAnchorIdsAsync</see>.
        /// </summary>
        /// <value>
        /// If `false`, `TryGetSavedAnchorIdsAsync` will throw a <see cref="NotImplementedException"/>.
        /// </value>
        public bool supportsGetSavedAnchorIds
            => m_SupportsGetSavedAnchorIdsDelegate?.Invoke() ?? m_SupportsGetSavedAnchorIdsObsolete;

        /// <summary>
        /// Indicates whether the provider implementation supports cancelling async operations in progress using a
        /// <see cref="CancellationToken"/>.
        /// </summary>
        /// <value>
        /// If `false`, <see cref="XRAnchorSubsystem"/> APIs that take a `CancellationToken` as input
        /// will ignore the input cancellation token.
        /// </value>
        public bool supportsAsyncCancellation { get; }

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
            SubsystemDescriptorStore.RegisterDescriptor(new XRAnchorSubsystemDescriptor(cinfo));
        }

        XRAnchorSubsystemDescriptor(Cinfo cinfo)
        {
            id = cinfo.id;
            providerType = cinfo.providerType;
            subsystemTypeOverride = cinfo.subsystemTypeOverride;
            supportsTrackableAttachments = cinfo.supportsTrackableAttachments;
            supportsSynchronousAdd = cinfo.supportsSynchronousAdd;
            supportsAsyncCancellation = cinfo.supportsAsyncCancellation;
            m_SupportsSaveAnchorDelegate = cinfo.supportsSaveAnchorDelegate;
            m_SupportsLoadAnchorDelegate = cinfo.supportsLoadAnchorDelegate;
            m_SupportsEraseAnchorDelegate = cinfo.supportsEraseAnchorDelegate;
            m_SupportsGetSavedAnchorIdsDelegate = cinfo.supportsGetSavedAnchorIdsDelegate;
#pragma warning disable CS0618 // Type or member is obsolete
            m_SupportsSaveAnchorObsolete = cinfo.supportsSaveAnchor;
            m_SupportsLoadAnchorObsolete = cinfo.supportsLoadAnchor;
            m_SupportsEraseAnchorObsolete = cinfo.supportsEraseAnchor;
            m_SupportsGetSavedAnchorIdsObsolete = cinfo.supportsGetSavedAnchorIds;
#pragma warning restore CS0618 // Type or member is obsolete
        }
    }
}
