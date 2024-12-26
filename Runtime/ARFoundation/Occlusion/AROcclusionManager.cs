using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.XR.CoreUtils.Collections;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation
{
    /// <summary>
    /// The manager for the occlusion subsystem.
    /// </summary>
    /// <remarks>
    /// Related information: <a href="xref:arfoundation-occlusion">Occlusion</a>
    /// </remarks>
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(ARUpdateOrder.k_OcclusionManager)]
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("XR/AR Foundation/AR Occlusion Manager")]
    [HelpURL("features/occlusion")]
    public sealed class AROcclusionManager :
        SubsystemLifecycleManager<XROcclusionSubsystem, XROcclusionSubsystemDescriptor, XROcclusionSubsystem.Provider>
    {
        ISwapchainStrategy m_SwapchainStrategy;
        ARTextureInfo m_HumanStencilTextureInfo;
        ARTextureInfo m_HumanDepthTextureInfo;
        ARTextureInfo m_EnvironmentDepthTextureInfo;
        ARTextureInfo m_EnvironmentDepthConfidenceTextureInfo;

        // Output collections. These are not used for any internal state.
        readonly List<ARExternalTexture> m_Textures = new();
        readonly List<Pose> m_Poses = new();
        readonly List<XRFov> m_Fovs = new();
        ReadOnlyList<ARExternalTexture> m_TexturesReadOnly;
        ReadOnlyList<Pose> m_PosesReadOnly;
        ReadOnlyList<XRFov> m_FovsReadOnly;

        /// <summary>
        /// An event that fires each time an occlusion camera frame is received.
        /// </summary>
        public event Action<AROcclusionFrameEventArgs> frameReceived;

        /// <summary>
        /// Get or set the requested mode for generating the human segmentation stencil texture.
        /// </summary>
        public HumanSegmentationStencilMode requestedHumanStencilMode
        {
            get => subsystem?.requestedHumanStencilMode ?? m_HumanSegmentationStencilMode;
            set
            {
                m_HumanSegmentationStencilMode = value;
                if (enabled && descriptor?.humanSegmentationStencilImageSupported == Supported.Supported)
                    subsystem.requestedHumanStencilMode = value;
            }
        }

        /// <summary>
        /// Get the current mode in use for generating the human segmentation stencil mode.
        /// </summary>
        public HumanSegmentationStencilMode currentHumanStencilMode => subsystem?.currentHumanStencilMode ?? HumanSegmentationStencilMode.Disabled;

        [SerializeField]
        [Tooltip("The mode for generating human segmentation stencil texture.\n\n"
                 + "Disabled -- No human stencil texture produced.\n"
                 + "Fastest -- Minimal rendering quality. Minimal frame computation.\n"
                 + "Medium -- Medium rendering quality. Medium frame computation.\n"
                 + "Best -- Best rendering quality. Increased frame computation.")]
        HumanSegmentationStencilMode m_HumanSegmentationStencilMode = HumanSegmentationStencilMode.Disabled;

        /// <summary>
        /// Get or set the requested human segmentation depth mode.
        /// </summary>
        public HumanSegmentationDepthMode requestedHumanDepthMode
        {
            get => subsystem?.requestedHumanDepthMode ?? m_HumanSegmentationDepthMode;
            set
            {
                m_HumanSegmentationDepthMode = value;
                if (enabled && descriptor?.humanSegmentationDepthImageSupported == Supported.Supported)
                    subsystem.requestedHumanDepthMode = value;
            }
        }

        /// <summary>
        /// Get the current human segmentation depth mode in use by the subsystem.
        /// </summary>
        public HumanSegmentationDepthMode currentHumanDepthMode => subsystem?.currentHumanDepthMode ?? HumanSegmentationDepthMode.Disabled;

        [SerializeField]
        [Tooltip("The mode for generating human segmentation depth texture.\n\n"
                 + "Disabled -- No human depth texture produced.\n"
                 + "Fastest -- Minimal rendering quality. Minimal frame computation.\n"
                 + "Best -- Best rendering quality. Increased frame computation.")]
        HumanSegmentationDepthMode m_HumanSegmentationDepthMode = HumanSegmentationDepthMode.Disabled;

        /// <summary>
        /// Get or set the requested environment depth mode.
        /// </summary>
        public EnvironmentDepthMode requestedEnvironmentDepthMode
        {
            get => subsystem?.requestedEnvironmentDepthMode ?? m_EnvironmentDepthMode;
            set
            {
                m_EnvironmentDepthMode = value;
                if (enabled && descriptor?.environmentDepthImageSupported == Supported.Supported)
                    subsystem.requestedEnvironmentDepthMode = value;
            }
        }

        /// <summary>
        /// Get the current environment depth mode in use by the subsystem.
        /// </summary>
        public EnvironmentDepthMode currentEnvironmentDepthMode => subsystem?.currentEnvironmentDepthMode ?? EnvironmentDepthMode.Disabled;

        [SerializeField]
        [Tooltip("The mode for generating the environment depth texture.\n\n"
                 + "Disabled -- No environment depth texture produced.\n"
                 + "Fastest -- Minimal rendering quality. Minimal frame computation.\n"
                 + "Medium -- Medium rendering quality. Medium frame computation.\n"
                 + "Best -- Best rendering quality. Increased frame computation.")]
        EnvironmentDepthMode m_EnvironmentDepthMode = EnvironmentDepthMode.Fastest;

        [SerializeField]
        bool m_EnvironmentDepthTemporalSmoothing = true;

        /// <summary>
        /// Whether temporal smoothing should be applied to the environment depth image. Query for support with
        /// [environmentDepthTemporalSmoothingSupported](xref:UnityEngine.XR.ARSubsystems.XROcclusionSubsystemDescriptor.environmentDepthTemporalSmoothingSupported).
        /// </summary>
        /// <value><see langword="true"/> if environment depth temporal smoothing is requested. Otherwise, <see langword="false"/>.</value>
        public bool environmentDepthTemporalSmoothingRequested
        {
            get => subsystem?.environmentDepthTemporalSmoothingRequested ?? m_EnvironmentDepthTemporalSmoothing;
            set
            {
                m_EnvironmentDepthTemporalSmoothing = value;
                if (enabled && descriptor?.environmentDepthTemporalSmoothingSupported == Supported.Supported)
                    subsystem.environmentDepthTemporalSmoothingRequested = value;
            }
        }

        /// <summary>
        /// Whether temporal smoothing is applied to the environment depth image. Query for support with
        /// [environmentDepthTemporalSmoothingSupported](xref:UnityEngine.XR.ARSubsystems.XROcclusionSubsystemDescriptor.environmentDepthTemporalSmoothingSupported).
        /// </summary>
        /// <value><see langword="true"/> if temporal smoothing is applied to the environment depth image. Otherwise, <see langword="false"/>.</value>
        public bool environmentDepthTemporalSmoothingEnabled => subsystem?.environmentDepthTemporalSmoothingEnabled ?? false;

        /// <summary>
        /// Get or set the requested occlusion preference mode.
        /// </summary>
        public OcclusionPreferenceMode requestedOcclusionPreferenceMode
        {
            get => subsystem?.requestedOcclusionPreferenceMode ?? m_OcclusionPreferenceMode;
            set
            {
                m_OcclusionPreferenceMode = value;
                if (enabled && subsystem != null)
                    subsystem.requestedOcclusionPreferenceMode = value;
            }
        }

        /// <summary>
        /// Get the current occlusion preference mode in use by the subsystem.
        /// </summary>
        public OcclusionPreferenceMode currentOcclusionPreferenceMode =>
            subsystem?.currentOcclusionPreferenceMode ?? OcclusionPreferenceMode.PreferEnvironmentOcclusion;

        [SerializeField]
        [Tooltip("If both environment texture and human stencil & depth textures are available, this mode specifies which should be used for occlusion.")]
        OcclusionPreferenceMode m_OcclusionPreferenceMode = OcclusionPreferenceMode.PreferEnvironmentOcclusion;

        /// <summary>
        /// The human segmentation stencil texture, if any. Otherwise, <see langword="null"/>.
        /// </summary>
        /// <value>The human segmentation stencil texture.</value>
        public Texture2D humanStencilTexture
        {
            get
            {
                if (descriptor?.humanSegmentationStencilImageSupported != Supported.Supported ||
                    !subsystem.TryGetHumanStencil(out var humanStencilDescriptor))
                    return null;

                if (m_HumanStencilTextureInfo == null)
                    m_HumanStencilTextureInfo = new ARTextureInfo(humanStencilDescriptor);
                else if (!m_HumanStencilTextureInfo.TryUpdateTextureInfo(humanStencilDescriptor))
                    return null;

                DebugAssert.That(m_HumanStencilTextureInfo.descriptor.textureType is XRTextureType.Texture2D or XRTextureType.None)?
                    .WithMessage($"Human stencil texture must be Texture2D, but is {m_HumanStencilTextureInfo.descriptor.textureType}");

                return m_HumanStencilTextureInfo.texture as Texture2D;
            }
        }

        /// <summary>
        /// The human segmentation depth texture, if any. Otherwise, <see langword="null"/>.
        /// </summary>
        /// <value>The human segmentation depth texture.</value>
        public Texture2D humanDepthTexture
        {
            get
            {
                if (descriptor?.humanSegmentationDepthImageSupported != Supported.Supported ||
                    !subsystem.TryGetHumanDepth(out var humanDepthDescriptor))
                    return null;

                if (m_HumanDepthTextureInfo == null)
                    m_HumanDepthTextureInfo = new ARTextureInfo(humanDepthDescriptor);
                else if (!m_HumanDepthTextureInfo.TryUpdateTextureInfo(humanDepthDescriptor))
                    return null;

                DebugAssert.That(m_HumanDepthTextureInfo.descriptor.textureType is XRTextureType.Texture2D or XRTextureType.None)?
                    .WithMessage($"Human depth texture must be Texture2D, but is {m_HumanDepthTextureInfo.descriptor.textureType}");

                return m_HumanDepthTextureInfo.texture as Texture2D;
            }
        }

        /// <summary>
        /// Attempt to get the latest human stencil CPU image. This provides directly access to the raw pixel data.
        /// </summary>
        /// <remarks>
        /// The `XRCpuImage` must be disposed to avoid resource leaks.
        /// </remarks>
        /// <param name="cpuImage">An acquired `XRCpuImage`, if this method returns <see langword="true"/>.</param>
        /// <returns><see langword="true"/> if the CPU image was acquired. Otherwise, <see langword="false"/>.</returns>
        public bool TryAcquireHumanStencilCpuImage(out XRCpuImage cpuImage)
        {
            if (descriptor?.humanSegmentationStencilImageSupported == Supported.Supported)
                return subsystem.TryAcquireHumanStencilCpuImage(out cpuImage);

            cpuImage = default;
            return false;
        }

        /// <summary>
        /// Attempt to get the latest human depth CPU image. This provides direct access to the raw pixel data.
        /// </summary>
        /// <remarks>
        /// The `XRCpuImage` must be disposed to avoid resource leaks.
        /// </remarks>
        /// <param name="cpuImage">The human depth CPU image, if this method returns <see langword="true"/>.</param>
        /// <returns><see langword="true"/> if the CPU image was acquired. Otherwise, <see langword="false"/>.</returns>
        public bool TryAcquireHumanDepthCpuImage(out XRCpuImage cpuImage)
        {
            if (descriptor?.humanSegmentationDepthImageSupported == Supported.Supported)
                return subsystem.TryAcquireHumanDepthCpuImage(out cpuImage);

            cpuImage = default;
            return false;
        }

        /// <summary>
        /// Attempt to get the latest environment depth confidence CPU image. This provides direct access to the
        /// raw pixel data.
        /// </summary>
        /// <remarks>
        /// The `XRCpuImage` must be disposed to avoid resource leaks.
        /// </remarks>
        /// <param name="cpuImage">The environment depth confidence CPU image, if this method returns <see langword="true"/>.</param>
        /// <returns><see langword="true"/> if the CPU image was acquired. Otherwise, <see langword="false"/>.</returns>
        public bool TryAcquireEnvironmentDepthConfidenceCpuImage(out XRCpuImage cpuImage)
        {
            if (descriptor?.environmentDepthConfidenceImageSupported == Supported.Supported)
                return subsystem.TryAcquireEnvironmentDepthConfidenceCpuImage(out cpuImage);

            cpuImage = default;
            return false;
        }

        /// <summary>
        /// Attempt to get the latest environment depth CPU image. This provides direct access to the raw pixel data.
        /// </summary>
        /// <remarks>
        /// The `XRCpuImage` must be disposed to avoid resource leaks.
        /// </remarks>
        /// <param name="cpuImage">The environment depth CPU image, if this method returns <see langword="true"/>.</param>
        /// <returns><see langword="true"/> if the CPU image was acquired. Otherwise, <see langword="false"/>.</returns>
        public bool TryAcquireEnvironmentDepthCpuImage(out XRCpuImage cpuImage)
        {
            if (descriptor?.environmentDepthImageSupported == Supported.Supported)
                return subsystem.TryAcquireEnvironmentDepthCpuImage(out cpuImage);

            cpuImage = default;
            return false;
        }

        /// <summary>
        /// Attempt to get the latest raw environment depth CPU image. This provides direct access to the raw pixel data.
        /// </summary>
        /// <remarks>
        /// > [!NOTE]
        /// > The `XRCpuImage` must be disposed to avoid resource leaks.
        /// This differs from <see cref="TryAcquireEnvironmentDepthCpuImage"/> in that it always tries to acquire the
        /// raw environment depth image, whereas <see cref="TryAcquireEnvironmentDepthCpuImage"/> depends on the value
        /// of <see cref="environmentDepthTemporalSmoothingEnabled"/>.
        /// </remarks>
        /// <param name="cpuImage">The raw environment depth CPU image, if this method returns <see langword="true"/>.</param>
        /// <returns><see langword="true"/> if the CPU image was acquired. Otherwise, <see langword="false"/>.</returns>
        public bool TryAcquireRawEnvironmentDepthCpuImage(out XRCpuImage cpuImage)
        {
            if (subsystem == null)
            {
                cpuImage = default;
                return false;
            }

            return subsystem.TryAcquireRawEnvironmentDepthCpuImage(out cpuImage);
        }

        /// <summary>
        /// Attempt to get the latest smoothed environment depth CPU image. This provides direct access to
        /// the raw pixel data.
        /// </summary>
        /// <remarks>
        /// > [!NOTE]
        /// > The `XRCpuImage` must be disposed to avoid resource leaks.
        /// This differs from <see cref="TryAcquireEnvironmentDepthCpuImage"/> in that it always tries to acquire the
        /// smoothed environment depth image, whereas <see cref="TryAcquireEnvironmentDepthCpuImage"/>
        /// depends on the value of <see cref="environmentDepthTemporalSmoothingEnabled"/>.
        /// </remarks>
        /// <param name="cpuImage">The smoothed environment depth CPU image, if this method returns <see langword="true"/>.</param>
        /// <returns><see langword="true"/> if the CPU image was acquired. Otherwise, <see langword="false"/>.</returns>
        public bool TryAcquireSmoothedEnvironmentDepthCpuImage(out XRCpuImage cpuImage)
        {
            if (subsystem == null)
            {
                cpuImage = default;
                return false;
            }

            return subsystem.TryAcquireSmoothedEnvironmentDepthCpuImage(out cpuImage);
        }

        /// <summary>
        /// Gets environment depth texture, if possible. On OpenXR platforms, this may be a [RenderTexture](xref:UnityEngine.RenderTexture).
        /// Otherwise, the texture is of type [Texture2D](xref:UnityEngine.Texture2D).
        /// </summary>
        /// <param name="depthTexture">The output environment depth texture, if this method returns <see langword="true"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="depthTexture"/> was successfully output.
        /// Otherwise, <see langword="false"/>.</returns>
        public bool TryGetEnvironmentDepthTexture(out Texture depthTexture)
        {
            if (descriptor?.environmentDepthImageSupported != Supported.Supported ||
                !subsystem.TryGetEnvironmentDepth(out var environmentDepthDescriptor))
            {
                depthTexture = null;
                return false;
            }

            if (m_EnvironmentDepthTextureInfo == null)
            {
                m_EnvironmentDepthTextureInfo = new ARTextureInfo(environmentDepthDescriptor);
            }
            else if (!m_EnvironmentDepthTextureInfo.TryUpdateTextureInfo(environmentDepthDescriptor))
            {
                depthTexture = null;
                return false;
            }

            depthTexture = m_EnvironmentDepthTextureInfo.texture;

            var textureType = m_EnvironmentDepthTextureInfo.descriptor.textureType;
            DebugAssert.That(textureType is XRTextureType.Texture2D or XRTextureType.None || textureType.IsRenderTexture())?
                .WithMessage($"Environment depth texture must be Texture2D or RenderTexture, but was {textureType}.");

            return true;
        }

        /// <summary>
        /// Gets environment depth confidence texture, if possible. On OpenXR platforms, this may be a [RenderTexture](xref:UnityEngine.RenderTexture).
        /// Otherwise, the texture is of type [Texture2D](xref:UnityEngine.Texture2D).
        /// </summary>
        /// <param name="depthConfidenceTexture">The output environment depth confidence texture, if this method returns
        /// <see langword="true"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="depthConfidenceTexture"/> was successfully output.
        /// Otherwise, <see langword="false"/>.</returns>
        public bool TryGetEnvironmentDepthConfidenceTexture(out ARExternalTexture depthConfidenceTexture)
        {
            if (descriptor?.environmentDepthConfidenceImageSupported != Supported.Supported ||
                !subsystem.TryGetEnvironmentDepthConfidence(out var depthConfidenceDescriptor))
            {
                depthConfidenceTexture = default;
                return false;
            }

            if (m_EnvironmentDepthConfidenceTextureInfo == null)
            {
                m_EnvironmentDepthConfidenceTextureInfo = new ARTextureInfo(depthConfidenceDescriptor);
            }
            else if (!m_EnvironmentDepthConfidenceTextureInfo.TryUpdateTextureInfo(depthConfidenceDescriptor))
            {
                depthConfidenceTexture = default;
                return false;
            }

            depthConfidenceTexture = new ARExternalTexture(
                m_EnvironmentDepthConfidenceTextureInfo.texture,
                m_EnvironmentDepthConfidenceTextureInfo.descriptor.propertyNameId);

            var textureType = m_EnvironmentDepthConfidenceTextureInfo.descriptor.textureType;
            DebugAssert.That(textureType is XRTextureType.Texture2D or XRTextureType.None || textureType.IsRenderTexture())?
                .WithMessage($"Environment depth confidence texture must be Texture2D or RenderTexture, but was {textureType}.");

            return true;
        }

        /// <summary>
        /// Callback before the subsystem is started (but after it is created).
        /// </summary>
        protected override void OnBeforeStart()
        {
            requestedHumanStencilMode = m_HumanSegmentationStencilMode;
            requestedHumanDepthMode = m_HumanSegmentationDepthMode;
            requestedEnvironmentDepthMode = m_EnvironmentDepthMode;
            requestedOcclusionPreferenceMode = m_OcclusionPreferenceMode;
            environmentDepthTemporalSmoothingRequested = m_EnvironmentDepthTemporalSmoothing;

            m_TexturesReadOnly ??= new(m_Textures);
            m_PosesReadOnly ??= new(m_Poses);
            m_FovsReadOnly ??= new(m_Fovs);

            Application.onBeforeRender += OnBeforeRender;
        }

        /// <summary>
        /// Callback after the subsystem is started.
        /// </summary>
        protected override void OnAfterStart()
        {
            if (subsystem.TryGetSwapchainTextureDescriptors(out var swapchainDescriptors))
                m_SwapchainStrategy = new FixedLengthSwapchainStrategy(swapchainDescriptors);
            else
                m_SwapchainStrategy = new NoSwapchainStrategy();
        }

        /// <summary>
        /// Callback when the manager is being disabled.
        /// </summary>
        protected override void OnDisable()
        {
            Application.onBeforeRender -= OnBeforeRender;
            base.OnDisable();
            DestroyTextures();
        }

        /// <summary>
        /// Callback as the manager is being updated.
        /// </summary>
        public void Update()
        {
            requestedEnvironmentDepthMode = m_EnvironmentDepthMode;
            requestedHumanDepthMode = m_HumanSegmentationDepthMode;
            requestedHumanStencilMode = m_HumanSegmentationStencilMode;
            requestedOcclusionPreferenceMode = m_OcclusionPreferenceMode;
            environmentDepthTemporalSmoothingRequested = m_EnvironmentDepthTemporalSmoothing;
        }

        void OnBeforeRender()
        {
            if (subsystem == null)
                return;

            if (!subsystem.TryGetFrame(Allocator.Temp, out var frame))
                return;

            var descriptors = subsystem.GetTextureDescriptors(Allocator.Temp);
            if (m_SwapchainStrategy.TryUpdateTextureInfosForFrame(descriptors, out var textureInfos))
                InvokeFrameReceived(frame, textureInfos);
        }

        void DestroyTextures()
        {
            m_HumanStencilTextureInfo?.DestroyTexture();
            m_HumanDepthTextureInfo?.DestroyTexture();
            m_EnvironmentDepthTextureInfo?.DestroyTexture();
            m_EnvironmentDepthConfidenceTextureInfo?.DestroyTexture();
            m_SwapchainStrategy.Dispose();
        }

        /// <remarks>
        /// Method must be correct whether `frame` is a default value or an initialized frame.
        /// </remarks>
        void InvokeFrameReceived(XROcclusionFrame frame, ReadOnlyListSpan<ARTextureInfo> textureInfos)
        {
            if (frameReceived == null)
                return;

            m_Textures.Clear();
            int numTextureInfos = textureInfos.Count;
            if (numTextureInfos > m_Textures.Capacity)
                m_Textures.Capacity = numTextureInfos;

            for (var i = 0; i < numTextureInfos; ++i)
            {
                var textureType = textureInfos[i].descriptor.textureType;
                DebugAssert.That(textureType is XRTextureType.Texture2D || textureType.IsRenderTexture())?
                    .WithMessage($"Texture needs to be a Texture2D or RenderTexture, but is {textureType}");

                m_Textures.Add(new ARExternalTexture(textureInfos[i].texture, textureInfos[i].descriptor.propertyNameId));
            }

            m_Poses.Clear();
            if (frame.TryGetPoses(out var poses))
            {
                if (m_Poses.Capacity < poses.Length)
                    m_Poses.Capacity = poses.Length;

                foreach (Pose pose in poses)
                {
                    m_Poses.Add(pose);
                }
            }

            m_Fovs.Clear();
            if (frame.TryGetFovs(out var fovs))
            {
                if (m_Fovs.Capacity < fovs.Length)
                    m_Fovs.Capacity = fovs.Length;

                foreach (XRFov fov in fovs)
                {
                    m_Fovs.Add(fov);
                }
            }

            frame.TryGetTimestamp(out var timestampNs);
            frame.TryGetNearFarPlanes(out var planes);

            var args = new AROcclusionFrameEventArgs
            {
                shaderKeywords = subsystem.GetShaderKeywords2(),
                externalTextures = m_TexturesReadOnly,
                properties = frame.properties,
                timestamp = timestampNs,
                nearFarPlanes = planes,
                poses = m_PosesReadOnly,
                fovs = m_FovsReadOnly,
            };

            frameReceived?.Invoke(args);
        }

        /// <summary>
        /// The environment depth texture.
        /// </summary>
        /// <value>The environment depth texture, if any. Otherwise, <c>null</c>.</value>
        [Obsolete("environmentDepthTexture is deprecated in AR Foundation version 6.1. Use TryGetEnvironmentDepthTexture() instead.", false)]
        public Texture2D environmentDepthTexture
        {
            get
            {
                if (!TryGetEnvironmentDepthTexture(out var texture))
                    return null;

                return texture as Texture2D;
            }
        }

        /// <summary>
        /// The environment depth confidence texture.
        /// </summary>
        /// <value>The environment depth confidence texture, if any. Otherwise, <c>null</c>.</value>
        [Obsolete("environmentDepthConfidenceTexture is deprecated in AR Foundation version 6.1. Use TryGetEnvironmentDepthConfidenceTexture() instead.", false)]
        public Texture2D environmentDepthConfidenceTexture
        {
            get
            {
                if (!TryGetEnvironmentDepthConfidenceTexture(out var gpuTexture))
                    return null;

                return gpuTexture.texture as Texture2D;
            }
        }
    }
}
