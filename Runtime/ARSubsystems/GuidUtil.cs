using System;

namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// Utility for dealing with <c>Guid</c>s.
    /// </summary>
    public static class GuidUtil
    {
        /// <summary>
        /// Reconstructs a [Guid](xref:System.Guid) from two <c>ulong</c>s representing the low and high bytes.
        /// </summary>
        /// <param name="low">The low 8 bytes of the guid</param>
        /// <param name="high">The high 8 bytes of the guid.</param>
        /// <returns>The Guid composed of <paramref name="low"/> and <paramref name="high"/>.</returns>
        public static Guid Compose(ulong low, ulong high)
        {
            return new Guid(
                (uint)((low   & 0x00000000ffffffff) >> 0),
                (ushort)((low & 0x0000ffff00000000) >> 32),
                (ushort)((low & 0xffff000000000000) >> 48),
                (byte)((high  & 0x00000000000000ff) >> 0),
                (byte)((high  & 0x000000000000ff00) >> 8),
                (byte)((high  & 0x0000000000ff0000) >> 16),
                (byte)((high  & 0x00000000ff000000) >> 24),
                (byte)((high  & 0x000000ff00000000) >> 32),
                (byte)((high  & 0x0000ff0000000000) >> 40),
                (byte)((high  & 0x00ff000000000000) >> 48),
                (byte)((high  & 0xff00000000000000) >> 56));
        }

        struct GuidParts
        {
            public ulong low;
            public ulong high;
        }

        internal static (ulong low, ulong high) Decompose(Guid guid)
        {
            unsafe
            {
                var parts = *(GuidParts*)&guid;
                return (parts.low, parts.high);
            }
        }
    }
}
