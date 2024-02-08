using System;

namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// This struct uses an integer to represent the status of a completed operation.
    /// </summary>
    public struct XRResultStatus : IEquatable<XRResultStatus>, IComparable<XRResultStatus>
    {
        /// <summary>
        /// Get the integer value of this struct. The integer value has these meanings:
        /// <list type="table">
        ///   <listheader>
        ///     <term>Value</term>
        ///     <term>Meaning</term>
        ///   </listheader>
        ///   <item>
        ///     <description>Less than zero</description>
        ///     <description>The operation failed with an error.</description>
        ///   </item>
        ///   <item>
        ///     <description>Zero</description>
        ///     <description>The operation was an unqualified success.</description>
        ///   </item>
        ///   <item>
        ///     <description>Greater than zero</description>
        ///     <description>The operation succeeded but additional status information is available.</description>
        ///   </item>
        /// </list>
        /// </summary>
        /// <remarks>
        /// Platforms may assign integer values to specific error codes and success codes associated with that platform.
        /// Refer to the provider plug-in documentation for your target platform(s) to further understand how to
        /// interpret this integer value on each platform.
        /// </remarks>
        public int value => m_Value;
        int m_Value;

        /// <summary>
        /// Construct an instance with a given integer value.
        /// </summary>
        /// <param name="value">The integer value.</param>
        public XRResultStatus(int value)
        {
            m_Value = value;
        }

        /// <summary>
        /// Construct an instance from a boolean value.
        /// </summary>
        /// <param name="wasSuccessful">If <see langword="true"/>, assigns a <see cref="value"/> of zero.
        /// Otherwise, assigns -1.</param>
        public XRResultStatus(bool wasSuccessful)
        {
            m_Value = wasSuccessful ? 0 : -1;
        }

        /// <summary>
        /// Indicates whether the operation was an unqualified success. In other words, returns <see langword="true"/>
        /// if the operation succeeded and no additional status information is available.
        /// </summary>
        /// <returns><see langword="true"/> if the operation was an unqualified success. Otherwise, <see langword="false"/>.</returns>
        public bool IsUnqualifiedSuccess() => m_Value == 0;

        /// <summary>
        /// Indicates whether the operation was successful, inclusive of all success codes. If <see cref="value"/> is
        /// greater than 0, additional status information may be available. Refer to your provider plug-in documentation
        /// for more information about the possible success codes on that platform.
        /// </summary>
        /// <remarks>
        /// Equivalent to both `!IsError()` and implicitly converting this instance to <see cref="bool"/>.
        /// </remarks>
        /// <returns><see langword="true"/> if the operation was successful. Otherwise, <see langword="false"/>.</returns>
        public bool IsSuccess() => m_Value >= 0;

        /// <summary>
        /// Indicates whether the operation failed with an error.
        /// </summary>
        /// <remarks>
        /// Equivalent to `!IsSuccess()`.
        /// </remarks>
        /// <returns><see langword="true"/> if the operation failed with error. Otherwise, <see langword="false"/>.</returns>
        public bool IsError() => m_Value < 0;

        /// <summary>
        /// Convert from <see cref="int"/> to `XRResultStatus`.
        /// </summary>
        /// <param name="value">The integer value.</param>
        /// <returns>The status.</returns>
        public static implicit operator XRResultStatus(int value)
        {
            return new XRResultStatus(value);
        }

        /// <summary>
        /// Convert from `XRResultStatus` to <see cref="int"/>.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <returns>The integer value.</returns>
        public static implicit operator int(XRResultStatus status)
        {
            return status.value;
        }

        /// <summary>
        /// Convert from <see cref="bool"/> to `XRResultStatus`.
        /// </summary>
        /// <param name="wasSuccessful">Whether the operation was successful.</param>
        /// <returns>The status.</returns>
        public static implicit operator XRResultStatus(bool wasSuccessful)
        {
            return new XRResultStatus(wasSuccessful);
        }

        /// <summary>
        /// Convert from `XRResultStatus` to <see cref="bool"/>.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <returns><see langword="true"/> if the operation was successful. Otherwise, <see langword="false"/>.</returns>
        public static implicit operator bool(XRResultStatus status)
        {
            return status.value >= 0;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">The other object.</param>
        /// <returns><see langword="true"/> if the objects are equal. Otherwise, <see langword="false"/>.</returns>
        public bool Equals(XRResultStatus other)
        {
            return m_Value == other.value;
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates
        /// whether the current instance precedes, follows, or occurs in the same position in the sort order as the
        /// other object. `XRResultStatus` objects are sorted by their <see cref="value"/>.
        /// </summary>
        /// <param name="other">An object to compare with this instance.</param>
        /// <returns>A value that indicates the relative order of the objects being compared. The return value has these
        /// meanings:
        /// <list type="table">
        ///   <listheader>
        ///     <term>Value</term>
        ///     <term>Meaning</term>
        ///   </listheader>
        ///   <item>
        ///     <description>Less than zero</description>
        ///     <description>The instance precedes <paramref name="other"/> in the sort order.</description>
        ///   </item>
        ///   <item>
        ///     <description>Zero</description>
        ///     <description>The instance occurs in the same position in the sort order as <paramref name="other"/>.</description>
        ///   </item>
        ///   <item>
        ///     <description>Greater than zero</description>
        ///     <description>This instance follows <paramref name="other"/> in the sort order.</description>
        ///   </item>
        /// </list>
        /// </returns>
        public int CompareTo(XRResultStatus other)
        {
            return value.CompareTo(other.value);
        }
    }
}
