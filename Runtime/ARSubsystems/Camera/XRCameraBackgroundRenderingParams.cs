using System;

namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// Describes the geometry and transform of the camera background for a given platform.
    /// </summary>
    public readonly struct XRCameraBackgroundRenderingParams : IEquatable<XRCameraBackgroundRenderingParams>
    {
        /// <summary>
        /// The geometry that should be used to render the camera background.
        /// </summary>
        public Mesh backgroundGeometry { get; }

        /// <summary>
        /// The transform that should be used to render the camera background.
        /// </summary>
        public Matrix4x4 backgroundTransform { get; }

        /// <summary>
        /// Constructs a <see cref="XRCameraBackgroundRenderingParams"/> from a mesh and transform.
        /// </summary>
        /// <param name="mesh">The geometry that should be used to render the camera background. Cannot be null.</param>
        /// <param name="transform">The transform that should be used to render the camera background. </param>
        public XRCameraBackgroundRenderingParams(Mesh mesh, Matrix4x4 transform)
        {
            if (mesh == null)
                throw new ArgumentNullException(nameof(mesh), "Cannot construct XRCameraBackgroundRenderingParams from null mesh");

            backgroundGeometry = mesh;
            backgroundTransform = transform;
        }

        /// <summary>
        /// Constructs a <see cref="XRCameraBackgroundRenderingParams"/> from a mesh and transform.
        /// </summary>
        /// <param name="mesh">The geometry that should be used to render the camera background. Cannot be null.</param>
        /// <param name="model">The model matrix that should be used to render the camera background.</param>
        /// <param name="view">The view matrix that should be used to render the camera background.</param>
        /// <param name="projection">The projection matrix that should be used to render the camera background.</param>
        /// <remarks>
        /// The model, view, and projection matrices are combined to form the transform.
        /// </remarks>
        public XRCameraBackgroundRenderingParams(Mesh mesh, Matrix4x4 model, Matrix4x4 view, Matrix4x4 projection)
            : this(mesh, model * view * projection)
        {
        }

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <param name="other">The other <see cref="XRCameraBackgroundRenderingParams"/> to compare against.</param>
        /// <returns><see langword="true"/> if every field in <paramref name="other"/> is equal to this <see cref="XRCameraBackgroundRenderingParams"/>. Otherwise, <see langword="false"/>.</returns>
        public bool Equals(XRCameraBackgroundRenderingParams other)
            => backgroundGeometry.Equals(other.backgroundGeometry) && backgroundTransform.Equals(other.backgroundTransform);

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <param name="obj">The `object` to compare against.</param>
        /// <returns><see langword="true"/> if <paramref name="obj"/> is of type
        /// <see cref="XRCameraBackgroundRenderingParams"/> and <see cref="Equals(XRCameraBackgroundRenderingParams)"/>
        /// also returns <see langword="true"/>. Otherwise, <see langword="false"/>.</returns>
        public override bool Equals(object obj)
            => obj is XRCameraBackgroundRenderingParams other && Equals(other);

        /// <summary>
        /// Generates a hash suitable for use with containers like `HashSet` and `Dictionary`.
        /// </summary>
        /// <returns>A hash code generated from this object's fields.</returns>
        public override int GetHashCode() => HashCode.Combine(backgroundGeometry, backgroundTransform);

        /// <summary>
        /// Tests for equality. Same as <see cref="Equals(XRCameraBackgroundRenderingParams)"/>.
        /// </summary>
        /// <param name="lhs">The left-hand side of the comparison.</param>
        /// <param name="rhs">The right-hand side of the comparison.</param>
        /// <returns><see langword="true"/> if <paramref name="lhs"/> is equal to <paramref name="rhs"/>.
        /// Otherwise, <see langword="false"/>.</returns>
        public static bool operator ==(XRCameraBackgroundRenderingParams lhs, XRCameraBackgroundRenderingParams rhs)
            => lhs.Equals(rhs);

        /// <summary>
        /// Tests for inequality. Same as `!`<see cref="Equals(XRCameraBackgroundRenderingParams)"/>.
        /// </summary>
        /// <param name="lhs">The left-hand side of the comparison.</param>
        /// <param name="rhs">The right-hand side of the comparison.</param>
        /// <returns><see langword="true"/> if <paramref name="lhs"/> is not equal to <paramref name="rhs"/>.
        /// Otherwise, <see langword="false"/>.</returns>
        public static bool operator !=(XRCameraBackgroundRenderingParams lhs, XRCameraBackgroundRenderingParams rhs)
            => !lhs.Equals(rhs);
    }
}
