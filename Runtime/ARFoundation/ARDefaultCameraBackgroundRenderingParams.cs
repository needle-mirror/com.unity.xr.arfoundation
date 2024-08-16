using System.Runtime.CompilerServices;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation
{
    internal class ARDefaultCameraBackgroundRenderingParams
    {
        readonly XRCameraBackgroundRenderingParams m_DefaultRenderParamsAfterOpaques;
        readonly XRCameraBackgroundRenderingParams m_DefaultRenderParamsBeforeOpaques;

        internal ARDefaultCameraBackgroundRenderingParams()
        {
            m_DefaultRenderParamsAfterOpaques = new(
                BuildFullscreenMesh(-1f),
                Matrix4x4.Ortho(0f, 1f, 0f, 1f, 0f, 1f));

            m_DefaultRenderParamsBeforeOpaques = new(
                BuildFullscreenMesh(0.1f),
                Matrix4x4.Ortho(0f, 1f, 0f, 1f, -0.1f, 9.9f));
        }

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
        public XRCameraBackgroundRenderingParams SelectDefaultBackgroundRenderParametersForRenderMode(XRCameraBackgroundRenderingMode currentRenderingMode)
        {
            return currentRenderingMode switch
            {
                XRCameraBackgroundRenderingMode.AfterOpaques => m_DefaultRenderParamsAfterOpaques,
                XRCameraBackgroundRenderingMode.BeforeOpaques => m_DefaultRenderParamsBeforeOpaques,
                XRCameraBackgroundRenderingMode.None => default,
                _ => default
            };
        }
    }
}
