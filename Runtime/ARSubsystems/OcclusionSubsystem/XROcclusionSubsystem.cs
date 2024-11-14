using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine.SubsystemsImplementation;

namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// Defines an interface for interacting with occlusion functionality.
    /// </summary>
    public class XROcclusionSubsystem
        : SubsystemWithProvider<XROcclusionSubsystem, XROcclusionSubsystemDescriptor, XROcclusionSubsystem.Provider>
    {
        /// <summary>
        /// Specifies the human segmentation stencil mode.
        /// </summary>
        /// <value>The human segmentation stencil mode.</value>
        /// <exception cref="System.NotSupportedException">Thrown when setting the human segmentation stencil mode to
        /// `enabled` if the implementation does not support human segmentation.</exception>
        public HumanSegmentationStencilMode requestedHumanStencilMode
        {
            get => provider.requestedHumanStencilMode;
            set => provider.requestedHumanStencilMode = value;
        }

        /// <summary>
        /// Get the current segmentation stencil mode in use by the subsystem.
        /// </summary>
        public HumanSegmentationStencilMode currentHumanStencilMode => provider.currentHumanStencilMode;

        /// <summary>
        /// Specifies the human segmentation depth mode.
        /// </summary>
        /// <value>The human segmentation depth mode.</value>
        /// <exception cref="System.NotSupportedException">Thrown when setting the human segmentation depth mode to
        /// `enabled` if the implementation does not support human segmentation.</exception>
        public HumanSegmentationDepthMode requestedHumanDepthMode
        {
            get => provider.requestedHumanDepthMode;
            set => provider.requestedHumanDepthMode = value;
        }

        /// <summary>
        /// Get the human segmentation depth mode currently in use by the provider.
        /// </summary>
        public HumanSegmentationDepthMode currentHumanDepthMode => provider.currentHumanDepthMode;

        /// <summary>
        /// Specifies the environment depth mode.
        /// </summary>
        /// <value>The environment depth mode.</value>
        public EnvironmentDepthMode requestedEnvironmentDepthMode
        {
            get => provider.requestedEnvironmentDepthMode;
            set => provider.requestedEnvironmentDepthMode = value;
        }

        /// <summary>
        /// Get the environment depth mode currently in use by the provider.
        /// </summary>
        public EnvironmentDepthMode currentEnvironmentDepthMode => provider.currentEnvironmentDepthMode;

        /// <summary>
        /// Get or set whether temporal smoothing should be applied to the environment depth image. Query for support with
        /// <see cref="XROcclusionSubsystemDescriptor.environmentDepthTemporalSmoothingSupported"/>.
        /// </summary>
        /// <value><see langword="true"/> if temporal smoothing is requested for the environment depth image.
        /// Otherwise, <see langword="false"/>.</value>
        public bool environmentDepthTemporalSmoothingRequested
        {
            get => provider.environmentDepthTemporalSmoothingRequested;
            set => provider.environmentDepthTemporalSmoothingRequested = value;
        }

        /// <summary>
        /// Get whether temporal smoothing is applied to the environment depth image. Query for support with
        /// <see cref="XROcclusionSubsystemDescriptor.environmentDepthTemporalSmoothingSupported"/>.
        /// </summary>
        /// <value><see langword="true"/> if temporal smoothing is enabled for the environment depth image.
        /// Otherwise, <see langword="false"/>.</value>
        public bool environmentDepthTemporalSmoothingEnabled => provider.environmentDepthTemporalSmoothingEnabled;

        /// <summary>
        /// Specifies the requested occlusion preference mode.
        /// </summary>
        /// <value>The requested occlusion preference mode.</value>
        public OcclusionPreferenceMode requestedOcclusionPreferenceMode
        {
            get => provider.requestedOcclusionPreferenceMode;
            set => provider.requestedOcclusionPreferenceMode = value;
        }

        /// <summary>
        /// Get the occlusion preference mode currently in use by the provider.
        /// </summary>
        /// <value>The current occlusion preference mode.</value>
        public OcclusionPreferenceMode currentOcclusionPreferenceMode => provider.currentOcclusionPreferenceMode;

        /// <summary>
        /// Construct the subsystem by creating the functionality provider.
        /// </summary>
        public XROcclusionSubsystem() { }

        /// <summary>
        /// Gets the human stencil texture descriptor.
        /// </summary>
        /// <param name="humanStencilDescriptor">The human stencil texture descriptor to be populated, if available
        /// from the provider.</param>
        /// <returns><see langword="true"/> if the human stencil texture descriptor is available and returned.
        /// Otherwise, <see langword="false"/>.</returns>
        /// <exception cref="System.NotSupportedException">Thrown if the implementation does not support human stencil
        /// texture.</exception>
        public bool TryGetHumanStencil(out XRTextureDescriptor humanStencilDescriptor)
            => provider.TryGetHumanStencil(out humanStencilDescriptor);

        /// <summary>
        /// Tries to acquire the latest human stencil CPU image.
        /// </summary>
        /// <param name="cpuImage">An acquired `XRCpuImage`, if this method returns <see langword="true"/>. The CPU image
        /// must be disposed by the caller.</param>
        /// <returns><see langword="true"/> if an image was successfully acquired. Otherwise, <see langword="false"/>.</returns>
        public bool TryAcquireHumanStencilCpuImage(out XRCpuImage cpuImage)
        {
            if (provider.humanStencilCpuImageApi != null && provider.TryAcquireHumanStencilCpuImage(out var cinfo))
            {
                cpuImage = new XRCpuImage(provider.humanStencilCpuImageApi, cinfo);
                return true;
            }

            cpuImage = default;
            return false;
        }

        /// <summary>
        /// Gets the human depth texture descriptor.
        /// </summary>
        /// <param name="humanDepthDescriptor">The human depth texture descriptor to be populated, if available from
        /// the provider.</param>
        /// <returns><see langword="true"/> if the human depth texture descriptor is available and returned.
        /// Otherwise, <see langword="false"/>.</returns>
        /// <exception cref="System.NotSupportedException">Thrown if the implementation does not support human depth
        /// texture.</exception>
        public bool TryGetHumanDepth(out XRTextureDescriptor humanDepthDescriptor)
            => provider.TryGetHumanDepth(out humanDepthDescriptor);

        /// <summary>
        /// Tries to acquire the latest human depth CPU image.
        /// </summary>
        /// <param name="cpuImage">An acquired `XRCpuImage`, if this method returns <see langword="true"/>. The CPU image
        /// must be disposed by the caller.</param>
        /// <returns><see langword="true"/> if an image was successfully acquired. Otherwise, <see langword="false"/>.</returns>
        public bool TryAcquireHumanDepthCpuImage(out XRCpuImage cpuImage)
        {
            if (provider.humanDepthCpuImageApi != null && provider.TryAcquireHumanDepthCpuImage(out var cinfo))
            {
                cpuImage = new XRCpuImage(provider.humanDepthCpuImageApi, cinfo);
                return true;
            }

            cpuImage = default;
            return false;
        }

        /// <summary>
        /// Gets the environment depth texture descriptor.
        /// </summary>
        /// <remarks>
        /// Whether the depth image is smoothed or raw depends on the value of <see cref="environmentDepthTemporalSmoothingEnabled"/>.
        /// </remarks>
        /// <param name="environmentDepthDescriptor">The environment depth texture descriptor to be populated, if
        /// available from the provider.</param>
        /// <returns><see langword="true"/> if the environment depth texture descriptor is available and returned.
        /// Otherwise, <see langword="false"/>.</returns>
        /// <exception cref="System.NotSupportedException">Thrown if the implementation does not support environment depth
        /// texture.</exception>
        public bool TryGetEnvironmentDepth(out XRTextureDescriptor environmentDepthDescriptor)
            => provider.TryGetEnvironmentDepth(out environmentDepthDescriptor);

        /// <summary>
        /// Tries to acquire the latest environment depth CPU image.
        /// </summary>
        /// <remarks>
        /// Whether the depth image is smoothed or raw depends on the value of <see cref="environmentDepthTemporalSmoothingEnabled"/>.
        /// </remarks>
        /// <param name="cpuImage">An acquired `XRCpuImage`, if this method returns <see langword="true"/>. The CPU image
        /// must be disposed by the caller.</param>
        /// <returns><see langword="true"/> if an image was successfully acquired. Otherwise, <see langword="false"/>.</returns>
        /// <seealso cref="environmentDepthTemporalSmoothingEnabled"/>
        /// <seealso cref="environmentDepthTemporalSmoothingRequested"/>
        public bool TryAcquireEnvironmentDepthCpuImage(out XRCpuImage cpuImage)
        {
            if (provider.environmentDepthCpuImageApi != null && provider.TryAcquireEnvironmentDepthCpuImage(out var cinfo))
            {
                cpuImage = new XRCpuImage(provider.environmentDepthCpuImageApi, cinfo);
                return true;
            }

            cpuImage = default;
            return false;
        }

        /// <summary>
        /// Tries to acquire the latest raw environment depth CPU image.
        /// </summary>
        /// <remarks>
        /// This method differs from <see cref="TryAcquireEnvironmentDepthCpuImage"/> in that it always tries to
        /// acquire the raw depth image, whereas the image provided by <see cref="TryAcquireEnvironmentDepthCpuImage"/>
        /// depends on the value of <see cref="environmentDepthTemporalSmoothingEnabled"/>.
        /// </remarks>
        /// <param name="cpuImage">An acquired `XRCpuImage`, if this method returns <see langword="true"/>. The CPU image
        /// must be disposed by the caller.</param>
        /// <returns><see langword="true"/> if the raw environment depth CPU image was acquired.
        /// Otherwise, <see langword="false"/>.</returns>
        public bool TryAcquireRawEnvironmentDepthCpuImage(out XRCpuImage cpuImage)
        {
            if (provider.TryAcquireRawEnvironmentDepthCpuImage(out var cinfo))
            {
                cpuImage = new XRCpuImage(provider.environmentDepthCpuImageApi, cinfo);
                return true;
            }

            cpuImage = default;
            return false;
        }

        /// <summary>
        /// Tries to acquire the latest smoothed environment depth CPU image.
        /// </summary>
        /// <remarks>
        /// This method differs from <see cref="TryAcquireEnvironmentDepthCpuImage"/> in that it always tries to
        /// acquire the temporally smoothed depth image, whereas the image provided by
        /// <see cref="TryAcquireEnvironmentDepthCpuImage"/> depends on the value of
        /// <see cref="environmentDepthTemporalSmoothingEnabled"/>.
        ///
        /// The type of smoothing applied is implementation dependent; refer to the documentation for the specific
        /// provider in use.
        /// </remarks>
        /// <param name="cpuImage">An acquired `XRCpuImage`, if this method returns <see langword="true"/>. The CPU image
        /// must be disposed by the caller.</param>
        /// <returns><see langword="true"/> if the smoothed environment depth CPU image was acquired.
        /// Otherwise, <see langword="false"/>.</returns>
        public bool TryAcquireSmoothedEnvironmentDepthCpuImage(out XRCpuImage cpuImage)
        {
            if (provider.TryAcquireSmoothedEnvironmentDepthCpuImage(out var cinfo))
            {
                cpuImage = new XRCpuImage(provider.environmentDepthCpuImageApi, cinfo);
                return true;
            }

            cpuImage = default;
            return false;
        }

        /// <summary>
        /// Gets the environment depth confidence texture descriptor.
        /// </summary>
        /// <param name="environmentDepthConfidenceDescriptor">The environment depth confidence texture descriptor to
        /// be populated, if available from the provider.</param>
        /// <returns><see langword="true"/> if the environment depth confidence texture descriptor is available and returned.
        /// Otherwise, <see langword="false"/>.</returns>
        /// <exception cref="System.NotSupportedException">Thrown if the implementation does not support environment depth
        /// confidence texture.</exception>
        public bool TryGetEnvironmentDepthConfidence(out XRTextureDescriptor environmentDepthConfidenceDescriptor)
            => provider.TryGetEnvironmentDepthConfidence(out environmentDepthConfidenceDescriptor);

        /// <summary>
        /// Tries to acquire the latest environment depth confidence CPU image.
        /// </summary>
        /// <param name="cpuImage">An acquired `XRCpuImage`, if this method returns <see langword="true"/>. The CPU image
        /// must be disposed by the caller.</param>
        /// <returns><see langword="true"/> if the image was successfully acquired. Otherwise, <see langword="false"/>.</returns>
        public bool TryAcquireEnvironmentDepthConfidenceCpuImage(out XRCpuImage cpuImage)
        {
            if (provider.environmentDepthConfidenceCpuImageApi != null && provider.TryAcquireEnvironmentDepthConfidenceCpuImage(out var cinfo))
            {
                cpuImage = new XRCpuImage(provider.environmentDepthConfidenceCpuImageApi, cinfo);
                return true;
            }

            cpuImage = default;
            return false;
        }

        /// <summary>
        /// Shader property ID for the depth view-projection matrix array.
        /// Any `ARShaderOcclusion` components in your scene will write to this property.
        /// </summary>
        public int depthViewProjectionMatricesPropertyId => provider.depthViewProjectionMatricesPropertyId;

        /// <summary>
        /// Gets the occlusion texture descriptors associated with the current AR frame.
        /// </summary>
        /// <param name="allocator">The allocator to use when creating the returned <c>NativeArray</c>.</param>
        /// <returns>An array of texture descriptors.</returns>
        /// <remarks>
        /// The caller owns the returned <c>NativeArray</c> and is responsible for calling <c>Dispose</c> on it.
        /// </remarks>
        public NativeArray<XRTextureDescriptor> GetTextureDescriptors(Allocator allocator)
            => provider.GetTextureDescriptors(default, allocator);

        /// <summary>
        /// Get the current occlusion frame.
        /// </summary>
        /// <param name="allocator">The allocator to use for any <see cref="NativeArray{T}"/>s contained in the frame.</param>
        /// <param name="frame">The output occlusion frame, if this method returned <see langword="true"/>.</param>
        /// <returns><see langword="true"/> if the method successfully got a frame. Otherwise, <see langword="false"/>.</returns>
        public bool TryGetFrame(Allocator allocator, out XROcclusionFrame frame)
        {
            if (running && provider.TryGetFrame(allocator, out frame).IsSuccess())
                return true;

            frame = default;
            return false;
        }

        /// <summary>
        /// Get the enabled and disabled shader keywords for the material.
        /// </summary>
        /// <param name="enabledKeywords">The keywords to enable for the material.</param>
        /// <param name="disabledKeywords">The keywords to disable for the material.</param>
        [Obsolete("GetMaterialKeywords(out List<string>, out List<string>) has been deprecated in AR Foundation version 6.0. Use GetShaderKeywords() instead.")]
        public void GetMaterialKeywords(out List<string> enabledKeywords, out List<string> disabledKeywords)
            => provider.GetMaterialKeywords(out enabledKeywords, out disabledKeywords);

        /// <summary>
        /// Get the enabled and disabled shader keywords for the material.
        /// </summary>
        /// <returns>Returns an <see cref="ShaderKeywords"/> with the enabled and disabled shader keywords for the Material.</returns>
        public ShaderKeywords GetShaderKeywords()
            => provider.GetShaderKeywords();

        /// <summary>
        /// Register the descriptor for the occlusion subsystem implementation.
        /// </summary>
        /// <param name="occlusionSubsystemCinfo">The occlusion subsystem implementation construction information.
        /// </param>
        /// <returns><c>true</c> if the descriptor was registered. Otherwise, <c>false</c>.</returns>
        [Obsolete("XROcclusionSubsystem.Register(XROcclusionSubsystemCinfo) has been deprecated in AR Foundation version 6.0. Use XROcclusionSubsystemDescriptor.Register(XROcclusionSubsystemDescriptor.Cinfo) instead.")]
        public static bool Register(XROcclusionSubsystemCinfo occlusionSubsystemCinfo)
        {
            var occlusionSubsystemInfo = new XROcclusionSubsystemDescriptor.Cinfo()
            {
                id = occlusionSubsystemCinfo.id,
                providerType = occlusionSubsystemCinfo.providerType,
                subsystemTypeOverride = occlusionSubsystemCinfo.subsystemTypeOverride,
                humanSegmentationStencilImageSupportedDelegate = occlusionSubsystemCinfo.humanSegmentationStencilImageSupportedDelegate,
                humanSegmentationDepthImageSupportedDelegate = occlusionSubsystemCinfo.humanSegmentationDepthImageSupportedDelegate,
                environmentDepthImageSupportedDelegate = occlusionSubsystemCinfo.environmentDepthImageSupportedDelegate,
                environmentDepthConfidenceImageSupportedDelegate = occlusionSubsystemCinfo.environmentDepthConfidenceImageSupportedDelegate,
                environmentDepthTemporalSmoothingSupportedDelegate = occlusionSubsystemCinfo.environmentDepthTemporalSmoothingSupportedDelegate
            };
            XROcclusionSubsystemDescriptor.Register(occlusionSubsystemInfo);
            return true;
        }

        /// <summary>
        /// The provider which will service the <see cref="XROcclusionSubsystem"/>.
        /// </summary>
        public abstract class Provider : SubsystemProvider<XROcclusionSubsystem>
        {
            /// <summary>
            /// Property to be implemented by the provider to get or set the requested human segmentation stencil mode.
            /// </summary>
            /// <value>
            /// The requested human segmentation stencil mode.
            /// </value>
            /// <exception cref="System.NotSupportedException">Thrown when setting the human segmentation stencil mode
            /// to `enabled` if the implementation does not support human segmentation.</exception>
            public virtual HumanSegmentationStencilMode requestedHumanStencilMode
            {
                get => HumanSegmentationStencilMode.Disabled;
                set
                {
                    if (value.Enabled())
                    {
                        throw new NotSupportedException(
                            "Setting human segmentation stencil to enabled is not supported by this implementation");
                    }
                }
            }

            /// <summary>
            /// Property to be implemented by the provider to get the segmentation stencil mode currently in use.
            /// </summary>
            public virtual HumanSegmentationStencilMode currentHumanStencilMode => HumanSegmentationStencilMode.Disabled;

            /// <summary>
            /// Property to be implemented by the provider to get or set the requested human segmentation depth mode.
            /// </summary>
            /// <value>
            /// The requested human segmentation depth mode.
            /// </value>
            /// <exception cref="System.NotSupportedException">Thrown when setting the human segmentation depth mode
            /// to `enabled` if the implementation does not support human segmentation.</exception>
            public virtual HumanSegmentationDepthMode requestedHumanDepthMode
            {
                get => HumanSegmentationDepthMode.Disabled;
                set
                {
                    if (value.Enabled())
                    {
                        throw new NotSupportedException(
                            "Setting human segmentation depth to enabled is not supported by this implementation");
                    }
                }
            }

            /// <summary>
            /// Property to be implemented by the provider to get the human segmentation depth mode currently in use.
            /// </summary>
            /// <value>The current human segmentation depth mode.</value>
            public virtual HumanSegmentationDepthMode currentHumanDepthMode => HumanSegmentationDepthMode.Disabled;

            /// <summary>
            /// Property to be implemented by the provider to get or set the environment depth mode.
            /// </summary>
            /// <value>The requested environment depth mode.</value>
            public virtual EnvironmentDepthMode requestedEnvironmentDepthMode
            {
                get => EnvironmentDepthMode.Disabled;
                set {}
            }

            /// <summary>
            /// Property to be implemented by the provider to get the environment depth mode currently in use.
            /// </summary>
            /// <value>The current environment depth mode.</value>
            public virtual EnvironmentDepthMode currentEnvironmentDepthMode => EnvironmentDepthMode.Disabled;

            /// <summary>
            /// Property to be implemented by the provider to get whether temporal smoothing has been requested for the
            /// environment depth image.
            /// </summary>
            /// <value><see langword="true"/> if environment depth temporal smoothing is requested.
            /// Otherwise, <see langword="false"/>.</value>
            public virtual bool environmentDepthTemporalSmoothingRequested
            {
                get => false;
                set {}
            }

            /// <summary>
            /// Property to be implemented by the provider to get whether temporal smoothing is currently applied to the
            /// environment depth image.
            /// </summary>
            /// <value><see langword="true"/> if environment depth temporal smoothing is enabled.
            /// Otherwise, <see langword="false"/>.</value>
            public virtual bool environmentDepthTemporalSmoothingEnabled => false;

            /// <summary>
            /// Specifies the requested occlusion preference mode.
            /// </summary>
            /// <value>The requested occlusion preference mode.</value>
            public virtual OcclusionPreferenceMode requestedOcclusionPreferenceMode
            {
                get => default;
                set {}
            }

            /// <summary>
            /// Get the occlusion preference mode currently in use by the provider.
            /// </summary>
            /// <value>The current occlusion preference mode.</value>
            public virtual OcclusionPreferenceMode currentOcclusionPreferenceMode => default;

            /// <summary>
            /// Get the current occlusion frame.
            /// </summary>
            /// <param name="allocator">The allocator to use for any <see cref="NativeArray{T}"/>s contained in the frame.</param>
            /// <param name="frame">The output occlusion frame, if this method returned <see langword="true"/>.</param>
            /// <returns><see langword="true"/> if the method successfully got a frame. Otherwise, <see langword="false"/>.</returns>
            public virtual XRResultStatus TryGetFrame(Allocator allocator, out XROcclusionFrame frame)
            {
                frame = default;
                return false;
            }

            /// <summary>
            /// Method to be implemented by the provider to get the human stencil texture descriptor.
            /// </summary>
            /// <param name="humanStencilDescriptor">The human stencil texture descriptor to be populated, if
            /// available.</param>
            /// <returns><see langword="true"/> if the human stencil texture descriptor is available and returned.
            /// Otherwise, <see langword="false"/>.</returns>
            /// <exception cref="System.NotSupportedException">Thrown if the implementation does not support human
            /// stencil texture.</exception>
            public virtual bool TryGetHumanStencil(out XRTextureDescriptor humanStencilDescriptor)
                => throw new NotSupportedException("Human stencil texture is not supported by this implementation");

            /// <summary>
            /// Tries to acquire the latest human stencil CPU image.
            /// </summary>
            /// <param name="cinfo">If this method returns `true`, this should be populated with construction
            /// information for an <see cref="XRCpuImage"/>.</param>
            /// <returns><see langword="true"/> if the human stencil CPU image was acquired. Otherwise, <see langword="false"/>.</returns>
            /// <exception cref="System.NotSupportedException">Thrown if the implementation does not support human
            /// stencil CPU images.</exception>
            public virtual bool TryAcquireHumanStencilCpuImage(out XRCpuImage.Cinfo cinfo)
                => throw new NotSupportedException("Human stencil CPU images are not supported by this implementation.");

            /// <summary>
            /// The API for interacting with an <see cref="XRCpuImage"/> acquired with <see cref="TryAcquireHumanStencilCpuImage"/>.
            /// </summary>
            /// <value>The human stencil CPU image API.</value>
            public virtual XRCpuImage.Api humanStencilCpuImageApi => null;

            /// <summary>
            /// Method to be implemented by the provider to get the human depth texture descriptor.
            /// </summary>
            /// <param name="humanDepthDescriptor">The human depth texture descriptor to be populated, if available.</param>
            /// <returns><see langword="true"/> if the human depth texture descriptor is available and returned.
            /// Otherwise, <see langword="false"/>.</returns>
            /// <exception cref="System.NotSupportedException">Thrown if the implementation does not support human
            /// depth texture.</exception>
            public virtual bool TryGetHumanDepth(out XRTextureDescriptor humanDepthDescriptor)
                => throw new NotSupportedException("Human depth texture is not supported by this implementation");

            /// <summary>
            /// Tries to acquire the latest human depth CPU image.
            /// </summary>
            /// <param name="cinfo">The construction information for an <see cref="XRCpuImage"/>, if this method
            /// returns <see langword="true"/>.</param>
            /// <returns><see langword="true"/> if the human depth CPU image was acquired.
            /// Otherwise, <see langword="false"/>.</returns>
            /// <exception cref="System.NotSupportedException">Thrown if the implementation does not support human
            /// depth CPU images.</exception>
            public virtual bool TryAcquireHumanDepthCpuImage(out XRCpuImage.Cinfo cinfo)
                => throw new NotSupportedException("Human depth CPU images are not supported by this implementation.");

            /// <summary>
            /// The API for interacting with an <see cref="XRCpuImage"/> acquired with <see cref="TryAcquireHumanDepthCpuImage"/>.
            /// </summary>
            /// <value>The human depth CPU image API.</value>
            public virtual XRCpuImage.Api humanDepthCpuImageApi => null;

            /// <summary>
            /// Method to be implemented by the provider to get the environment depth texture descriptor.
            /// </summary>
            /// <param name="environmentDepthDescriptor">The environment depth texture descriptor to be populated, if
            /// available.</param>
            /// <returns><see langword="true"/> if the environment depth texture descriptor is available and returned.
            /// Otherwise, <see langword="false"/>.</returns>
            /// <exception cref="System.NotSupportedException">Thrown if the implementation does not support environment
            /// depth texture.</exception>
            public virtual bool TryGetEnvironmentDepth(out XRTextureDescriptor environmentDepthDescriptor)
                => throw new NotSupportedException("Environment depth texture is not supported by this implementation");

            /// <summary>
            /// Tries to acquire the latest environment depth CPU image.
            /// </summary>
            /// <param name="cinfo">Construction information for an <see cref="XRCpuImage"/>, if this method returns
            /// <see langword="true"/>.</param>
            /// <returns><see langword="true"/> if the environment depth CPU image was acquired.
            /// Otherwise, <see langword="false"/>.</returns>
            /// <exception cref="System.NotSupportedException">Thrown if the implementation does not support environment
            /// CPU images.</exception>
            public virtual bool TryAcquireEnvironmentDepthCpuImage(out XRCpuImage.Cinfo cinfo)
                => throw new NotSupportedException("Environment depth CPU images are not supported by this implementation.");

            /// <summary>
            /// Tries to acquire the latest environment depth CPU image.
            /// </summary>
            /// <remarks>
            /// This method differs from <see cref="TryAcquireEnvironmentDepthCpuImage"/> in that it always tries to
            /// acquire the raw depth image, whereas the image provided by <see cref="TryAcquireEnvironmentDepthCpuImage"/>
            /// depends on the value of <see cref="environmentDepthTemporalSmoothingEnabled"/>.
            /// </remarks>
            /// <param name="cinfo">Construction information for an <see cref="XRCpuImage"/>, if this method returns
            /// <see langword="true"/>.</param>
            /// <returns><see langword="true"/> if the raw environment depth CPU image was acquired.
            /// Otherwise, <see langword="false"/>.</returns>
            public virtual bool TryAcquireRawEnvironmentDepthCpuImage(out XRCpuImage.Cinfo cinfo)
            {
                cinfo = default;
                return false;
            }

            /// <summary>
            /// Tries to acquire the latest smoothed environment depth CPU image.
            /// </summary>
            /// <remarks>
            /// This method differs from <see cref="TryAcquireEnvironmentDepthCpuImage"/> in that it always tries to
            /// acquire the smoothed depth image, whereas the image provided by
            /// <see cref="TryAcquireEnvironmentDepthCpuImage"/> depends on the value of
            /// <see cref="environmentDepthTemporalSmoothingEnabled"/>.
            /// </remarks>
            /// <param name="cinfo">Construction information for an `XRCpuImage`, if this method returns
            /// <see langword="true"/>.</param>
            /// <returns><see langword="true"/> if the smoothed environment depth CPU image was acquired.
            /// Otherwise, <see langword="false"/>.</returns>
            public virtual bool TryAcquireSmoothedEnvironmentDepthCpuImage(out XRCpuImage.Cinfo cinfo)
            {
                cinfo = default;
                return false;
            }

            /// <summary>
            /// The API for interacting with an <see cref="XRCpuImage"/> acquired with <see cref="TryAcquireEnvironmentDepthCpuImage"/>.
            /// </summary>
            /// <value>The environment depth CPU image API.</value>
            public virtual XRCpuImage.Api environmentDepthCpuImageApi => null;

            /// <summary>
            /// Method to be implemented by the provider to get the environment depth confidence texture descriptor.
            /// </summary>
            /// <param name="environmentDepthConfidenceDescriptor">The environment depth texture descriptor to be
            /// populated, if available.</param>
            /// <returns><see langword="true"/> if the environment depth confidence texture descriptor is available and returned.
            /// Otherwise, <see langword="false"/>.</returns>
            /// <exception cref="System.NotSupportedException">Thrown if the implementation does not support environment
            /// depth confidence texture.</exception>
            public virtual bool TryGetEnvironmentDepthConfidence(out XRTextureDescriptor environmentDepthConfidenceDescriptor)
                => throw new NotSupportedException("Environment depth confidence texture is not supported by this implementation");

            /// <summary>
            /// Tries to acquire the latest environment depth confidence CPU image.
            /// </summary>
            /// <param name="cinfo">Construction information for an <see cref="XRCpuImage"/>, if this method returns
            /// <see langword="true"/>.</param>
            /// <returns><see langword="true"/> if the environment depth confidence CPU image was acquired.
            /// Otherwise, <see langword="false"/>.</returns>
            /// <exception cref="System.NotSupportedException">Thrown if the implementation does not support environment
            /// depth confidence CPU images.</exception>
            public virtual bool TryAcquireEnvironmentDepthConfidenceCpuImage(out XRCpuImage.Cinfo cinfo)
                => throw new NotSupportedException("Environment depth CPU images are not supported by this implementation.");

            /// <summary>
            /// The API for interacting with an <see cref="XRCpuImage"/> acquired with <see cref="TryAcquireEnvironmentDepthConfidenceCpuImage"/>.
            /// </summary>
            /// <value>The environment depth confidence CPU image API.</value>
            public virtual XRCpuImage.Api environmentDepthConfidenceCpuImageApi => null;

            const string k_defaultDepthViewProjectionMatricesPropertyName = "_DepthVPMatrices";
            int m_depthViewProjectionMatricesPropertyId = Shader.PropertyToID(k_defaultDepthViewProjectionMatricesPropertyName);

            /// <summary>
            /// Shader property ID for the depth view-projection matrix array.
            /// Any `ARShaderOcclusion` components in your scene will write to this property.
            /// </summary>
            public virtual int depthViewProjectionMatricesPropertyId => m_depthViewProjectionMatricesPropertyId;

            /// <summary>
            /// Method to be implemented by the provider to get the occlusion texture descriptors associated with the
            /// current AR frame.
            /// </summary>
            /// <param name="defaultDescriptor">The default descriptor value.</param>
            /// <param name="allocator">The allocator to use when creating the returned <c>NativeArray</c>.</param>
            /// <returns>An array of the occlusion texture descriptors.</returns>
            public virtual NativeArray<XRTextureDescriptor> GetTextureDescriptors(
                XRTextureDescriptor defaultDescriptor, Allocator allocator)
                => new(0, allocator);

            /// <summary>
            /// Method to be implemented by the provider to get the enabled and disabled shader keywords for the material.
            /// </summary>
            /// <param name="enabledKeywords">The keywords to enable for the material.</param>
            /// <param name="disabledKeywords">The keywords to disable for the material.</param>
            [Obsolete("GetMaterialKeywords(out List<string>, out List<string>) has been deprecated in AR Foundation version 6.0. Use GetShaderKeywords() instead.")]
            public virtual void GetMaterialKeywords(out List<string> enabledKeywords, out List<string> disabledKeywords)
            {
                enabledKeywords = null;
                disabledKeywords = null;
            }

            /// <summary>
            /// Get the enabled and disabled shader keywords for the material.
            /// </summary>
            /// <returns>The enabled and disabled shader keywords for the Material.</returns>
            public virtual ShaderKeywords GetShaderKeywords()
            {
                return default;
            }
        }
    }
}
