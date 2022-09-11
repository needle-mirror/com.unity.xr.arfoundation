using System;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation
{
    /// <summary>
    /// Describes when the AR Camera Background should render.
    /// </summary>
    public enum CameraBackgroundRenderingMode : byte
    {
        /// <summary>
        /// Allows the platform to select its preferred render mode.
        /// </summary>
        Any,

        /// <summary>
        /// Perform background rendering prior to rendering opaque geometry in the scene.
        /// </summary>
        BeforeOpaques,

        /// <summary>
        /// Perform background rendering after opaques have been rendered.
        /// </summary>
        AfterOpaques
    }

    /// <summary>
    /// Provides conversion utilities between (xref: UnityEngine.XR.ARSubsystems.XRCameraBackgroundRenderingMode) and
    /// <see cref="CameraBackgroundRenderingMode"/>.
    /// </summary>
    public static class CameraBackgroundRenderingModeUtilities
    {
        /// <summary>
        /// Converts a <see cref="CameraBackgroundRenderingMode"/> to a (xref: UnityEngine.XR.ARSubsystems.XRCameraBackgroundRenderingMode).
        /// </summary>
        /// <param name="mode">
        /// The <see cref="CameraBackgroundRenderingMode"/> to convert to a (xref: UnityEngine.XR.ARSubsystems.XRCameraBackgroundRenderingMode).
        /// </param>
        /// <returns>
        /// The (xref: UnityEngine.XR.ARSubsystems.XRCameraBackgroundRenderingMode) conversion of the given <see cref="CameraBackgroundRenderingMode"/>.
        /// </returns>
        public static XRSupportedCameraBackgroundRenderingMode ToXRSupportedCameraBackgroundRenderingMode(this CameraBackgroundRenderingMode mode)
        {
            switch (mode)
            {
                case CameraBackgroundRenderingMode.Any:
                    return XRSupportedCameraBackgroundRenderingMode.Any;

                case CameraBackgroundRenderingMode.BeforeOpaques:
                    return XRSupportedCameraBackgroundRenderingMode.BeforeOpaques;

                case CameraBackgroundRenderingMode.AfterOpaques:
                    return XRSupportedCameraBackgroundRenderingMode.AfterOpaques;

                default:
                    return XRSupportedCameraBackgroundRenderingMode.None;
            }
        }

        /// <summary>
        /// Converts a (xref: UnityEngine.XR.ARSubsystems.XRCameraBackgroundRenderingMode) to a <see cref="CameraBackgroundRenderingMode"/>.
        /// </summary>
        /// <param name="mode">
        /// The (xref: UnityEngine.XR.ARSubsystems.XRCameraBackgroundRenderingMode) to convert to a <see cref="CameraBackgroundRenderingMode"/>.
        /// </param>
        /// <returns>
        /// The <see cref="CameraBackgroundRenderingMode"/> conversion of the given (xref: UnityEngine.XR.ARSubsystems.XRCameraBackgroundRenderingMode).
        /// </returns>
        public static CameraBackgroundRenderingMode ToBackgroundRenderingMode(this XRSupportedCameraBackgroundRenderingMode mode)
        {
            switch (mode)
            {
                case XRSupportedCameraBackgroundRenderingMode.BeforeOpaques:
                    return CameraBackgroundRenderingMode.BeforeOpaques;

                case XRSupportedCameraBackgroundRenderingMode.AfterOpaques:
                    return CameraBackgroundRenderingMode.AfterOpaques;

                case XRSupportedCameraBackgroundRenderingMode.Any:
                default:
                    return CameraBackgroundRenderingMode.Any;
            }
        }
    }
}
