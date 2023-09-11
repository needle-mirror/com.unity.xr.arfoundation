namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// Represents the result of an operation.
    /// </summary>
    /// <typeparam name="T">The result type.</typeparam>
    public struct Result<T>
    {
        bool m_WasSuccessful;
        T m_Result;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="wasSuccessful"><see langword="true"/> if the operation was successful. Otherwise, <see langword="false"/>.</param>
        /// <param name="result">The result. Only valid if <paramref name="wasSuccessful"/> is <see langword="true"/>.</param>
        public Result(bool wasSuccessful, T result)
        {
            m_WasSuccessful = wasSuccessful;
            m_Result = result;
        }

        /// <summary>
        /// Attempts to get the result of the operation. If this method returns <see langword="false"/>, the
        /// operation failed, and the <paramref name="result"/> value is invalid.
        /// </summary>
        /// <param name="result">The result. Only valid if this method returns <see langword="true"/>.</param>
        /// <returns><see langword="true"/> if the async operation was successful. Otherwise, <see langword="false"/>.</returns>
        public bool TryGetResult(out T result)
        {
            result = m_Result;
            return m_WasSuccessful;
        }
    }
}
