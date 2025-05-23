using System;
using Unity.Collections;

namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// A <c>Guid</c> that can be serialized by Unity. The 128-bit <c>Guid</c>
    /// is stored as two 64-bit <c>ulong</c>s. Refer to the creation utility at
    /// <see cref="UnityEditor.XR.ARSubsystems.SerializableGuidUtil"/> for more information.
    /// </summary>
    [Serializable]
    public struct SerializableGuid : IEquatable<SerializableGuid>
    {
        [SerializeField]
        ulong m_GuidLow;

        internal ulong guidLow => m_GuidLow;

        [SerializeField]
        ulong m_GuidHigh;

        internal ulong guidHigh => m_GuidHigh;

        /// <summary>
        /// Constructs a <see cref="SerializableGuid"/> from two 64-bit <c>ulong</c>s.
        /// </summary>
        /// <param name="guidLow">The low 8 bytes of the <c>Guid</c>.</param>
        /// <param name="guidHigh">The high 8 bytes of the <c>Guid</c>.</param>
        public SerializableGuid(ulong guidLow, ulong guidHigh)
        {
            m_GuidLow = guidLow;
            m_GuidHigh = guidHigh;
        }

        /// <summary>
        /// Constructs a <see cref="SerializableGuid"/> from a <see cref="System.Guid"/>.
        /// </summary>
        /// <param name="guid">The <c>Guid</c> used to create the <c>SerializableGuid</c></param>
        public SerializableGuid(Guid guid)
        {
            var guidParts = GuidUtil.Decompose(guid);
            m_GuidLow = guidParts.low;
            m_GuidHigh = guidParts.high;
        }

        static readonly SerializableGuid k_Empty = new SerializableGuid(0, 0);

        /// <summary>
        /// Used to represent <c>System.Guid.Empty</c> (that is, a GUID whose values are all zeros).
        /// </summary>
        public static SerializableGuid empty => k_Empty;

        /// <summary>
        /// Reconstructs the <c>Guid</c> from the serialized data.
        /// </summary>
        public Guid guid => GuidUtil.Compose(m_GuidLow, m_GuidHigh);

        /// <summary>
        /// Convert the SerializableGuid to a NativeArray of bytes.
        /// </summary>
        /// <param name="allocator">The allocation strategy for the returned `NativeArray`.</param>
        /// <returns>The byte `NativeArray`.</returns>
        public NativeArray<byte> AsByteNativeArray(Allocator allocator = Allocator.Temp)
        {
            var nativeArray = new NativeArray<byte>(16, allocator);
            guid.TryWriteBytes(nativeArray.AsSpan());
            return nativeArray;
        }

        /// <summary>
        /// Convert from <see cref="TrackableId"/> to `SerializableGuid` using the <see cref="SerializableGuid(ulong, ulong)"/> constructor.
        /// </summary>
        /// <param name="trackableId">The TrackableId to convert.</param>
        /// <returns>The SerializableGuid.</returns>
        public static implicit operator SerializableGuid(TrackableId trackableId)
        {
            return new SerializableGuid(trackableId.subId1, trackableId.subId2);
        }

        /// <summary>
        /// Generates a hash suitable for use with containers like `HashSet` and `Dictionary`.
        /// </summary>
        /// <returns>A hash code generated from this object's fields.</returns>
        public override int GetHashCode() => HashCodeUtil.Combine(m_GuidLow.GetHashCode(), m_GuidHigh.GetHashCode());

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <param name="obj">The `object` to compare against.</param>
        /// <returns>`True` if <paramref name="obj"/> is of type <see cref="SerializableGuid"/> and
        /// <see cref="Equals(SerializableGuid)"/> also returns `true`; otherwise `false`.</returns>
        public override bool Equals(object obj) => (obj is SerializableGuid) && Equals((SerializableGuid)obj);

        /// <summary>
        /// Generates a string representation of the <c>Guid</c>. Same as <see cref="guid"/><c>.ToString()</c>.
        /// See <a href="https://docs.microsoft.com/en-us/dotnet/api/system.guid.tostring?view=netframework-4.7.2#System_Guid_ToString">Microsoft's documentation</a>
        /// for more details.
        /// </summary>
        /// <returns>A string representation of the <c>Guid</c>.</returns>
        public override string ToString()
        {
            return $"{m_GuidLow:X16}-{m_GuidHigh:X16}";
        }

        /// <summary>
        /// Generates a string representation of the <c>Guid</c>. Same as <see cref="guid"/><c>.ToString(format)</c>.
        /// </summary>
        /// <param name="format">A single format specifier that indicates how to format the value of the <c>Guid</c>.
        /// See <a href="https://docs.microsoft.com/en-us/dotnet/api/system.guid.tostring?view=netframework-4.7.2#System_Guid_ToString_System_String_">Microsoft's documentation</a>
        /// for more details.</param>
        /// <returns>A string representation of the <c>Guid</c>.</returns>
        public string ToString(string format) => guid.ToString(format);

        /// <summary>
        /// Generates a string representation of the <c>Guid</c>. Same as <see cref="guid"/><c>.ToString(format, provider)</c>.
        /// </summary>
        /// <param name="format">A single format specifier that indicates how to format the value of the <c>Guid</c>.
        /// See <a href="https://docs.microsoft.com/en-us/dotnet/api/system.guid.tostring?view=netframework-4.7.2#System_Guid_ToString_System_String_System_IFormatProvider_">Microsoft's documentation</a>
        /// for more details.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>A string representation of the <c>Guid</c>.</returns>
        public string ToString(string format, IFormatProvider provider) => guid.ToString(format, provider);

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <param name="other">The other <see cref="SerializableGuid"/> to compare against.</param>
        /// <returns>`True` if every field in <paramref name="other"/> is equal to this <see cref="SerializableGuid"/>, otherwise false.</returns>
        public bool Equals(SerializableGuid other)
        {
            return
                (m_GuidLow == other.m_GuidLow) &&
                (m_GuidHigh == other.m_GuidHigh);
        }

        /// <summary>
        /// Tests for equality. Same as <see cref="Equals(SerializableGuid)"/>.
        /// </summary>
        /// <param name="lhs">The left-hand side of the comparison.</param>
        /// <param name="rhs">The right-hand side of the comparison.</param>
        /// <returns>`True` if <paramref name="lhs"/> is equal to <paramref name="rhs"/>, otherwise `false`.</returns>
        public static bool operator ==(SerializableGuid lhs, SerializableGuid rhs) => lhs.Equals(rhs);

        /// <summary>
        /// Tests for inequality. Same as `!`<see cref="Equals(SerializableGuid)"/>.
        /// </summary>
        /// <param name="lhs">The left-hand side of the comparison.</param>
        /// <param name="rhs">The right-hand side of the comparison.</param>
        /// <returns>`True` if <paramref name="lhs"/> is not equal to <paramref name="rhs"/>, otherwise `false`.</returns>
        public static bool operator !=(SerializableGuid lhs, SerializableGuid rhs) => !lhs.Equals(rhs);
    }
}
