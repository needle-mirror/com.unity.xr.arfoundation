using System.Text;

namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// A shared <c>StringBuilder</c>, necessary because certain objects must remain blittable
    /// and thus cannot have a <c>StringBuilder</c> member field.
    /// </summary>
    internal static class SharedStringBuilder
    {
        static StringBuilder s_StringBuilder = new();
        internal static StringBuilder stringBuilder => s_StringBuilder;
    }
}
