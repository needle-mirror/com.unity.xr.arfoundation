using System.Text;
#if UNITY_6000_5_OR_NEWER
using Unity.Scripting.LifecycleManagement;
#endif

namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// A shared <c>StringBuilder</c>, necessary because certain objects must remain blittable
    /// and thus cannot have a <c>StringBuilder</c> member field.
    /// </summary>
    internal static class SharedStringBuilder
    {
#if UNITY_6000_5_OR_NEWER
        [NoAutoStaticsCleanup]
#endif
        internal static StringBuilder instance { get; } = new();
    }
}
