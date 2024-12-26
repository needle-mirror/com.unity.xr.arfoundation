using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.XR.CoreUtils;
using Unity.XR.CoreUtils.Collections;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.Simulation
{
    /// <summary>
    /// Simulation implementation of <see cref="UnityEngine.XR.ARSubsystems.XROcclusionSubsystem"/>.
    /// </summary>
    public sealed class SimulationOcclusionSubsystem : XROcclusionSubsystem
    {
        internal const string k_SubsystemId = "XRSimulation-Occlusion";

        /// <summary>
        /// The shader property name for the depth component of the camera video frame.
        /// </summary>
        internal const string k_TextureSingleDepthPropertyName = "_TextureSingleDepth";

        class SimulationProvider : Provider
        {
            const string k_SimulationOcclusionEnabledMaterialKeyword = "SIMULATION_OCCLUSION_ENABLED";
            static readonly List<string> k_EnvironmentDepthKeywords = new() { k_SimulationOcclusionEnabledMaterialKeyword };
            static readonly ReadOnlyList<string> k_EnvironmentDepthKeywordsReadOnly = new(k_EnvironmentDepthKeywords);

            static readonly XRShaderKeywords k_EnvironmentDepthEnabledKeywords =
                new(k_EnvironmentDepthKeywordsReadOnly, null);

            static readonly XRShaderKeywords k_EnvironmentDepthDisabledKeywords =
                new(null, k_EnvironmentDepthKeywordsReadOnly);

            CameraTextureProvider m_CameraTextureProvider;
            NativeArray<XRTextureDescriptor> m_TextureDescriptors;
            bool m_Running;

            public override void Start()
            {
#if UNITY_EDITOR
                SimulationSubsystemAnalytics.SubsystemStarted(k_SubsystemId);
#endif

                var xrOrigin = Object.FindAnyObjectByType<XROrigin>();
                if (xrOrigin == null)
                {
                    Debug.LogError("No XROrigin found. Occlusion will be disabled.");
                    return;
                }

                var xrCamera = xrOrigin.Camera;
                if (xrCamera == null)
                {
                    Debug.LogError("XROrigin's Camera property is null. Occlusion will be disabled.");
                    return;
                }

                var simCameraPoseProvider = SimulationCameraPoseProvider.GetOrCreateSimulationCameraPoseProvider();
                var simCamera = simCameraPoseProvider.GetComponent<Camera>();
                simCamera.depthTextureMode = DepthTextureMode.Depth;
                m_CameraTextureProvider = CameraTextureProvider.AddTextureProviderToCamera(simCamera, xrCamera);

                m_Running = true;
                m_CameraTextureProvider.SetEnableDepthReadback(true);
            }

            public override void Stop()
            {
                m_Running = false;
                m_CameraTextureProvider.SetEnableDepthReadback(false);
            }

            public override void Destroy() { }

            public override bool TryGetEnvironmentDepth(out XRTextureDescriptor environmentDepthDescriptor)
            {
                if (running && m_TextureDescriptors.Length > 0)
                {
                    environmentDepthDescriptor = m_TextureDescriptors[0];
                    return true;
                }

                environmentDepthDescriptor = default;
                return false;
            }

            public override NativeArray<XRTextureDescriptor> GetTextureDescriptors(XRTextureDescriptor defaultDescriptor, Allocator allocator)
            {
                if (!running)
                    return default;

                m_CameraTextureProvider.TryGetDepthTextureDescriptors(out m_TextureDescriptors, allocator);
                bool success = m_TextureDescriptors != default &&
                    m_TextureDescriptors.Length > 0 &&
                    m_TextureDescriptors[0] != default &&
                    m_TextureDescriptors.IsCreated;

                return success ? m_TextureDescriptors : default;
            }

            [Obsolete]
            public override ShaderKeywords GetShaderKeywords()
            {
                return m_Running
                    ? new ShaderKeywords(k_EnvironmentDepthKeywords.AsReadOnly(), null)
                    : new ShaderKeywords(null, k_EnvironmentDepthKeywords.AsReadOnly());
            }

            public override XRShaderKeywords GetShaderKeywords2()
                => m_Running ? k_EnvironmentDepthEnabledKeywords : k_EnvironmentDepthDisabledKeywords;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Register()
        {
            var cInfo = new XROcclusionSubsystemDescriptor.Cinfo
            {
                id = k_SubsystemId,
                providerType = typeof(SimulationProvider),
                subsystemTypeOverride = typeof(SimulationOcclusionSubsystem),
                humanSegmentationDepthImageSupportedDelegate = () => Supported.Unsupported,
                humanSegmentationStencilImageSupportedDelegate = () => Supported.Unsupported,
                environmentDepthImageSupportedDelegate = () => Supported.Supported,
            };

            XROcclusionSubsystemDescriptor.Register(cInfo);
        }
    }
}
