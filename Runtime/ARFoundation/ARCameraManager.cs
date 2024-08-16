using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.Rendering;

namespace UnityEngine.XR.ARFoundation
{
    /// <summary>
    /// Manages the life cycle of the <see cref="XRCameraSubsystem"/>. Add one of these to a <c>Camera</c> in your scene
    /// if you want camera texture and light estimation information to be available.
    /// </summary>
    [DefaultExecutionOrder(ARUpdateOrder.k_CameraManager)]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Camera))]
    [HelpURL(typeof(ARCameraManager))]
    public sealed class ARCameraManager :
        SubsystemLifecycleManager<XRCameraSubsystem, XRCameraSubsystemDescriptor, XRCameraSubsystem.Provider>,
        ISerializationCallbackReceiver
    {
        static List<Texture2D> s_Textures = new();
        static List<int> s_PropertyIds = new();
        readonly List<ARTextureInfo> m_TextureInfos = new();

        Camera m_Camera;
        bool m_PreRenderInvertCullingValue;
        ARTextureInfo m_CameraGrainInfo;

        /// <summary>
        /// An event which fires each time a new camera frame is received.
        /// </summary>
        public event Action<ARCameraFrameEventArgs> frameReceived;

        [SerializeField]
        [HideInInspector]
        CameraFocusMode m_FocusMode = CameraFocusMode.Auto;

        [SerializeField]
        [HideInInspector]
#pragma warning disable CS0618
        // If a user has an old project from 2019 lying around, OnAfterDeserialize will auto-upgrade them to the new API.
        LightEstimationMode m_LightEstimationMode = LightEstimationMode.Disabled;
#pragma warning restore CS0618

        [SerializeField]
        [Tooltip("When enabled, auto focus will be requested on the (physical) AR camera.")]
        bool m_AutoFocus = true;

        /// <summary>
        /// Get or set whether auto focus is requested.
        /// </summary>
        public bool autoFocusRequested
        {
            get => subsystem?.autoFocusRequested ?? m_AutoFocus;
            set
            {
                m_AutoFocus = value;
                if (enabled && subsystem != null)
                {
                    subsystem.autoFocusRequested = value;
                }
            }
        }

        /// <summary>
        /// Get the current focus mode in use by the subsystem.
        /// </summary>
        /// <value><see langword="true"/> if auto focus is enabled. <see langword="false"/> if fixed focus is enabled
        /// or if there is no loaded <see cref="XRCameraSubsystem"/>.</value>
        public bool autoFocusEnabled => subsystem?.autoFocusEnabled ?? false;

        [SerializeField]
        [Tooltip("When enabled, Image Stabilization will be requested on the AR camera.")]
        bool m_ImageStabilization = false;

        /// <summary>
        /// Get or set whether Image Stabilization is requested.
        /// </summary>
        public bool imageStabilizationRequested
        {
            get => subsystem?.imageStabilizationRequested ?? m_ImageStabilization;
            set
            {
                m_ImageStabilization = value;
                if (enabled && subsystem != null)
                {
                    subsystem.imageStabilizationRequested = value;
                }
            }
        }

        /// <summary>
        /// Get whether Image Stabilization in enabled.
        /// </summary>
        /// <value><see langword="true"/> if EIS is enabled. <see langword="false"/> if EIS is not enabled
        /// or if there is no loaded <see cref="XRCameraSubsystem"/>.</value>
        public bool imageStabilizationEnabled => subsystem?.imageStabilizationEnabled ?? false;

        [SerializeField]
        [Tooltip("The light estimation mode for the AR camera.")]
        LightEstimation m_LightEstimation = LightEstimation.None;

        /// <summary>
        /// Get or set the requested <see cref="LightEstimation"/> for the camera.
        /// </summary>
        /// <value>The light estimation mode for the camera.</value>
        public LightEstimation requestedLightEstimation
        {
            get => subsystem?.requestedLightEstimation.ToLightEstimation() ?? m_LightEstimation;
            set
            {
                m_LightEstimation = value;
                if (enabled && subsystem != null)
                {
                    subsystem.requestedLightEstimation = value.ToFeature();
                }
            }
        }

        /// <summary>
        /// Get the current light estimation mode used by the subsystem, or <c>LightEstimation.None</c>
        /// if there is no subsystem.
        /// </summary>
        public LightEstimation currentLightEstimation => subsystem?.currentLightEstimation.ToLightEstimation() ?? LightEstimation.None;

        [SerializeField]
        [Tooltip("The requested camera facing direction")]
        CameraFacingDirection m_FacingDirection = CameraFacingDirection.World;

        /// <summary>
        /// Get or set the requested camera facing direction.
        /// </summary>
        public CameraFacingDirection requestedFacingDirection
        {
            get => subsystem?.requestedCamera.ToCameraFacingDirection() ?? m_FacingDirection;
            set
            {
                m_FacingDirection = value;
                if (enabled && subsystem != null)
                {
                    subsystem.requestedCamera = value.ToFeature();
                }
            }
        }

        /// <summary>
        /// The current camera facing direction. This should usually match <see cref="requestedFacingDirection"/>
        /// but might be different if the platform cannot service the requested camera facing direction, or it might
        /// take a few frames for the requested facing direction to become active.
        /// </summary>
        public CameraFacingDirection currentFacingDirection => subsystem?.currentCamera.ToCameraFacingDirection() ?? CameraFacingDirection.None;

        [SerializeField]
        [Tooltip("The requested background rendering mode. Using mode 'Any' allows the platform provider to determine the rendering mode.")]
        CameraBackgroundRenderingMode m_RenderMode = CameraBackgroundRenderingMode.Any;

        /// <summary>
        /// The current requested <see cref="CameraBackgroundRenderingMode"/>. When set, this value is converted to an
        /// <see cref="XRCameraBackgroundRenderingMode"/> and passed to <see cref="XRCameraSubsystem.requestedCameraBackgroundRenderingMode"/>
        /// if the camera subsystem is non-null.
        /// </summary>
        public CameraBackgroundRenderingMode requestedBackgroundRenderingMode
        {
            get => subsystem?.requestedCameraBackgroundRenderingMode.ToBackgroundRenderingMode() ?? m_RenderMode;
            set
            {
                m_RenderMode = value;
                if (enabled && subsystem != null)
                {
                    subsystem.requestedCameraBackgroundRenderingMode = value.ToXRSupportedCameraBackgroundRenderingMode();
                }
            }
        }

        /// <summary>
        /// The current <see cref="XRCameraBackgroundRenderingMode"/> of the <see cref="XRCameraSubsystem"/>, or
        /// <see cref="XRCameraBackgroundRenderingMode.None"/> if the subsystem is <see langword="null"/>.
        /// </summary>
        /// <value>The current camera background rendering mode.</value>
        public XRCameraBackgroundRenderingMode currentRenderingMode => subsystem?.currentCameraBackgroundRenderingMode ?? XRCameraBackgroundRenderingMode.None;

        /// <summary>
        /// Indicates whether camera permission has been granted.
        /// </summary>
        /// <value><see langword="true"/> if permission has been granted. Otherwise, <see langword="false"/>.</value>
        public bool permissionGranted => subsystem is { permissionGranted: true };

        /// <summary>
        /// The Material used in background rendering.
        /// </summary>
        /// <value>The Material used in background rendering.</value>
        public Material cameraMaterial => subsystem?.cameraMaterial;

        /// <summary>
        /// Part of the [ISerializationCallbackReceiver](https://docs.unity3d.com/ScriptReference/ISerializationCallbackReceiver.html)
        /// interface. Invoked before serialization.
        /// </summary>
        public void OnBeforeSerialize() { }

        /// <summary>
        /// Part of the [ISerializationCallbackReceiver](https://docs.unity3d.com/ScriptReference/ISerializationCallbackReceiver.html)
        /// interface. Invoked after deserialization.
        /// </summary>
        public void OnAfterDeserialize()
        {
            if (m_FocusMode != (CameraFocusMode)(-1))
            {
                m_AutoFocus = m_FocusMode == CameraFocusMode.Auto;
                m_FocusMode = (CameraFocusMode)(-1);
            }

#pragma warning disable CS0618
            if (m_LightEstimationMode != (LightEstimationMode)(-1))
            {
                m_LightEstimation = m_LightEstimationMode.ToLightEstimation();
                m_LightEstimationMode = (LightEstimationMode)(-1);
            }
#pragma warning restore CS0618
        }

        /// <summary>
        /// Tries to get camera intrinsics. Camera intrinsics refers to properties of a physical camera which might be
        /// useful when performing additional computer vision processing on the camera image.
        /// </summary>
        /// <param name="cameraIntrinsics">The camera intrinsics to be populated if the camera supports intrinsics.
        /// </param>
        /// <returns><see langword="true"/> if <paramref name="cameraIntrinsics"/> was populated. Otherwise,
        /// <see langword="false"/>.</returns>
        /// <remarks>
        /// > [!NOTE]
        /// > The intrinsics may change each frame. You should call this each frame that you need intrinsics
        /// > in order to ensure you are using the intrinsics for the current frame.
        /// </remarks>
        public bool TryGetIntrinsics(out XRCameraIntrinsics cameraIntrinsics)
        {
            if (subsystem == null)
            {
                cameraIntrinsics = default;
                return false;
            }

            return subsystem.TryGetIntrinsics(out cameraIntrinsics);
        }

        /// <summary>
        /// Get the camera configurations currently supported for the implementation.
        /// </summary>
        /// <param name="allocator">The allocation strategy to use for the returned data.</param>
        /// <returns>The supported camera configurations.</returns>
        public NativeArray<XRCameraConfiguration> GetConfigurations(Allocator allocator)
            => subsystem?.GetConfigurations(allocator) ?? new NativeArray<XRCameraConfiguration>(0, allocator);

        /// <summary>
        /// The current camera configuration.
        /// </summary>
        /// <value>The current camera configuration, if it exists. Otherwise, <see langword="null"/>.</value>
        /// <exception cref="System.NotSupportedException">Thrown when setting the current configuration if the
        /// implementation does not support camera configurations.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown when setting the current configuration if the given
        /// configuration is <see langword="null"/>.</exception>
        /// <exception cref="System.ArgumentException">Thrown when setting the current configuration if the given
        /// configuration is not a supported camera configuration.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown when setting the current configuration if the
        /// implementation is unable to set the current camera configuration.</exception>
        public XRCameraConfiguration? currentConfiguration
        {
            get => subsystem?.currentConfiguration;
            set
            {
                if (subsystem != null)
                    subsystem.currentConfiguration = value;
            }
        }

        /// <summary>
        /// Attempts to acquire the latest camera image. This provides direct access to the raw pixel data, as well as
        /// to utilities to convert to RGB and Grayscale formats.
        /// </summary>
        /// <param name="cpuImage">A valid `XRCpuImage` if this method returns `true`.</param>
        /// <returns><see langword="true"/> if the latest camera image was successfully acquired. Otherwise,
        /// <see langword="false"/>.</returns>
        /// <remarks>The <see cref="XRCpuImage"/> must be disposed to avoid resource leaks.</remarks>
        public bool TryAcquireLatestCpuImage(out XRCpuImage cpuImage)
        {
            if (subsystem == null)
            {
                cpuImage = default;
                return false;
            }

            return subsystem.TryAcquireLatestCpuImage(out cpuImage);
        }

        void Awake()
        {
            m_Camera = GetComponent<Camera>();
        }

        /// <inheritdoc/>
        protected override void OnBeforeStart()
        {
            subsystem.requestedCameraBackgroundRenderingMode = m_RenderMode.ToXRSupportedCameraBackgroundRenderingMode();
            subsystem.autoFocusRequested = m_AutoFocus;
            subsystem.imageStabilizationRequested = m_ImageStabilization;
            subsystem.requestedLightEstimation = m_LightEstimation.ToFeature();
            subsystem.requestedCamera = m_FacingDirection.ToFeature();
        }

        /// <inheritdoc/>
        protected override void OnDisable()
        {
            base.OnDisable();

            foreach (var textureInfo in m_TextureInfos)
            {
                textureInfo.Dispose();
            }

            m_TextureInfos.Clear();
        }

        void Update()
        {
            if (subsystem == null)
                return;

            m_RenderMode = subsystem.requestedCameraBackgroundRenderingMode.ToBackgroundRenderingMode();
            m_FacingDirection = subsystem.requestedCamera.ToCameraFacingDirection();
            m_LightEstimation = subsystem.requestedLightEstimation.ToLightEstimation();
            m_AutoFocus = subsystem.autoFocusRequested;
            m_ImageStabilization = subsystem.imageStabilizationRequested;

            var cameraParams = new XRCameraParams
            {
                zNear = m_Camera.nearClipPlane,
                zFar = m_Camera.farClipPlane,
                screenWidth = Screen.width,
                screenHeight = Screen.height,
                screenOrientation = Screen.orientation
            };

            if (subsystem.TryGetLatestFrame(cameraParams, out XRCameraFrame frame))
            {
                UpdateTexturesInfos();
                InvokeFrameReceivedEvent(frame);
            }
        }

        /// <summary>
        /// Pull the texture descriptors from the camera subsystem, and update the texture information maintained by
        /// this component.
        /// </summary>
        void UpdateTexturesInfos()
        {
            var textureDescriptors = subsystem.GetTextureDescriptors(Allocator.Temp);
            try
            {
                int numUpdated = Math.Min(m_TextureInfos.Count, textureDescriptors.Length);

                // Update the existing textures that are in common between the two arrays.
                for (var i = 0; i < numUpdated; ++i)
                {
                    m_TextureInfos[i] = ARTextureInfo.GetUpdatedTextureInfo(m_TextureInfos[i], textureDescriptors[i]);
                }

                // If there are fewer textures in the current frame than we had previously, destroy any remaining unneeded
                // textures.
                if (numUpdated < m_TextureInfos.Count)
                {
                    for (var i = numUpdated; i < m_TextureInfos.Count; ++i)
                    {
                        m_TextureInfos[i].Reset();
                    }
                    m_TextureInfos.RemoveRange(numUpdated, (m_TextureInfos.Count - numUpdated));
                }
                // Else, if there are more textures in the current frame than we have previously, add new textures for any
                // additional descriptors.
                else if (textureDescriptors.Length > m_TextureInfos.Count)
                {
                    for (var i = numUpdated; i < textureDescriptors.Length; ++i)
                    {
                        m_TextureInfos.Add(new ARTextureInfo(textureDescriptors[i]));
                    }
                }
            }
            finally
            {
                if (textureDescriptors.IsCreated)
                    textureDescriptors.Dispose();
            }
        }

        /// <summary>
        /// Invoke the camera frame received event packing the frame information into the event argument.
        /// </summary>
        /// <param name="frame">The camera frame raising the event.</param>
        void InvokeFrameReceivedEvent(XRCameraFrame frame)
        {
            if (frameReceived == null)
                return;

            var lightEstimation = new ARLightEstimationData();

            if (frame.TryGetAverageBrightness(out var averageBrightness))
                lightEstimation.averageBrightness = averageBrightness;

            if (frame.TryGetAverageIntensityInLumens(out var averageIntensityInLumens))
                lightEstimation.averageIntensityInLumens = averageIntensityInLumens;

            if (frame.TryGetAverageColorTemperature(out var averageColorTemperature))
                lightEstimation.averageColorTemperature = averageColorTemperature;

            if (frame.TryGetColorCorrection(out var colorCorrection))
                lightEstimation.colorCorrection = colorCorrection;

            if (frame.TryGetMainLightDirection(out var mainLightDirection))
                lightEstimation.mainLightDirection = mainLightDirection;

            if (frame.TryGetMainLightIntensityLumens(out var mainLightIntensityLumens))
                lightEstimation.mainLightIntensityLumens = mainLightIntensityLumens;

            if (frame.TryGetMainLightColor(out var mainLightColor))
                lightEstimation.mainLightColor = mainLightColor;

            if (frame.TryGetAmbientSphericalHarmonics(out var ambientSphericalHarmonics))
                lightEstimation.ambientSphericalHarmonics = ambientSphericalHarmonics;

            var eventArgs = new ARCameraFrameEventArgs();

            eventArgs.lightEstimation = lightEstimation;

            if (frame.TryGetTimestamp(out var timestampNs))
                eventArgs.timestampNs = timestampNs;

            if (frame.TryGetProjectionMatrix(out var projectionMatrix))
                eventArgs.projectionMatrix = projectionMatrix;

            if (frame.TryGetDisplayMatrix(out var displayMatrix))
                eventArgs.displayMatrix = displayMatrix;

            if (frame.TryGetExposureDuration(out var exposureDuration))
                eventArgs.exposureDuration = exposureDuration;

            if (frame.TryGetExposureOffset(out var exposureOffset))
                eventArgs.exposureOffset = exposureOffset;

            if (frame.TryGetCameraGrain(out var cameraGrain))
            {
                if (m_CameraGrainInfo.texture == null && ARTextureInfo.IsSupported(cameraGrain))
                {
                    m_CameraGrainInfo = new ARTextureInfo(cameraGrain);
                }
                else if (m_CameraGrainInfo.texture != null && ARTextureInfo.IsSupported(cameraGrain))
                {
                    m_CameraGrainInfo = ARTextureInfo.GetUpdatedTextureInfo(m_CameraGrainInfo, cameraGrain);
                }

                eventArgs.cameraGrainTexture = m_CameraGrainInfo.texture;
            }

            if (frame.TryGetNoiseIntensity(out var noiseIntensity))
                eventArgs.noiseIntensity = noiseIntensity;

            if (frame.TryGetExifData(out XRCameraFrameExifData exifData))
                eventArgs.exifData = exifData;

            s_Textures.Clear();
            s_PropertyIds.Clear();
            foreach (var textureInfo in m_TextureInfos)
            {
                DebugAssert.That(textureInfo.descriptor.dimension == TextureDimension.Tex2D)?.
                    WithMessage($"Camera Texture needs to be a Texture 2D, but instead is {textureInfo.descriptor.dimension.ToString()}.");

                s_Textures.Add((Texture2D)textureInfo.texture);
                s_PropertyIds.Add(textureInfo.descriptor.propertyNameId);
            }

            ShaderKeywords shaderKeywords = subsystem.GetShaderKeywords();

            eventArgs.textures = s_Textures;
            eventArgs.propertyNameIds = s_PropertyIds;
            eventArgs.enabledShaderKeywords = shaderKeywords.enabledKeywords;
            eventArgs.disabledShaderKeywords = shaderKeywords.disabledKeywords;

            frameReceived(eventArgs);
        }
    }
}
