using System;

namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// Represents the status of a completed operation as a cross-platform <see cref="statusCode"/> and a
    /// platform-specific <see cref="nativeStatusCode"/>.
    /// </summary>
    public struct XRResultStatus : IEquatable<XRResultStatus>, IComparable<XRResultStatus>
    {
        /// <summary>
        /// Indicates whether the operation succeeded or failed as well as whether additional status information is
        /// available in <see cref="XRResultStatus.nativeStatusCode"/>.
        /// </summary>
        /// <remarks>
        /// The integer value of this enum has the following meanings:
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
        ///     <description>The operation succeeded.</description>
        ///   </item>
        /// </list>
        /// </remarks>
        public enum StatusCode
        {
            /// <summary>
            /// Indicates that the operation was successful, and additional information is available in
            /// <see cref="XRResultStatus.nativeStatusCode"/>.
            /// </summary>
            PlatformQualifiedSuccess = 1,

            /// <summary>
            /// Indicates that the operation was successful, and no additional information is available.
            /// </summary>
            UnqualifiedSuccess = 0,

            /// <summary>
            /// Indicates that the operation failed with an error, and additional information is available in
            /// <see cref="XRResultStatus.nativeStatusCode"/>.
            /// </summary>
            PlatformError = -1,

            /// <summary>
            /// Indicates that the operation failed with an unknown error, and no additional information is available.
            /// </summary>
            UnknownError = -2,
        }

        /// <summary>
        /// The cross-platform status code.
        /// </summary>
        /// <value>The status code.</value>
        public StatusCode statusCode => m_StatusCode;
        StatusCode m_StatusCode;

        /// <summary>
        /// The platform-specific status code.
        ///
        /// If <see cref="statusCode"/> is <see cref="StatusCode.PlatformQualifiedSuccess"/> or
        /// <see cref="StatusCode.PlatformError"/>, this property contains platform-specific status information
        /// that you can use to help debug issues on that platform. Refer to the provider plug-in documentation for your
        /// target platform(s) to further understand how to interpret this integer value.
        /// </summary>
        public int nativeStatusCode => m_NativeStatusCode;
        int m_NativeStatusCode;

        /// <summary>
        /// Construct an instance with a given status code. This constructor assumes that there is no platform-specific
        /// status information.
        /// </summary>
        /// <param name="statusCode">The platform-agnostic status code.</param>
        public XRResultStatus(StatusCode statusCode)
        {
            m_StatusCode = statusCode;
            m_NativeStatusCode = default;
        }

        /// <summary>
        /// Construct an instance with a given native status code.
        ///
        /// This constructor assumes that the integer value of the status code follows OpenXR conventions:
        /// <list type="table">
        ///   <listheader>
        ///     <term>Value</term>
        ///     <term>Meaning</term>
        ///   </listheader>
        ///   <item>
        ///     <description>Less than zero</description>
        ///     <description>
        ///       The operation failed with an error, and additional status information is available.
        ///       <see cref="statusCode"/> will be assigned the value <see cref="StatusCode.PlatformError"/>.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>Zero</description>
        ///     <description>
        ///       The operation was an unqualified success.
        ///       <see cref="statusCode"/> will be assigned the value <see cref="StatusCode.UnqualifiedSuccess"/>.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>Greater than zero</description>
        ///     <description>
        ///       The operation succeeded, and additional status information is available.
        ///       <see cref="statusCode"/> will be assigned the value <see cref="StatusCode.PlatformQualifiedSuccess"/>.
        ///     </description>
        ///   </item>
        /// </list>
        ///
        /// If your platform's native status code does not follow these conventions, use the
        /// <see cref="XRResultStatus(StatusCode, int)"/> constructor instead.
        /// </summary>
        /// <param name="nativeStatusCode">The integer value.</param>
        public XRResultStatus(int nativeStatusCode)
        {
            m_NativeStatusCode = nativeStatusCode;
            m_StatusCode = nativeStatusCode switch
            {
                > 0 => StatusCode.PlatformQualifiedSuccess,
                0 => StatusCode.UnqualifiedSuccess,
                < 0 => StatusCode.PlatformError
            };
        }

        /// <summary>
        /// Construct an instance with a given status code and native status code.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <param name="nativeStatusCode">The native status code.</param>
        public XRResultStatus(StatusCode statusCode, int nativeStatusCode)
        {
            m_StatusCode = statusCode;
            m_NativeStatusCode = nativeStatusCode;
        }

        /// <summary>
        /// Construct an instance from a boolean value.
        /// </summary>
        /// <param name="wasSuccessful">
        /// If <see langword="true"/>, assigns a <see cref="statusCode"/> of <see cref="StatusCode.UnqualifiedSuccess"/>.
        /// Otherwise, assigns <see cref="StatusCode.UnknownError"/>
        /// </param>
        public XRResultStatus(bool wasSuccessful)
        {
            m_StatusCode = wasSuccessful ? StatusCode.UnqualifiedSuccess : StatusCode.UnknownError;
            m_NativeStatusCode = default;
        }

        /// <summary>
        /// Indicates whether the operation was an unqualified success. In other words, returns <see langword="true"/>
        /// if the operation succeeded and no additional status information is available.
        /// </summary>
        /// <returns><see langword="true"/> if the operation was an unqualified success. Otherwise, <see langword="false"/>.</returns>
        public bool IsUnqualifiedSuccess() => m_StatusCode == StatusCode.UnqualifiedSuccess;

        /// <summary>
        /// Indicates whether the operation was successful, inclusive of all success codes and
        /// <see cref="StatusCode.UnqualifiedSuccess"/>.
        /// </summary>
        /// <remarks>
        /// Equivalent to both `!IsError()` and implicitly converting this instance to <see cref="bool"/>.
        /// </remarks>
        /// <returns><see langword="true"/> if the operation was successful. Otherwise, <see langword="false"/>.</returns>
        public bool IsSuccess() => m_StatusCode >= 0;

        /// <summary>
        /// Indicates whether the operation failed with an error.
        /// </summary>
        /// <remarks>
        /// Equivalent to `!IsSuccess()`.
        /// </remarks>
        /// <returns><see langword="true"/> if the operation failed with error. Otherwise, <see langword="false"/>.</returns>
        public bool IsError() => m_StatusCode < 0;

        /// <summary>
        /// Convert from <see cref="bool"/> to `XRResultStatus` using the <see cref="XRResultStatus(bool)"/> constructor.
        /// </summary>
        /// <param name="wasSuccessful">Whether the operation was successful.</param>
        /// <returns>The status.</returns>
        public static implicit operator XRResultStatus(bool wasSuccessful)
        {
            return new XRResultStatus(wasSuccessful);
        }

        /// <summary>
        /// Convert from `XRResultStatus` to <see cref="bool"/> using <see cref="XRResultStatus.IsSuccess"/>.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <returns><see langword="true"/> if the operation was successful. Otherwise, <see langword="false"/>.</returns>
        public static implicit operator bool(XRResultStatus status)
        {
            return status.IsSuccess();
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type. Equality is compared using
        /// both <see cref="statusCode"/> and <see cref="nativeStatusCode"/>.
        /// </summary>
        /// <param name="other">The other object.</param>
        /// <returns><see langword="true"/> if the objects are equal. Otherwise, <see langword="false"/>.</returns>
        public bool Equals(XRResultStatus other)
        {
            return m_StatusCode == other.statusCode && m_NativeStatusCode == other.nativeStatusCode;
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates
        /// whether the current instance precedes, follows, or occurs in the same position in the sort order as the
        /// other object.
        ///
        /// `XRResultStatus` objects are sorted first by their <see cref="nativeStatusCode"/>, then by their
        /// <see cref="statusCode"/>.
        /// </summary>
        /// <param name="other">An object to compare with this instance.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has these meanings:
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
            var nativeComparison = nativeStatusCode.CompareTo(other.nativeStatusCode);
            return nativeComparison == 0 ? statusCode.CompareTo(other.statusCode) : nativeComparison;
        }
    }
}
