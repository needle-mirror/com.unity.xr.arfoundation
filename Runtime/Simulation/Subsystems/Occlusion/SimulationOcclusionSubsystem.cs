using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.XR.CoreUtils;
using UnityEngine.Rendering;
using UnityEngine.XR.ARFoundation.InternalUtils;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.Simulation
{
    /// <summary>
    /// Simulation implementation of <see cref="UnityEngine.XR.ARSubsystems.XROcclusionSubsystem"/>.
    /// Do not create this directly. Use the <see cref="UnityEngine.SubsystemManager"/> instead.
    /// </summary>
    public sealed class SimulationOcclusionSubsystem : XROcclusionSubsystem
    {
        internal const string k_SubsystemId = "XRSimulation-Occlusion";

        /// <summary>
        /// The shader property name for the depth component of the camera video frame.
        /// </summary>
        /// <value>The shader depth property name</value>
        const string k_TextureSingleDepthPropertyName = "_TextureSingleDepth";

        /// <summary>
        /// The shader property name identifier for the depth component of the camera video frame.
        /// </summary>
        internal static readonly int textureSingleDepthPropertyNameId =
            Shader.PropertyToID(k_TextureSingleDepthPropertyName);

        class SimulationProvider : Provider
        {
            CameraTextureProvider m_CameraTextureProvider;

            NativeArray<XRTextureDescriptor> m_TextureDescriptors;

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

                var simulationCamera = SimulationCamera.GetOrCreateSimulationCamera();
                var simCamera = simulationCamera.GetComponent<Camera>();
                simCamera.depthTextureMode = DepthTextureMode.Depth;
                m_CameraTextureProvider = CameraTextureProvider.AddTextureProviderToCamera(simCamera, xrCamera);

                m_CameraTextureProvider.SetEnableDepthReadback(true);
            }

            public override void Stop()
            {
                m_CameraTextureProvider.SetEnableDepthReadback(false);
            }

            public override void Destroy()
            {
            }

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
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Register()
        {
            var cInfo = new XROcclusionSubsystemDescriptor.Cinfo()
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
