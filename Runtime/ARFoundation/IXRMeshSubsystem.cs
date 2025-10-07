using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine.Events;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.Management;
using Unity.XR.CoreUtils;
using Unity.XR.CoreUtils.Collections;

namespace UnityEngine.XR.ARFoundation
{
    /// <summary>
    /// An interface used to allow for dependency injection into ARMeshManager
    /// </summary>
    interface IXRMeshSubsystem : ISubsystem
    {
        float meshDensity { get; set; }

#if UNITY_6000_4_OR_NEWER
        bool submeshClassificationEnabled { get; set; }
#endif

        bool SetBoundingVolume(Vector3 origin, Vector3 extents);

#if UNITY_6000_4_OR_NEWER
        bool TryGetSubmeshClassifications(
            MeshId id,
            Allocator allocator,
            out uint indexVectorLength,
            out NativeArray<uint> vertexIndexVectors,
            out NativeArray<uint> classifications
        );
#endif

        void GenerateMeshAsync(
            MeshId meshId,
            Mesh mesh,
            MeshCollider meshCollider,
            MeshVertexAttributes attributes,
            Action<MeshGenerationResult> onMeshGenerationComplete,
            MeshGenerationOptions options
        );

        bool TryGetMeshInfos(List<MeshInfo> meshInfosOut);

        NativeArray<MeshTransform> GetUpdatedMeshTransforms(Allocator allocator);
    }

    /// <summary>
    /// A wrapper around XRMeshSubsystem to conform to IXRMeshSubsystem
    /// </summary>
    class XRMeshSubsystemWrapper : IXRMeshSubsystem
    {
        internal readonly XRMeshSubsystem m_Subsystem;

        public XRMeshSubsystem subsystem => m_Subsystem;

        public bool running => m_Subsystem.running;

        public float meshDensity
        {
            get => m_Subsystem.meshDensity;
            set => m_Subsystem.meshDensity = value;
        }

#if UNITY_6000_4_OR_NEWER
        public bool submeshClassificationEnabled
        {
            get => m_Subsystem.submeshClassificationEnabled;
            set => m_Subsystem.submeshClassificationEnabled = value;
        }
#endif

        public XRMeshSubsystemWrapper(XRMeshSubsystem subsystem)
        {
            m_Subsystem = subsystem;
        }

        public void Start() => m_Subsystem.Start();

        public void Stop() => m_Subsystem.Stop();

        public void Destroy() => m_Subsystem.Destroy();

        public bool SetBoundingVolume(Vector3 origin, Vector3 extents) =>
            m_Subsystem.SetBoundingVolume(origin, extents);

#if UNITY_6000_4_OR_NEWER
        public bool TryGetSubmeshClassifications(
            MeshId id,
            Allocator allocator,
            out uint indexVectorLength,
            out NativeArray<uint> vertexIndexVectors,
            out NativeArray<uint> classifications
        ) =>
            m_Subsystem.TryGetSubmeshClassifications(
                id,
                allocator,
                out indexVectorLength,
                out vertexIndexVectors,
                out classifications
            );
#endif

        public void GenerateMeshAsync(
            MeshId meshId,
            Mesh mesh,
            MeshCollider meshCollider,
            MeshVertexAttributes attributes,
            Action<MeshGenerationResult> onMeshGenerationComplete,
            MeshGenerationOptions options
        ) =>
            m_Subsystem.GenerateMeshAsync(
                meshId,
                mesh,
                meshCollider,
                attributes,
                onMeshGenerationComplete,
                options
            );

        public bool TryGetMeshInfos(List<MeshInfo> meshInfosOut) =>
            m_Subsystem.TryGetMeshInfos(meshInfosOut);

        public NativeArray<MeshTransform> GetUpdatedMeshTransforms(Allocator allocator) =>
            m_Subsystem.GetUpdatedMeshTransforms(allocator);
    }
}
