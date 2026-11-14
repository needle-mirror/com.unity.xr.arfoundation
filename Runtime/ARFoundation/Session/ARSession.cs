using System;

namespace UnityEngine.XR.ARFoundation
{
    /// <summary>
    /// Controls the lifecycle and configuration options for an AR session.
    /// </summary>
    /// <remarks>
    /// `ARSession` has been renamed to <see cref="ARSessionManager"/>. The Unity API Updater automatically replaces
    /// references to this type with `ARSessionManager` when you open your project. Scenes and prefabs that contain
    /// an AR Session component don't need to change, because the component keeps its script reference.
    /// </remarks>
    [Obsolete("ARSession has been renamed in AR Foundation version 7.0. Use ARSessionManager instead. (UnityUpgradable) -> UnityEngine.XR.ARFoundation.ARSessionManager", true)]
    [AddComponentMenu("")]
    public sealed class ARSession : MonoBehaviour
    {
    }
}
