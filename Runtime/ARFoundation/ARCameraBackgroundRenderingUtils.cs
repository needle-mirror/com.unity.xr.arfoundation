using System;
using System.Runtime.CompilerServices;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation
{
    internal static class ARCameraBackgroundRenderingUtils
    {
        static bool s_InitializedFarClipMesh;
        static Mesh s_FarClipMesh;

        static Mesh fullScreenFarClipMesh
        {
            get
            {
                if (s_InitializedFarClipMesh)
                    return s_FarClipMesh;

                s_FarClipMesh = BuildFullscreenMesh(-1f);
                s_InitializedFarClipMesh = s_FarClipMesh != null;

                return s_FarClipMesh;
            }
        }

        static bool s_InitializedNearClipMesh;
        static Mesh s_NearClipMesh;

        /// <summary>
        /// A mesh that is placed near the near-clip plane
        /// </summary>
        static Mesh fullScreenNearClipMesh
        {
            get
            {
                if (s_InitializedNearClipMesh)
                    return s_NearClipMesh;

                s_NearClipMesh = BuildFullscreenMesh(0.1f);
                s_InitializedNearClipMesh = s_NearClipMesh != null;

                return s_NearClipMesh;
            }
        }

        /// <summary>
        /// The orthogonal projection matrix for the before opaque background rendering. For use when drawing the
        /// <see cref="fullScreenNearClipMesh"/>.
        /// </summary>
        static readonly Matrix4x4 beforeOpaquesOrthoProjection = Matrix4x4.Ortho(0f, 1f, 0f, 1f, -0.1f, 9.9f);

        /// <summary>
        /// The orthogonal projection matrix for the after opaque background rendering. For use when drawing the
        /// <see cref="fullScreenFarClipMesh"/>.
        /// </summary>
        static readonly Matrix4x4 afterOpaquesOrthoProjection = Matrix4x4.Ortho(0f, 1f, 0f, 1f, 0f, 1f);

        static readonly XRCameraBackgroundRenderingParams k_DefaultRenderParamsBeforeOpaques = new (
            SelectDefaultMeshForRenderMode(XRCameraBackgroundRenderingMode.BeforeOpaques),
            SelectDefaultProjectionMatrixForRenderMode(XRCameraBackgroundRenderingMode.BeforeOpaques));

        static readonly XRCameraBackgroundRenderingParams k_DefaultRenderParamsAfterOpaques = new (
            SelectDefaultMeshForRenderMode(XRCameraBackgroundRenderingMode.AfterOpaques),
            SelectDefaultProjectionMatrixForRenderMode(XRCameraBackgroundRenderingMode.AfterOpaques));

        static Mesh BuildFullscreenMesh(float zVal)
        {
            const float bottomV = 0f;
            const float topV = 1f;
            var mesh = new Mesh
            {
                vertices = new[]
                {
                    new Vector3(0f, 0f, zVal),
                    new Vector3(0f, 1f, zVal),
                    new Vector3(1f, 1f, zVal),
                    new Vector3(1f, 0f, zVal),
                },
                uv = new[]
                {
                    new Vector2(0f, bottomV),
                    new Vector2(0f, topV),
                    new Vector2(1f, topV),
                    new Vector2(1f, bottomV),
                },
                triangles = new[] { 0, 1, 2, 0, 2, 3 }
            };

            mesh.UploadMeshData(false);
            return mesh;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Mesh SelectDefaultMeshForRenderMode(XRCameraBackgroundRenderingMode currentRenderingMode)
        {
            switch (currentRenderingMode)
            {
                case XRCameraBackgroundRenderingMode.BeforeOpaques:
                    return fullScreenNearClipMesh;
                case XRCameraBackgroundRenderingMode.AfterOpaques:
                    return fullScreenFarClipMesh;
                case XRCameraBackgroundRenderingMode.None:
                default:
                    throw new ArgumentOutOfRangeException(nameof(currentRenderingMode), currentRenderingMode, null);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Matrix4x4 SelectDefaultProjectionMatrixForRenderMode(XRCameraBackgroundRenderingMode currentRenderingMode)
        {
            switch (currentRenderingMode)
            {
                case XRCameraBackgroundRenderingMode.BeforeOpaques:
                    return beforeOpaquesOrthoProjection;
                case XRCameraBackgroundRenderingMode.AfterOpaques:
                    return afterOpaquesOrthoProjection;
                case XRCameraBackgroundRenderingMode.None:
                default:
                    return Matrix4x4.identity;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static XRCameraBackgroundRenderingParams SelectDefaultBackgroundRenderParametersForRenderMode(XRCameraBackgroundRenderingMode currentRenderingMode)
        {
            return currentRenderingMode switch
            {
                XRCameraBackgroundRenderingMode.BeforeOpaques => k_DefaultRenderParamsBeforeOpaques,
                XRCameraBackgroundRenderingMode.AfterOpaques => k_DefaultRenderParamsAfterOpaques,
                XRCameraBackgroundRenderingMode.None => default,
                _ => default
            };
        }
    }
}
