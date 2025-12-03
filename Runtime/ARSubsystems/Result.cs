using System;
using System.Collections.Generic;
using static UnityEngine.XR.ARSubsystems.XRResultStatus;

namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// Represents the result of a completed operation that attempted to create an object of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The result type.</typeparam>
    public struct Result<T> : IEquatable<XRResultStatus>, IEquatable<Result<T>>
    {
        /// <summary>
        /// The status of the completed operation. You should check whether the operation was successful before you
        /// access the result <see cref="value"/>.
        /// </summary>
        public XRResultStatus status => m_Status;
        XRResultStatus m_Status;

        /// <summary>
        /// The result value of the completed operation. Only valid if <see cref="XRResultStatus.IsSuccess">status.IsSuccess()</see>
        /// is <see langword="true"/>.
        /// </summary>
        /// <remarks>
        /// > [!IMPORTANT]
        /// > If the operation was unsuccessful, you should not access this value. It may be <see langword="null"/> or
        /// > could contain default data.
        /// </remarks>
        public T value => m_Value;
        T m_Value;

        /// <summary>
        /// Construct an instance with a given status and value.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="value">The result value.</param>
        public Result(XRResultStatus status, T value)
        {
            m_Status = status;
            m_Value = value;
        }

        /// <summary>
        /// Construct a result containing an unqualified success status code and the given value.
        /// </summary>
        /// <param name="value">The value.</param>
        public Result(T value)
        {
            m_Status = new XRResultStatus(StatusCode.UnqualifiedSuccess);
            m_Value = value;
        }

        /// <summary>
        /// Compare for equality with another instance.
        /// </summary>
        /// <param name="other">The other instance.</param>
        /// <returns>`true` if the instances are equal. Otherwise, `false`.</returns>
        public bool Equals(Result<T> other)
        {
            return m_Status.Equals(other.m_Status) && EqualityComparer<T>.Default.Equals(m_Value, other.m_Value);
        }

        /// <summary>
        /// Compares for equality with an `XRResultStatus` instance.
        /// </summary>
        /// <param name="other">The `XRResultStatus`.</param>
        /// <returns>`true` if the instances are equal. Otherwise, `false`.</returns>
        /// <remarks>
        /// A `Result&lt;T&gt;` compares equal to an `XRResultStatus` only if it represents an error, and its
        /// <see cref="status"/> value is equal to the given `XRResultStatus`.
        /// </remarks>
        public bool Equals(XRResultStatus other)
        {
            return m_Status.IsError() && m_Status.Equals(other);
        }

        /// <summary>
        /// Compare for equality with another object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>`true` if <paramref name="obj"/> is either of type `Result&lt;T&gt;` or `XRResultStatus`,
        /// and compares equal using one of the other overloads of this function.</returns>
        public override bool Equals(object obj)
        {
            return (obj is Result<T> otherResult && Equals(otherResult))
                || (obj is XRResultStatus otherStatus && Equals(otherStatus));
        }

        /// <summary>
        /// Get a hash code for this instance.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(m_Status, m_Value);
        }

        /// <summary>
        /// Returns a string suitable for debugging purposes.
        /// </summary>
        /// <returns>The string.</returns>
        public override string ToString()
        {
            return $"{{\n  status: {status.ToString()}\n  value: {value.ToString()}\n}}";
        }
    }
}
