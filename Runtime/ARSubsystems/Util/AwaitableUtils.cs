using System.Runtime.CompilerServices;

namespace UnityEngine.XR.ARSubsystems
{
    static class AwaitableUtils<T>
    {
        /// <summary>
        /// An `Awaitable` equivalent to C#'s `Task.FromResult`.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Awaitable<T> FromResult(
            AwaitableCompletionSource<T> completionSource, T result)
        {
            var awaitable = completionSource.Awaitable;
            completionSource.SetResult(result);
            completionSource.Reset();
            return awaitable;
        }
    }
}
