using System;
using UnityEngine.SubsystemsImplementation;

namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// Contains the parameters for creating a new <see cref="XRCameraSubsystemDescriptor"/>.
    /// </summary>
    [Obsolete("XRCameraSubsystemCinfo has been deprecated in AR Foundation version 6.0. Use XRCameraSubsystemDescriptor.Cinfo instead (UnityUpgradable) -> UnityEngine.XR.ARSubsystems.XRCameraSubsystemDescriptor/Cinfo", false)]
    public struct XRCameraSubsystemCinfo : IEquatable<XRCameraSubsystemCinfo>
    {
        /// <summary>
        /// The identifier for the provider implementation of the subsystem.
        /// </summary>
        /// <value>The identifier value.</value>
        public string id { get; set; }

        /// <summary>
        /// The provider implementation type to use for instantiation.
        /// </summary>
        /// <value>The provider implementation type.</value>
        public Type providerType { get; set; }

        /// <summary>
        /// The <see cref="XRCameraSubsystem"/>-derived type to use for instantiation. The instantiated instance of this
        /// type will forward casted calls to its provider.
        /// </summary>
        /// <value>The subsystem implementation type.
        /// If <see langword="null"/>, <see cref="XRCameraSubsystem"/> will be instantiated.</value>
        public Type subsystemTypeOverride { get; set; }

        /// <summary>
        /// Indicates whether the provider implementation can provide a value for
        /// <see cref="XRCameraFrame.averageBrightness">XRCameraFrame.averageBrightness</see>.
        /// </summary>
        /// <value><see langword="true"/> if the implementation can provide average brightness.
        /// Otherwise, <see langword="false"/>.</value>
        public bool supportsAverageBrightness { get; set; }

        /// <summary>
        /// Indicates whether the provider implementation can provide a value for
        /// <see cref="XRCameraFrame.averageColorTemperature">XRCameraFrame.averageColorTemperature</see>.
        /// </summary>
        /// <value><see langword="true"/> if the implementation can provide average camera temperature.
        /// Otherwise, <see langword="false"/>.</value>
        public bool supportsAverageColorTemperature { get; set; }

        /// <summary>
        /// Indicates whether the provider implementation can provide a value for
        /// <see cref="XRCameraFrame.averageIntensityInLumens">XRCameraFrame.averageIntensityInLumens</see>.
        /// </summary>
        /// <value><see langword="true"/> if the implementation can provide average intensity in lumens.
        /// Otherwise, <see langword="false"/>.</value>
        public bool supportsAverageIntensityInLumens { get; set; }

        /// <summary>
        /// Indicates whether the provider implementation can provide a value for
        /// <see cref="XRCameraFrame.cameraGrain">XRCameraFrame.cameraGrain</see>.
        /// </summary>
        /// <value><see langword="true"/> if the implementation can provide a camera grain texture.
        /// Otherwise, <see langword="false"/>.</value>
        public bool supportsCameraGrain { get; set; }

        /// <summary>
        /// Indicates whether the provider implementation can provide a value for
        /// <see cref="XRCameraFrame.colorCorrection">XRCameraFrame.colorCorrection</see>.
        /// </summary>
        /// <value><see langword="true"/> if the implementation can provide color correction.
        /// Otherwise, <see langword="false"/>.</value>
        public bool supportsColorCorrection { get; set; }

        /// <summary>
        /// Indicates whether the provider implementation can provide a value for
        /// <see cref="XRCameraFrame.displayMatrix">XRCameraFrame.displayMatrix</see>.
        /// </summary>
        /// <value><see langword="true"/> if the implementation can provide a display matrix.
        /// Otherwise, <see langword="false"/>.</value>
        public bool supportsDisplayMatrix { get; set; }

        /// <summary>
        /// Indicates whether the provider implementation can provide a value for
        /// <see cref="XRCameraFrame.projectionMatrix">XRCameraFrame.projectionMatrix</see>.
        /// </summary>
        /// <value><see langword="true"/> if the implementation can provide a projection matrix.
        /// Otherwise, <see langword="false"/>.</value>
        public bool supportsProjectionMatrix { get; set; }

        /// <summary>
        /// Indicates whether the provider implementation can provide a value for
        /// <see cref="XRCameraFrame.timestampNs">XRCameraFrame.timestampNs</see>.
        /// </summary>
        /// <value><see langword="true"/> if the implementation can provide a timestamp.
        /// Otherwise, <see langword="false"/>.</value>
        public bool supportsTimestamp { get; set; }

        /// <summary>
        /// Indicates whether the provider implementation supports camera configurations.
        /// If <see langword="false"/>, the <c>get</c> accessor for
        /// <see cref="XRCameraSubsystem.currentConfiguration">XRCameraSubsystem.currentConfiguration</see> may return
        /// <see langword="null"/>, and the <c>set</c> accessor must throw a <see cref="NotSupportedException"/>.
        /// </summary>
        /// <value><see langword="true"/> if the implementation supports camera configurations.
        /// Otherwise, <see langword="false"/>.</value>
        public bool supportsCameraConfigurations { get; set; }

        /// <summary>
        /// Indicates whether the provider implementation can provide camera images.
        /// If <see langword="false"/>,
        /// <see cref="XRCameraSubsystem.TryAcquireLatestCpuImage">XRCameraSubsystem.TryAcquireLatestCpuImage</see>
        /// must throw a <see cref="NotSupportedException"/>.
        /// </summary>
        /// <value><see langword="true"/> if the implementation can provide camera images.
        /// Otherwise, <see langword="false"/>.</value>
        public bool supportsCameraImage { get; set; }

        /// <summary>
        /// Indicates whether the provider implementation supports the ability to set the camera's focus mode.
        /// If <see langword="false"/>,the <c>set</c> accessor for
        /// <see cref="XRCameraSubsystem.autoFocusRequested">XRCameraSubsystem.autoFocusRequested</see> will have no effect.
        /// </summary>
        /// <value><see langword="true"/> if the implementation supports focus modes. Otherwise, <see langword="false"/>.</value>
        public bool supportsFocusModes { get; set; }

        /// <summary>
        /// Indicates whether the provider implementation supports the ability to set the camera's
        /// Image Stabilization mode. If the method delegate returns <see cref="Supported.Unsupported"/>, the <c>set</c> accessor for
        /// <see cref="XRCameraSubsystem.imageStabilizationRequested">XRCameraSubsystem.imageStabilizationRequested</see>
        /// will have no effect.
        /// </summary>
        /// <value>A method delegate indicating support for image stabilization.</value>
        public Func<Supported> supportsImageStabilizationDelegate { get; set; }

        /// <summary>
        /// Indicates whether the provider implementation supports ambient intensity light estimation while face
        /// tracking is enabled.
        /// If <see langword="false"/>, <see cref="XRCameraFrame.hasAverageBrightness">XRCameraFrame.hasAverageBrightness</see>
        /// and <see cref="XRCameraFrame.hasAverageIntensityInLumens">XRCameraFrame.hasAverageIntensityInLumens</see>
        /// must be <see langword="false"/> while face tracking is enabled.
        /// </summary>
        /// <value><see langword="true"/> if the implementation supports ambient intensity while face tracking is enabled.
        /// Otherwise, <see langword="false"/>.</value>
        public bool supportsFaceTrackingAmbientIntensityLightEstimation { get; set; }

        /// <summary>
        /// Indicates whether the provider implementation supports HDR light estimation while face tracking is enabled.
        /// </summary>
        /// <value><see langword="true"/> if the implementation supports HDR light estimation while face tracking is enabled.
        /// Otherwise, <see langword="false"/>.</value>
        public bool supportsFaceTrackingHDRLightEstimation { get; set; }

        /// <summary>
        /// Indicates whether the provider implementation supports ambient intensity light estimation while world tracking.
        /// If <see langword="false"/>, <see cref="XRCameraFrame.hasAverageBrightness">XRCameraFrame.hasAverageBrightness</see>
        /// and <see cref="XRCameraFrame.hasAverageIntensityInLumens">XRCameraFrame.hasAverageIntensityInLumens</see>
        /// must be <see langword="false"/> while world tracking.
        /// </summary>
        /// <value><see langword="true"/> if the implementation supports ambient intensity while world tracking.
        /// Otherwise, <see langword="false"/>.</value>
        public bool supportsWorldTrackingAmbientIntensityLightEstimation { get; set; }

        /// <summary>
        /// Indicates whether the provider implementation supports HDR light estimation while world tracking.
        /// </summary>
        /// <value><see langword="true"/> if the implementation supports HDR light estimation while world tracking.
        /// Otherwise, <see langword="false"/>.</value>
        public bool supportsWorldTrackingHDRLightEstimation { get; set; }

        /// <summary>
        /// Indicates whether the provider implementation supports EXIF data.
        /// </summary>
        /// <value><see langword="true"/> if the implementation supports EXIF data.
        /// Otherwise, <see langword="false"/>.</value>
        public bool supportsExifData { get; set; }

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <param name="other">The other <see cref="XRCameraSubsystemCinfo"/> to compare against.</param>
        /// <returns>`True` if every field in <paramref name="other"/> is equal to this <see cref="XRCameraSubsystemCinfo"/>, otherwise false.</returns>
        public bool Equals(XRCameraSubsystemCinfo other)
        {
            return
                ReferenceEquals(id, other.id)
                && ReferenceEquals(providerType, other.providerType)
                && ReferenceEquals(subsystemTypeOverride, other.subsystemTypeOverride)
                && supportsAverageBrightness.Equals(other.supportsAverageBrightness)
                && supportsAverageColorTemperature.Equals(other.supportsAverageColorTemperature)
                && supportsColorCorrection.Equals(other.supportsColorCorrection)
                && supportsDisplayMatrix.Equals(other.supportsDisplayMatrix)
                && supportsProjectionMatrix.Equals(other.supportsProjectionMatrix)
                && supportsTimestamp.Equals(other.supportsTimestamp)
                && supportsCameraConfigurations.Equals(other.supportsCameraConfigurations)
                && supportsCameraImage.Equals(other.supportsCameraImage)
                && supportsAverageIntensityInLumens.Equals(other.supportsAverageIntensityInLumens)
                && supportsFaceTrackingAmbientIntensityLightEstimation.Equals(other.supportsFaceTrackingAmbientIntensityLightEstimation)
                && supportsFaceTrackingHDRLightEstimation.Equals(other.supportsFaceTrackingHDRLightEstimation)
                && supportsWorldTrackingAmbientIntensityLightEstimation.Equals(other.supportsWorldTrackingAmbientIntensityLightEstimation)
                && supportsWorldTrackingHDRLightEstimation.Equals(other.supportsWorldTrackingHDRLightEstimation)
                && supportsFocusModes.Equals(other.supportsFocusModes)
                && supportsCameraGrain.Equals(other.supportsCameraGrain)
                && supportsImageStabilizationDelegate.Equals(other.supportsImageStabilizationDelegate)
                && supportsExifData.Equals(other.supportsExifData);
        }

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <param name="obj">The `object` to compare against.</param>
        /// <returns>`True` if <paramref name="obj"/> is of type <see cref="XRCameraSubsystemCinfo"/> and
        /// <see cref="Equals(XRCameraSubsystemCinfo)"/> also returns `true`; otherwise `false`.</returns>
        public override bool Equals(System.Object obj)
        {
            return ((obj is XRCameraSubsystemCinfo) && Equals((XRCameraSubsystemCinfo)obj));
        }

        /// <summary>
        /// Tests for equality. Same as <see cref="Equals(XRCameraSubsystemCinfo)"/>.
        /// </summary>
        /// <param name="lhs">The left-hand side of the comparison.</param>
        /// <param name="rhs">The right-hand side of the comparison.</param>
        /// <returns>`True` if <paramref name="lhs"/> is equal to <paramref name="rhs"/>, otherwise `false`.</returns>
        public static bool operator ==(XRCameraSubsystemCinfo lhs, XRCameraSubsystemCinfo rhs) => lhs.Equals(rhs);

        /// <summary>
        /// Tests for inequality. Same as `!`<see cref="Equals(XRCameraSubsystemCinfo)"/>.
        /// </summary>
        /// <param name="lhs">The left-hand side of the comparison.</param>
        /// <param name="rhs">The right-hand side of the comparison.</param>
        /// <returns>`True` if <paramref name="lhs"/> is not equal to <paramref name="rhs"/>, otherwise `false`.</returns>
        public static bool operator !=(XRCameraSubsystemCinfo lhs, XRCameraSubsystemCinfo rhs) => !lhs.Equals(rhs);

        /// <summary>
        /// Generates a hash suitable for use with containers like `HashSet` and `Dictionary`.
        /// </summary>
        /// <returns>A hash code generated from this object's fields.</returns>
        public override int GetHashCode()
        {
            int hashCode = 486187739;
            unchecked
            {
                hashCode = (hashCode * 486187739) + HashCodeUtil.ReferenceHash(id);
                hashCode = (hashCode * 486187739) + HashCodeUtil.ReferenceHash(providerType);
                hashCode = (hashCode * 486187739) + HashCodeUtil.ReferenceHash(subsystemTypeOverride);
                hashCode = (hashCode * 486187739) + supportsAverageBrightness.GetHashCode();
                hashCode = (hashCode * 486187739) + supportsAverageColorTemperature.GetHashCode();
                hashCode = (hashCode * 486187739) + supportsColorCorrection.GetHashCode();
                hashCode = (hashCode * 486187739) + supportsDisplayMatrix.GetHashCode();
                hashCode = (hashCode * 486187739) + supportsProjectionMatrix.GetHashCode();
                hashCode = (hashCode * 486187739) + supportsTimestamp.GetHashCode();
                hashCode = (hashCode * 486187739) + supportsCameraConfigurations.GetHashCode();
                hashCode = (hashCode * 486187739) + supportsCameraImage.GetHashCode();
                hashCode = (hashCode * 486187739) + supportsAverageIntensityInLumens.GetHashCode();
                hashCode = (hashCode * 486187739) + supportsFaceTrackingAmbientIntensityLightEstimation.GetHashCode();
                hashCode = (hashCode * 486187739) + supportsFaceTrackingHDRLightEstimation.GetHashCode();
                hashCode = (hashCode * 486187739) + supportsWorldTrackingAmbientIntensityLightEstimation.GetHashCode();
                hashCode = (hashCode * 486187739) + supportsWorldTrackingHDRLightEstimation.GetHashCode();
                hashCode = (hashCode * 486187739) + supportsFocusModes.GetHashCode();
                hashCode = (hashCode * 486187739) + supportsCameraGrain.GetHashCode();
                hashCode = (hashCode * 486187739) + supportsImageStabilizationDelegate.GetHashCode();
                hashCode = (hashCode * 486187739) + supportsExifData.GetHashCode();
            }
            return hashCode;
        }
    }

    /// <summary>
    /// Specifies the functionalities supported by a provider of the <see cref="XRCameraSubsystem"/>.
    /// Provider implementations must derive from <c>XRCameraSubsystem.Provider</c> and may override virtual class members.
    /// </summary>
    public sealed class XRCameraSubsystemDescriptor :
        SubsystemDescriptorWithProvider<XRCameraSubsystem, XRCameraSubsystem.Provider>
    {
        Func<Supported> m_SupportsImageStabilizationDelegate;

        /// <summary>
        /// Indicates whether the provider implementation can provide a value for
        /// <see cref="XRCameraFrame.averageBrightness">XRCameraFrame.averageBrightness</see>.
        /// </summary>
        /// <value><see langword="true"/> if the implementation can provide average brightness.
        /// Otherwise, <see langword="false"/>.</value>
        public bool supportsAverageBrightness { get; private set; }

        /// <summary>
        /// Indicates whether the provider implementation can provide a value for
        /// <see cref="XRCameraFrame.averageColorTemperature">XRCameraFrame.averageColorTemperature</see>.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if the implementation can provide average camera temperature.
        /// Otherwise, <see langword="false"/>.</value>
        public bool supportsAverageColorTemperature { get; private set; }

        /// <summary>
        /// Indicates whether the provider implementation can provide a value for
        /// <see cref="XRCameraFrame.averageIntensityInLumens">XRCameraFrame.averageIntensityInLumens</see>.
        /// </summary>
        /// <value><see langword="true"/> if the implementation can provide average intensity in lumens.
        /// Otherwise, <see langword="false"/>.</value>
        public bool supportsAverageIntensityInLumens { get; private set; }

        /// <summary>
        /// Indicates whether the provider implementation can provide a value for
        /// <see cref="XRCameraFrame.cameraGrain">XRCameraFrame.cameraGrain</see>.
        /// </summary>
        /// <value><see langword="true"/> if the implementation can provide a camera grain texture.
        /// Otherwise, <see langword="false"/>.</value>
        public bool supportsCameraGrain { get; private set; }

        /// <summary>
        /// Indicates whether the provider implementation can provide a value for
        /// <see cref="XRCameraFrame.colorCorrection">XRCameraFrame.colorCorrection</see>.
        /// </summary>
        /// <value><see langword="true"/> if the implementation can provide color correction.
        /// Otherwise, <see langword="false"/>.</value>
        public bool supportsColorCorrection { get; private set; }

        /// <summary>
        /// Indicates whether the provider implementation can provide a value for
        /// <see cref="XRCameraFrame.displayMatrix">XRCameraFrame.displayMatrix</see>.
        /// </summary>
        /// <value><see langword="true"/> if the implementation can provide a display matrix.
        /// Otherwise, <see langword="false"/>.</value>
        public bool supportsDisplayMatrix { get; private set; }

        /// <summary>
        /// Indicates whether the provider implementation can provide a value for
        /// <see cref="XRCameraFrame.projectionMatrix">XRCameraFrame.projectionMatrix</see>.
        /// </summary>
        /// <value><see langword="true"/> if the implementation can provide a projection matrix.
        /// Otherwise, <see langword="false"/>.</value>
        public bool supportsProjectionMatrix { get; private set; }

        /// <summary>
        /// Indicates whether the provider implementation can provide a value for
        /// <see cref="XRCameraFrame.timestampNs">XRCameraFrame.timestampNs</see>.
        /// </summary>
        /// <value><see langword="true"/> if the implementation can provide a timestamp.
        /// Otherwise, <see langword="false"/>.</value>
        public bool supportsTimestamp { get; private set; }

        /// <summary>
        /// Indicates whether the provider implementation supports camera configurations.
        /// If <see langword="false"/>, the <c>get</c> accessor for
        /// <see cref="XRCameraSubsystem.currentConfiguration">XRCameraSubsystem.currentConfiguration</see> may return
        /// <see langword="null"/>, and the <c>set</c> accessor must throw a <see cref="NotSupportedException"/>.
        /// </summary>
        /// <value><see langword="true"/> if the implementation supports camera configurations.
        /// Otherwise, <see langword="false"/>.</value>
        public bool supportsCameraConfigurations { get; private set; }

        /// <summary>
        /// Indicates whether the provider implementation can provide camera images.
        /// If <see langword="false"/>,
        /// <see cref="XRCameraSubsystem.TryAcquireLatestCpuImage">XRCameraSubsystem.TryAcquireLatestCpuImage</see>
        /// must throw a <see cref="NotSupportedException"/>.
        /// </summary>
        /// <value><see langword="true"/> if the implementation can provide camera images.
        /// Otherwise, <see langword="false"/>.</value>
        public bool supportsCameraImage { get; private set; }

        /// <summary>
        /// Indicates whether the provider implementation supports the ability to set the camera's focus mode.
        /// If <see langword="false"/>, the <c>set</c> accessor for
        /// <see cref="XRCameraSubsystem.autoFocusRequested">XRCameraSubsystem.autoFocusRequested</see> will have no effect.
        /// </summary>
        /// <value><see langword="true"/> if the implementation supports focus modes.
        /// Otherwise, <see langword="false"/>.</value>
        public bool supportsFocusModes { get; private set; }

        /// <summary>
        /// Indicates whether the provider implementation supports the ability to set the camera's
        /// Image Stabilization mode. The supported status might take time to determine. If the method
        /// delegate returns <see cref="Supported.Unsupported"/>, the <c>set</c> accessor for
        /// <see cref="XRCameraSubsystem.imageStabilizationRequested">XRCameraSubsystem.imageStabilizationRequested</see>
        /// will have no effect.
        /// </summary>
        /// <value><see cref="Supported.Supported"/> if the implementation supports Image Stabilization modes,
        /// <see cref="Supported.Unknown"/> if support is still being determined.
        /// Otherwise, <see cref="Supported.Unsupported"/>.</value>
        public Supported supportsImageStabilization => m_SupportsImageStabilizationDelegate?.Invoke() ?? Supported.Unsupported;

        /// <summary>
        /// Indicates whether the provider implementation supports ambient intensity light estimation while face
        /// tracking is enabled.
        /// If <see langword="false"/>, <see cref="XRCameraFrame.hasAverageBrightness">XRCameraFrame.hasAverageBrightness</see>
        /// and <see cref="XRCameraFrame.hasAverageIntensityInLumens">XRCameraFrame.hasAverageIntensityInLumens</see>
        /// must be <see langword="false"/> while face tracking is enabled.
        /// </summary>
        /// <value><see langword="true"/> if the implementation supports ambient intensity while face tracking is enabled.
        /// Otherwise, <see langword="false"/>.</value>
        public bool supportsFaceTrackingAmbientIntensityLightEstimation { get; private set; }

        /// <summary>
        /// Indicates whether the provider implementation supports HDR light estimation while face tracking is enabled.
        /// </summary>
        /// <value><see langword="true"/> if the implementation supports HDR light estimation while face tracking is enabled.
        /// Otherwise, <see langword="false"/>.</value>
        public bool supportsFaceTrackingHDRLightEstimation { get; private set; }

        /// <summary>
        /// Indicates whether the provider implementation supports ambient intensity light estimation while world tracking.
        /// If <see langword="false"/>, <see cref="XRCameraFrame.hasAverageBrightness">XRCameraFrame.hasAverageBrightness</see>
        /// and <see cref="XRCameraFrame.hasAverageIntensityInLumens">XRCameraFrame.hasAverageIntensityInLumens</see>
        /// must be <see langword="false"/> while world tracking.
        /// </summary>
        /// <value><see langword="true"/> if the implementation supports ambient intensity while world tracking.
        /// Otherwise, <see langword="false"/>.</value>
        public bool supportsWorldTrackingAmbientIntensityLightEstimation { get; private set; }

        /// <summary>
        /// Indicates whether the provider implementation supports HDR light estimation while world tracking.
        /// </summary>
        /// <value><see langword="true"/> if the implementation supports HDR light estimation while world tracking.
        /// Otherwise, <see langword="false"/>.</value>
        public bool supportsWorldTrackingHDRLightEstimation { get; private set; }

        /// <summary>
        /// Indicates whether the provider implementation supports EXIF data.
        /// </summary>
        /// <value><see langword="true"/> if the implementation supports EXIF data.
        /// Otherwise, <see langword="false"/>.</value>
        public bool supportsExifData { get; private set; }

        /// <summary>
        /// Contains the parameters for creating a new <see cref="XRCameraSubsystemDescriptor"/>.
        /// </summary>
        public struct Cinfo : IEquatable<Cinfo>
        {
            /// <summary>
            /// The identifier for the provider implementation of the subsystem.
            /// </summary>
            /// <value>The identifier value.</value>
            public string id { get; set; }

            /// <summary>
            /// The provider implementation type to use for instantiation.
            /// </summary>
            /// <value>The provider implementation type.</value>
            public Type providerType { get; set; }

            /// <summary>
            /// The <see cref="XRCameraSubsystem"/>-derived type to use for instantiation. The instantiated instance of this
            /// type will forward casted calls to its provider.
            /// </summary>
            /// <value>The subsystem implementation type.
            /// If <see langword="null"/>, <see cref="XRCameraSubsystem"/> will be instantiated.</value>
            public Type subsystemTypeOverride { get; set; }

            /// <summary>
            /// Indicates whether the provider implementation can provide a value for
            /// <see cref="XRCameraFrame.averageBrightness">XRCameraFrame.averageBrightness</see>.
            /// </summary>
            /// <value><see langword="true"/> if the implementation can provide average brightness.
            /// Otherwise, <see langword="false"/>.</value>
            public bool supportsAverageBrightness { get; set; }

            /// <summary>
            /// Indicates whether the provider implementation can provide a value for
            /// <see cref="XRCameraFrame.averageColorTemperature">XRCameraFrame.averageColorTemperature</see>.
            /// </summary>
            /// <value><see langword="true"/> if the implementation can provide average camera temperature.
            /// Otherwise, <see langword="false"/>.</value>
            public bool supportsAverageColorTemperature { get; set; }

            /// <summary>
            /// Indicates whether the provider implementation can provide a value for
            /// <see cref="XRCameraFrame.averageIntensityInLumens">XRCameraFrame.averageIntensityInLumens</see>.
            /// </summary>
            /// <value><see langword="true"/> if the implementation can provide average intensity in lumens.
            /// Otherwise, <see langword="false"/>.</value>
            public bool supportsAverageIntensityInLumens { get; set; }

            /// <summary>
            /// Indicates whether the provider implementation can provide a value for
            /// <see cref="XRCameraFrame.cameraGrain">XRCameraFrame.cameraGrain</see>.
            /// </summary>
            /// <value><see langword="true"/> if the implementation can provide a camera grain texture.
            /// Otherwise, <see langword="false"/>.</value>
            public bool supportsCameraGrain { get; set; }

            /// <summary>
            /// Indicates whether the provider implementation can provide a value for
            /// <see cref="XRCameraFrame.colorCorrection">XRCameraFrame.colorCorrection</see>.
            /// </summary>
            /// <value><see langword="true"/> if the implementation can provide color correction.
            /// Otherwise, <see langword="false"/>.</value>
            public bool supportsColorCorrection { get; set; }

            /// <summary>
            /// Indicates whether the provider implementation can provide a value for
            /// <see cref="XRCameraFrame.displayMatrix">XRCameraFrame.displayMatrix</see>.
            /// </summary>
            /// <value><see langword="true"/> if the implementation can provide a display matrix.
            /// Otherwise, <see langword="false"/>.</value>
            public bool supportsDisplayMatrix { get; set; }

            /// <summary>
            /// Indicates whether the provider implementation can provide a value for
            /// <see cref="XRCameraFrame.projectionMatrix">XRCameraFrame.projectionMatrix</see>.
            /// </summary>
            /// <value><see langword="true"/> if the implementation can provide a projection matrix.
            /// Otherwise, <see langword="false"/>.</value>
            public bool supportsProjectionMatrix { get; set; }

            /// <summary>
            /// Indicates whether the provider implementation can provide a value for
            /// <see cref="XRCameraFrame.timestampNs">XRCameraFrame.timestampNs</see>.
            /// </summary>
            /// <value><see langword="true"/> if the implementation can provide a timestamp.
            /// Otherwise, <see langword="false"/>.</value>
            public bool supportsTimestamp { get; set; }

            /// <summary>
            /// Indicates whether the provider implementation supports camera configurations.
            /// If <see langword="false"/>, the <c>get</c> accessor for
            /// <see cref="XRCameraSubsystem.currentConfiguration">XRCameraSubsystem.currentConfiguration</see> may return
            /// <see langword="null"/>, and the <c>set</c> accessor must throw a <see cref="NotSupportedException"/>.
            /// </summary>
            /// <value><see langword="true"/> if the implementation supports camera configurations.
            /// Otherwise, <see langword="false"/>.</value>
            public bool supportsCameraConfigurations { get; set; }

            /// <summary>
            /// Indicates whether the provider implementation can provide camera images.
            /// If <see langword="false"/>,
            /// <see cref="XRCameraSubsystem.TryAcquireLatestCpuImage">XRCameraSubsystem.TryAcquireLatestCpuImage</see>
            /// must throw a <see cref="NotSupportedException"/>.
            /// </summary>
            /// <value><see langword="true"/> if the implementation can provide camera images.
            /// Otherwise, <see langword="false"/>.</value>
            public bool supportsCameraImage { get; set; }

            /// <summary>
            /// Indicates whether the provider implementation supports the ability to set the camera's focus mode.
            /// If <see langword="false"/>,the <c>set</c> accessor for
            /// <see cref="XRCameraSubsystem.autoFocusRequested">XRCameraSubsystem.autoFocusRequested</see> will have no effect.
            /// </summary>
            /// <value><see langword="true"/> if the implementation supports focus modes. Otherwise, <see langword="false"/>.</value>
            public bool supportsFocusModes { get; set; }

            /// <summary>
            /// Indicates whether the provider implementation supports the ability to set the camera's
            /// Image Stabilization mode. If the method delegate returns <see cref="Supported.Unsupported"/>, the <c>set</c> accessor for
            /// <see cref="XRCameraSubsystem.imageStabilizationRequested">XRCameraSubsystem.imageStabilizationRequested</see>
            /// will have no effect.
            /// </summary>
            /// <value>A method delegate indicating support for image stabilization.</value>
            public Func<Supported> supportsImageStabilizationDelegate { get; set; }

            /// <summary>
            /// Indicates whether the provider implementation supports ambient intensity light estimation while face
            /// tracking is enabled.
            /// If <see langword="false"/>, <see cref="XRCameraFrame.hasAverageBrightness">XRCameraFrame.hasAverageBrightness</see>
            /// and <see cref="XRCameraFrame.hasAverageIntensityInLumens">XRCameraFrame.hasAverageIntensityInLumens</see>
            /// must be <see langword="false"/> while face tracking is enabled.
            /// </summary>
            /// <value><see langword="true"/> if the implementation supports ambient intensity while face tracking is enabled.
            /// Otherwise, <see langword="false"/>.</value>
            public bool supportsFaceTrackingAmbientIntensityLightEstimation { get; set; }

            /// <summary>
            /// Indicates whether the provider implementation supports HDR light estimation while face tracking is enabled.
            /// </summary>
            /// <value><see langword="true"/> if the implementation supports HDR light estimation while face tracking is enabled.
            /// Otherwise, <see langword="false"/>.</value>
            public bool supportsFaceTrackingHDRLightEstimation { get; set; }

            /// <summary>
            /// Indicates whether the provider implementation supports ambient intensity light estimation while world tracking.
            /// If <see langword="false"/>, <see cref="XRCameraFrame.hasAverageBrightness">XRCameraFrame.hasAverageBrightness</see>
            /// and <see cref="XRCameraFrame.hasAverageIntensityInLumens">XRCameraFrame.hasAverageIntensityInLumens</see>
            /// must be <see langword="false"/> while world tracking.
            /// </summary>
            /// <value><see langword="true"/> if the implementation supports ambient intensity while world tracking.
            /// Otherwise, <see langword="false"/>.</value>
            public bool supportsWorldTrackingAmbientIntensityLightEstimation { get; set; }

            /// <summary>
            /// Indicates whether the provider implementation supports HDR light estimation while world tracking.
            /// </summary>
            /// <value><see langword="true"/> if the implementation supports HDR light estimation while world tracking.
            /// Otherwise, <see langword="false"/>.</value>
            public bool supportsWorldTrackingHDRLightEstimation { get; set; }

            /// <summary>
            /// Indicates whether the provider implementation supports EXIF data.
            /// </summary>
            /// <value><see langword="true"/> if the implementation supports EXIF data.
            /// Otherwise, <see langword="false"/>.</value>
            public bool supportsExifData { get; set; }

            /// <summary>
            /// Tests for equality.
            /// </summary>
            /// <param name="other">The other <see cref="Cinfo"/> to compare against.</param>
            /// <returns>`True` if every field in <paramref name="other"/> is equal to this <see cref="Cinfo"/>, otherwise false.</returns>
            public bool Equals(Cinfo other)
            {
                return
                    ReferenceEquals(id, other.id) &&
                    ReferenceEquals(providerType, other.providerType) &&
                    ReferenceEquals(subsystemTypeOverride, other.subsystemTypeOverride) &&
                    supportsAverageBrightness.Equals(other.supportsAverageBrightness) &&
                    supportsAverageColorTemperature.Equals(other.supportsAverageColorTemperature) &&
                    supportsColorCorrection.Equals(other.supportsColorCorrection) &&
                    supportsDisplayMatrix.Equals(other.supportsDisplayMatrix) &&
                    supportsProjectionMatrix.Equals(other.supportsProjectionMatrix) &&
                    supportsTimestamp.Equals(other.supportsTimestamp) &&
                    supportsCameraConfigurations.Equals(other.supportsCameraConfigurations) &&
                    supportsCameraImage.Equals(other.supportsCameraImage) &&
                    supportsAverageIntensityInLumens.Equals(other.supportsAverageIntensityInLumens) &&
                    supportsFaceTrackingAmbientIntensityLightEstimation.Equals(other.supportsFaceTrackingAmbientIntensityLightEstimation) &&
                    supportsFaceTrackingHDRLightEstimation.Equals(other.supportsFaceTrackingHDRLightEstimation) &&
                    supportsWorldTrackingAmbientIntensityLightEstimation.Equals(other.supportsWorldTrackingAmbientIntensityLightEstimation) &&
                    supportsWorldTrackingHDRLightEstimation.Equals(other.supportsWorldTrackingHDRLightEstimation) &&
                    supportsFocusModes.Equals(other.supportsFocusModes) &&
                    supportsCameraGrain.Equals(other.supportsCameraGrain) &&
                    supportsImageStabilizationDelegate.Equals(other.supportsImageStabilizationDelegate) &&
                    supportsExifData.Equals(other.supportsExifData);
            }

            /// <summary>
            /// Tests for equality.
            /// </summary>
            /// <param name="obj">The `object` to compare against.</param>
            /// <returns>`True` if <paramref name="obj"/> is of type <see cref="Cinfo"/> and
            /// <see cref="Equals(Cinfo)"/> also returns `true`; otherwise `false`.</returns>
            public override bool Equals(System.Object obj)
            {
                return ((obj is Cinfo) && Equals((Cinfo)obj));
            }

            /// <summary>
            /// Tests for equality. Same as <see cref="Equals(Cinfo)"/>.
            /// </summary>
            /// <param name="lhs">The left-hand side of the comparison.</param>
            /// <param name="rhs">The right-hand side of the comparison.</param>
            /// <returns>`True` if <paramref name="lhs"/> is equal to <paramref name="rhs"/>, otherwise `false`.</returns>
            public static bool operator ==(Cinfo lhs, Cinfo rhs) => lhs.Equals(rhs);

            /// <summary>
            /// Tests for inequality. Same as `!`<see cref="Equals(Cinfo)"/>.
            /// </summary>
            /// <param name="lhs">The left-hand side of the comparison.</param>
            /// <param name="rhs">The right-hand side of the comparison.</param>
            /// <returns>`True` if <paramref name="lhs"/> is not equal to <paramref name="rhs"/>, otherwise `false`.</returns>
            public static bool operator !=(Cinfo lhs, Cinfo rhs) => !lhs.Equals(rhs);

            /// <summary>
            /// Generates a hash suitable for use with containers like `HashSet` and `Dictionary`.
            /// </summary>
            /// <returns>A hash code generated from this object's fields.</returns>
            public override int GetHashCode()
            {
                int hashCode = 486187739;
                unchecked
                {
                    hashCode = (hashCode * 486187739) + HashCodeUtil.ReferenceHash(id);
                    hashCode = (hashCode * 486187739) + HashCodeUtil.ReferenceHash(providerType);
                    hashCode = (hashCode * 486187739) + HashCodeUtil.ReferenceHash(subsystemTypeOverride);
                    hashCode = (hashCode * 486187739) + supportsAverageBrightness.GetHashCode();
                    hashCode = (hashCode * 486187739) + supportsAverageColorTemperature.GetHashCode();
                    hashCode = (hashCode * 486187739) + supportsColorCorrection.GetHashCode();
                    hashCode = (hashCode * 486187739) + supportsDisplayMatrix.GetHashCode();
                    hashCode = (hashCode * 486187739) + supportsProjectionMatrix.GetHashCode();
                    hashCode = (hashCode * 486187739) + supportsTimestamp.GetHashCode();
                    hashCode = (hashCode * 486187739) + supportsCameraConfigurations.GetHashCode();
                    hashCode = (hashCode * 486187739) + supportsCameraImage.GetHashCode();
                    hashCode = (hashCode * 486187739) + supportsAverageIntensityInLumens.GetHashCode();
                    hashCode = (hashCode * 486187739) + supportsFaceTrackingAmbientIntensityLightEstimation.GetHashCode();
                    hashCode = (hashCode * 486187739) + supportsFaceTrackingHDRLightEstimation.GetHashCode();
                    hashCode = (hashCode * 486187739) + supportsWorldTrackingAmbientIntensityLightEstimation.GetHashCode();
                    hashCode = (hashCode * 486187739) + supportsWorldTrackingHDRLightEstimation.GetHashCode();
                    hashCode = (hashCode * 486187739) + supportsFocusModes.GetHashCode();
                    hashCode = (hashCode * 486187739) + supportsCameraGrain.GetHashCode();
                    hashCode = (hashCode * 486187739) + supportsImageStabilizationDelegate.GetHashCode();
                    hashCode = (hashCode * 486187739) + supportsExifData.GetHashCode();
                }
                return hashCode;
            }
        }

        /// <summary>
        /// Registers a camera subsystem implementation based on the given subsystem parametersㄝ and validates that the
        /// <see cref="Cinfo.id"/> and <see cref="Cinfo.providerType"/>
        /// properties are properly specified.
        /// </summary>
        /// <param name="cinfo">The parameters that define how to initialize the descriptor.</param>
        /// <exception cref="System.ArgumentException">Thrown when the values specified in the
        /// <see cref="Cinfo"/> parameter are invalid. Typically, this happens when
        /// required parameters are <see langword="null"/> or empty or types that do not derive from the required base class.
        /// </exception>
        public static void Register(Cinfo cinfo)
        {
            if (string.IsNullOrEmpty(cinfo.id))
            {
                throw new ArgumentException(
                    "Cannot create camera subsystem descriptor because id is invalid",
                    nameof(cinfo));
            }

            if (cinfo.providerType == null
                || !cinfo.providerType.IsSubclassOf(typeof(XRCameraSubsystem.Provider)))
            {
                throw new ArgumentException(
                    "Cannot create camera subsystem descriptor because providerType is invalid",
                    nameof(cinfo));
            }

            if (cinfo.subsystemTypeOverride != null
                && !cinfo.subsystemTypeOverride.IsSubclassOf(typeof(XRCameraSubsystem)))
            {
                throw new ArgumentException(
                    "Cannot create camera subsystem descriptor because subsystemTypeOverride is invalid",
                    nameof(cinfo));
            }

            SubsystemDescriptorStore.RegisterDescriptor(new XRCameraSubsystemDescriptor(cinfo));
        }

        /// <summary>
        /// Construct an instance.
        /// </summary>
        /// <param name="cinfo">The parameters required to initialize the descriptor.</param>
        XRCameraSubsystemDescriptor(Cinfo cinfo)
        {
            id = cinfo.id;
            providerType = cinfo.providerType;
            subsystemTypeOverride = cinfo.subsystemTypeOverride;
            supportsAverageBrightness = cinfo.supportsAverageBrightness;
            supportsAverageColorTemperature = cinfo.supportsAverageColorTemperature;
            supportsColorCorrection = cinfo.supportsColorCorrection;
            supportsDisplayMatrix = cinfo.supportsDisplayMatrix;
            supportsProjectionMatrix = cinfo.supportsProjectionMatrix;
            supportsTimestamp = cinfo.supportsTimestamp;
            supportsCameraConfigurations = cinfo.supportsCameraConfigurations;
            supportsCameraImage = cinfo.supportsCameraImage;
            supportsAverageIntensityInLumens = cinfo.supportsAverageIntensityInLumens;
            supportsFocusModes = cinfo.supportsFocusModes;
            supportsFaceTrackingAmbientIntensityLightEstimation = cinfo.supportsFaceTrackingAmbientIntensityLightEstimation;
            supportsFaceTrackingHDRLightEstimation = cinfo.supportsFaceTrackingHDRLightEstimation;
            supportsWorldTrackingAmbientIntensityLightEstimation = cinfo.supportsWorldTrackingAmbientIntensityLightEstimation;
            supportsWorldTrackingHDRLightEstimation = cinfo.supportsWorldTrackingHDRLightEstimation;
            supportsCameraGrain = cinfo.supportsCameraGrain;
            m_SupportsImageStabilizationDelegate = cinfo.supportsImageStabilizationDelegate;
            supportsExifData = cinfo.supportsExifData;
        }
    }
}
