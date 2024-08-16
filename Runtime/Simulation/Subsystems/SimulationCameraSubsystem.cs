using System;
using Unity.Collections;
using Unity.XR.CoreUtils;
using UnityEngine.Rendering;
using UnityEngine.XR.ARFoundation.InternalUtils;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.Simulation
{
    /// <summary>
    /// Simulation implementation of
    /// [`XRCameraSubsystem`](xref:UnityEngine.XR.ARSubsystems.XRCameraSubsystem).
    /// </summary>
    public sealed class SimulationCameraSubsystem : XRCameraSubsystem
    {
        internal const string k_SubsystemId = "XRSimulation-Camera";

        /// <summary>
        /// The name for the shader for rendering the camera texture.
        /// </summary>
        /// <value>
        /// The name for the shader for rendering the camera texture.
        /// </value>
        const string k_BackgroundShaderName = "Unlit/Simulation Background Simple";

        /// <summary>
        /// The shader property name for the simple RGB component of the camera video frame.
        /// </summary>
        /// <value>
        /// The shader property name for the  simple RGB component of the camera video frame.
        /// </value>
        internal const string k_TextureSinglePropertyName = "_TextureSingle";

        class SimulationProvider : Provider
        {
            CameraTextureFrameEventArgs m_CameraTextureFrameEventArgs;
            CameraTextureProvider m_CameraTextureProvider;
            Camera m_Camera;
            Material m_CameraMaterial;
            XRCameraConfiguration m_XRCameraConfiguration;
            XRCameraIntrinsics m_XRCameraIntrinsics;

            double m_LastFrameTimestamp = 0;

            XRSupportedCameraBackgroundRenderingMode m_RequestedBackgroundRenderingMode = XRSupportedCameraBackgroundRenderingMode.BeforeOpaques;

            Feature m_RequestedLightEstimation = Feature.None;
            SimulatedLight m_MainLight;

            public override XRCpuImage.Api cpuImageApi => SimulationXRCpuImageApi.instance;
            public override Feature currentCamera => Feature.WorldFacingCamera;

            public override XRCameraConfiguration? currentConfiguration
            {
                get => m_XRCameraConfiguration;
                set
                {
                    // Currently assuming any not null configuration is valid for simulation
                    if (value == null)
                        throw new ArgumentNullException("value", "cannot set the camera configuration to null");

                    m_XRCameraConfiguration = (XRCameraConfiguration)value;
                }
            }

            public override Feature currentLightEstimation => base.currentLightEstimation;

            public override Feature requestedLightEstimation
            {
                get => m_RequestedLightEstimation;
                set => m_RequestedLightEstimation = value;
            }

            public override Material cameraMaterial => m_CameraMaterial;

            public override bool permissionGranted => true;

            public override XRSupportedCameraBackgroundRenderingMode requestedBackgroundRenderingMode
            {
                get => m_RequestedBackgroundRenderingMode;
                set => m_RequestedBackgroundRenderingMode = value;
            }

            public override XRCameraBackgroundRenderingMode currentBackgroundRenderingMode
            {
                get
                {
                    switch (requestedBackgroundRenderingMode)
                    {
                        case XRSupportedCameraBackgroundRenderingMode.AfterOpaques:
                            return XRCameraBackgroundRenderingMode.AfterOpaques;
                        case XRSupportedCameraBackgroundRenderingMode.BeforeOpaques:
                        case XRSupportedCameraBackgroundRenderingMode.Any:
                            return XRCameraBackgroundRenderingMode.BeforeOpaques;
                        default:
                            return XRCameraBackgroundRenderingMode.None;
                    }
                }
            }

            public override XRSupportedCameraBackgroundRenderingMode supportedBackgroundRenderingMode => XRSupportedCameraBackgroundRenderingMode.Any;

            public SimulationProvider()
            {
                var backgroundShader = Shader.Find(k_BackgroundShaderName);

                if (backgroundShader == null)
                    Debug.LogError("Cannot create camera background material compatible with the render pipeline");
                else
                    m_CameraMaterial = CreateCameraMaterial(k_BackgroundShaderName);
            }

            public override void Start()
            {
#if UNITY_EDITOR
                SimulationSubsystemAnalytics.SubsystemStarted(k_SubsystemId);
#endif

                var xrOrigin = Object.FindAnyObjectByType<XROrigin>();
                if (xrOrigin == null)
                {
                    Debug.LogError("An XR Origin is required in the scene, none found.");
                    return;
                }

                var xrCamera = xrOrigin.Camera;
                if (xrCamera == null)
                {
                    Debug.LogError("No camera found under XROrigin.");
                    return;
                }

                var simCameraPoseProvider = SimulationCameraPoseProvider.GetOrCreateSimulationCameraPoseProvider();
                m_CameraTextureProvider = CameraTextureProvider.AddTextureProviderToCamera(simCameraPoseProvider.GetComponent<Camera>(), xrCamera);
                m_CameraTextureProvider.onTextureReadbackFulfilled += SimulationXRCpuImageApi.OnCameraDataReceived;
                m_CameraTextureProvider.cameraFrameReceived += CameraFrameReceived;
                if (m_CameraTextureProvider != null && m_CameraTextureProvider.CameraFrameEventArgs != null)
                    m_CameraTextureFrameEventArgs = (CameraTextureFrameEventArgs)m_CameraTextureProvider.CameraFrameEventArgs;

                m_Camera = m_CameraTextureProvider.GetComponent<Camera>();

                m_XRCameraConfiguration = new XRCameraConfiguration(IntPtr.Zero, new Vector2Int(m_Camera.pixelWidth, m_Camera.pixelHeight));
                m_XRCameraIntrinsics = new XRCameraIntrinsics();

                BaseSimulationSceneManager.environmentSetupFinished += OnEnvironmentSetupFinished;
            }

            public override void Stop()
            {
                if (m_CameraTextureProvider != null)
                {
                    m_CameraTextureProvider.cameraFrameReceived -= CameraFrameReceived;
                    m_CameraTextureProvider.onTextureReadbackFulfilled -= SimulationXRCpuImageApi.OnCameraDataReceived;
                }

                BaseSimulationSceneManager.environmentSetupFinished -= OnEnvironmentSetupFinished;
            }

            public override void Destroy()
            {
                if (m_CameraTextureProvider != null)
                {
                    Object.Destroy(m_CameraTextureProvider.gameObject);
                    m_CameraTextureProvider = null;
                }
            }

            void OnEnvironmentSetupFinished()
            {
                m_MainLight = null;

                var simulationEnvironmentLights = SimulationSessionSubsystem.simulationSceneManager.simulationEnvironmentLights;
                foreach (var light in simulationEnvironmentLights)
                {
                    if (light.simulatedLight.type == LightType.Directional)
                    {
                        if (m_MainLight == null)
                        {
                            m_MainLight = light;
                        }
                        else if (m_MainLight.simulatedLight.intensity < light.simulatedLight.intensity)
                        {
                            Debug.LogWarning("Multiple directional lights were detected in the XR Simulation environment. The light with the highest intensity will be used as the main light.");
                            m_MainLight = light;
                        }
                    }
                }
            }

            public override NativeArray<XRCameraConfiguration> GetConfigurations(XRCameraConfiguration defaultCameraConfiguration, Allocator allocator)
            {
                var configs = new NativeArray<XRCameraConfiguration>(1, allocator);
                configs[0] = m_XRCameraConfiguration;
                return configs;
            }

            public override NativeArray<XRTextureDescriptor> GetTextureDescriptors(XRTextureDescriptor defaultDescriptor, Allocator allocator)
            {
                if (m_CameraTextureProvider != null)
                {
                    m_CameraTextureProvider.TryGetTextureDescriptors(out var descriptors, allocator);
                    return descriptors;
                }

                return base.GetTextureDescriptors(defaultDescriptor, allocator);
            }

            public override bool TryAcquireLatestCpuImage(out XRCpuImage.Cinfo cameraImageCinfo)
            {
                return SimulationXRCpuImageApi.TryAcquireLatestImage(SimulationXRCpuImageApi.ImageType.Camera, out cameraImageCinfo);
            }

            void CameraFrameReceived(CameraTextureFrameEventArgs args)
            {
                m_CameraTextureFrameEventArgs = args;
            }

            public override bool TryGetFrame(XRCameraParams cameraParams, out XRCameraFrame cameraFrame)
            {
                if (m_CameraTextureProvider == null)
                {
                    cameraFrame = new XRCameraFrame();
                    return false;
                }

                XRCameraFrameProperties properties = 0;

                long timeStamp = default;
                float averageBrightness = default;
                float averageColorTemperature = default;
                Color colorCorrection = default;
                Matrix4x4 projectionMatrix = default;
                Matrix4x4 displayMatrix = default;
                TrackingState trackingState = default;
                IntPtr nativePtr = default;
                float averageIntensityInLumens = default;
                double exposureDuration = default;
                float exposureOffset = default;
                float mainLightIntensityInLumens = default;
                Color mainLightColor = default;
                Vector3 mainLightDirection = default;
                SphericalHarmonicsL2 ambientSphericalHarmonics = default;
                XRTextureDescriptor cameraGrain = default;
                float noiseIntensity = default;

                if (m_CameraTextureFrameEventArgs.timestampNs.HasValue)
                {
                    timeStamp = (long)m_CameraTextureFrameEventArgs.timestampNs;
                    properties |= XRCameraFrameProperties.Timestamp;
                }

                if (m_CameraTextureFrameEventArgs.projectionMatrix.HasValue)
                {
                    projectionMatrix = (Matrix4x4)m_CameraTextureFrameEventArgs.projectionMatrix;
                    properties |= XRCameraFrameProperties.ProjectionMatrix;
                }

                if (m_CameraTextureProvider == null || !m_CameraTextureProvider.TryGetLatestImagePtr(out nativePtr))
                {
                    cameraFrame = default;
                    m_LastFrameTimestamp = timeStamp;
                    return false;
                }

                var simulationEnvironmentLights = SimulationSessionSubsystem.simulationSceneManager?.simulationEnvironmentLights;
                if (simulationEnvironmentLights?.Count > 0)
                {
                    averageBrightness = CalculateAverageBrightness();
                    properties |= XRCameraFrameProperties.AverageBrightness;

                    if (GraphicsSettings.lightsUseLinearIntensity && CalculateAverageColorTemperature(out averageColorTemperature))
                    {
                        properties |= XRCameraFrameProperties.AverageColorTemperature;
                    }

                    averageIntensityInLumens = CalculateAverageIntensityInLumens();
                    properties |= XRCameraFrameProperties.AverageIntensityInLumens;

                    if (m_MainLight != null)
                    {
                        colorCorrection = m_MainLight.simulatedLight.color;
                        properties |= XRCameraFrameProperties.ColorCorrection;

                        mainLightIntensityInLumens = UnitConversionUtility.ConvertBrightnessToLumens(m_MainLight.simulatedLight.intensity);
                        properties |= XRCameraFrameProperties.MainLightIntensityLumens;

                        mainLightColor = m_MainLight.simulatedLight.color;
                        properties |= XRCameraFrameProperties.MainLightColor;

                        mainLightDirection = m_MainLight.transform.forward;
                        properties |= XRCameraFrameProperties.MainLightDirection;
                    }
                }

                m_LastFrameTimestamp = timeStamp;

                cameraFrame = new XRCameraFrame(
                    timeStamp,
                    averageBrightness,
                    averageColorTemperature,
                    colorCorrection,
                    projectionMatrix,
                    displayMatrix,
                    trackingState,
                    nativePtr,
                    properties,
                    averageIntensityInLumens,
                    exposureDuration,
                    exposureOffset,
                    mainLightIntensityInLumens,
                    mainLightColor,
                    mainLightDirection,
                    ambientSphericalHarmonics,
                    cameraGrain,
                    noiseIntensity);

                return true;
            }

            static float CalculateAverageBrightness()
            {
                if (SimulationSessionSubsystem.simulationSceneManager == null)
                    return 0.0f;

                float sum = 0.0f;

                var simulationEnvironmentLights = SimulationSessionSubsystem.simulationSceneManager.simulationEnvironmentLights;
                foreach (var light in simulationEnvironmentLights)
                {
                    sum += light.simulatedLight.intensity;
                }
                return sum / Math.Max(simulationEnvironmentLights.Count, 1);
            }

            static bool CalculateAverageColorTemperature(out float result)
            {
                if (SimulationSessionSubsystem.simulationSceneManager == null)
                {
                    result = 0.0f;
                    return false;
                }

                float sum = 0.0f;
                bool usesColorTemperature = false;

                var simulationEnvironmentLights = SimulationSessionSubsystem.simulationSceneManager.simulationEnvironmentLights;
                foreach (var light in simulationEnvironmentLights)
                {
                    if (light.simulatedLight.useColorTemperature)
                    {
                        sum += light.simulatedLight.colorTemperature;
                        usesColorTemperature = true;
                    }
                }

                result = sum / Math.Max(simulationEnvironmentLights.Count, 1);
                return usesColorTemperature;
            }

            static float CalculateAverageIntensityInLumens()
            {
                if (SimulationSessionSubsystem.simulationSceneManager == null)
                    return 0.0f;

                float sum = 0.0f;

                var simulationEnvironmentLights = SimulationSessionSubsystem.simulationSceneManager.simulationEnvironmentLights;
                foreach (var light in simulationEnvironmentLights)
                {
                    sum += UnitConversionUtility.ConvertBrightnessToLumens(light.simulatedLight.intensity);
                }
                return sum / Math.Max(simulationEnvironmentLights.Count, 1);
            }

            public override bool TryGetIntrinsics(out XRCameraIntrinsics cameraIntrinsics)
            {
                cameraIntrinsics = m_XRCameraIntrinsics;
                return true;
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Register()
        {
            var cInfo = new XRCameraSubsystemDescriptor.Cinfo
            {
                id = k_SubsystemId,
                providerType = typeof(SimulationProvider),
                subsystemTypeOverride = typeof(SimulationCameraSubsystem),
                supportsCameraConfigurations = true,
                supportsCameraImage = true,
                supportsAverageColorTemperature = true,
                supportsColorCorrection = true,
                supportsAverageBrightness = true,
                supportsAverageIntensityInLumens = true,
            };

            XRCameraSubsystemDescriptor.Register(cInfo);
        }
    }
}
