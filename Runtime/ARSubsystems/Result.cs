namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// Represents the result of a completed operation that attempted to create an object of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The result type.</typeparam>
    public struct Result<T>
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
    }
}
