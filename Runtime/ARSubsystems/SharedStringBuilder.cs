using System.Text;

namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// A shared <c>StringBuilder</c>, necessary because certain objects must remain blittable
    /// and thus cannot have a <c>StringBuilder</c> member field.
    /// </summary>
    internal static class SharedStringBuilder
    {
        internal static StringBuilder instance { get; } = new();
    }
}
