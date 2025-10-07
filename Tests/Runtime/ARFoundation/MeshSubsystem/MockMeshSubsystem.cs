#if UNITY_6000_4_OR_NEWER
using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.XR.CoreUtils;
using UnityEngine.TestTools;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.Management;
using UnityEditor.XR.ARFoundation;
using LegacyMeshId = UnityEngine.XR.MeshId;

namespace UnityEngine.XR.ARSubsystems.Tests
{
    struct MockMeshInfo : IDisposable
    {
        public MeshInfo info;
        public MeshChangeState state;
        public NativeArray<uint>? vertexIndexVectors;
        public NativeArray<uint>? classifications;

        public void Dispose()
        {
            vertexIndexVectors?.Dispose();
            classifications?.Dispose();
        }
    }

    class MockMeshSubsystem : IXRMeshSubsystem, IDisposable
    {
        public float meshDensity { get; set; }
        public bool submeshClassificationEnabled { get; set; }
        public bool running => true;

        public bool SetBoundingVolume(Vector3 origin, Vector3 extents)
        {
            return true;
        }

        internal uint ElementsPerVector;
        Dictionary<MeshId, MockMeshInfo> m_CurrentMeshes = new Dictionary<MeshId, MockMeshInfo>();

        public bool TryGetMeshInfos(List<MeshInfo> meshInfosOut)
        {
            Assert.IsNotNull(meshInfosOut);

            foreach (var (_, mesh) in m_CurrentMeshes)
            {
                meshInfosOut.Add(mesh.info);
            }

            return true;
        }

        public void AddMesh(MockMeshInfo info)
        {
            m_CurrentMeshes[info.info.MeshId] = info;
        }

        public void ResetMeshes()
        {
            DisposeMeshes();
            m_CurrentMeshes.Clear();
        }

        public bool TryGetSubmeshClassifications(
            MeshId id,
            Allocator allocator,
            out uint elementsPerVector,
            out NativeArray<uint> vertexIndexVectors,
            out NativeArray<uint> classifications
        )
        {
            var mockInfo = m_CurrentMeshes[id];

            Assert.AreEqual(0, (mockInfo.vertexIndexVectors?.Length ?? 0) % (int)ElementsPerVector);

            elementsPerVector = ElementsPerVector;
            vertexIndexVectors = mockInfo.vertexIndexVectors ?? default;
            classifications = mockInfo.classifications ?? default;
            return true;
        }

        public void GenerateMeshAsync(
            MeshId meshId,
            Mesh mesh,
            MeshCollider meshCollider,
            MeshVertexAttributes attributes,
            Action<MeshGenerationResult> onMeshGenerationComplete,
            MeshGenerationOptions options
        ) { }

        public void Start() { }

        public void Stop() { }

        public void Destroy() { }

        public NativeArray<MeshTransform> GetUpdatedMeshTransforms(Allocator allocator)
        {
            return default;
        }

        public void Dispose()
        {
            DisposeMeshes();
        }

        void DisposeMeshes()
        {
            foreach (var (_, mesh) in m_CurrentMeshes)
            {
                mesh.Dispose();
            }
        }
    }
}
#endif // UNITY_6000_4_OR_NEWER
