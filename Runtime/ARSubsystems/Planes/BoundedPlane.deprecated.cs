using System;

namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// The session-relative data associated with a plane.
    /// </summary>
    /// <remarks>
    /// `BoundedPlane` has been renamed to <see cref="XRPlane"/> so that every AR Foundation trackable type uses the
    /// `XR` prefix. The Unity API Updater automatically replaces references to this type with `XRPlane` when you
    /// open your project.
    /// </remarks>
    [Obsolete("BoundedPlane has been renamed in AR Foundation version 7.0. Use XRPlane instead. (UnityUpgradable) -> UnityEngine.XR.ARSubsystems.XRPlane", true)]
    public struct BoundedPlane
    {
    }

    /// <summary>
    /// Defines a fluent API for constructing a plane.
    /// </summary>
    /// <remarks>
    /// `BoundedPlaneBuilder` has been renamed to <see cref="XRPlaneBuilder"/>. The Unity API Updater automatically
    /// replaces references to this type with `XRPlaneBuilder` when you open your project.
    /// </remarks>
    [Obsolete("BoundedPlaneBuilder has been renamed in AR Foundation version 7.0. Use XRPlaneBuilder instead. (UnityUpgradable) -> UnityEngine.XR.ARSubsystems.XRPlaneBuilder", true)]
    public class BoundedPlaneBuilder
    {
    }
}
