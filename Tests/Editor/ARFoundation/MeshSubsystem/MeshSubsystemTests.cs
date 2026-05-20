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
    class MeshSubsystemTestFixture
    {
        MockMeshSubsystem m_MockMeshSubsystem;
        ARMeshManager m_MeshManager;
        XROrigin m_XROrigin;
        List<TrackableId> m_MeshIds = new List<TrackableId>();

        [OneTimeSetUp]
        public void Setup()
        {
            m_MockMeshSubsystem = new MockMeshSubsystem();

            m_XROrigin = XROriginCreateUtil.CreateXROriginWithParent(null);
            Assert.IsNotNull(m_XROrigin);

            var arMeshManagerGameObject = new GameObject("MeshManager GameObject");
            arMeshManagerGameObject.transform.SetParent(m_XROrigin.gameObject.transform);
            arMeshManagerGameObject.SetActive(false);

            m_MeshManager = arMeshManagerGameObject.AddComponent<ARMeshManager>();
            Assert.IsNotNull(m_MeshManager);
            m_MeshManager.m_Subsystem = m_MockMeshSubsystem;
            arMeshManagerGameObject.SetActive(true);

            var meshFilterGameObject = new GameObject("MeshFilter GameObject");
            meshFilterGameObject.transform.SetParent(m_XROrigin.gameObject.transform);

            m_MeshManager.meshPrefab = meshFilterGameObject.AddComponent<MeshFilter>();
            Assert.IsNotNull(m_MeshManager.meshPrefab);

            m_MeshManager.meshInfosChanged.AddListener(infos =>
            {
                foreach (var info in infos.added)
                {
                    if (!m_MeshIds.Contains(info.id))
                    {
                        m_MeshIds.Add(info.id);
                    }
                }
                foreach (var info in infos.updated)
                {
                    if (!m_MeshIds.Contains(info.id))
                    {
                        m_MeshIds.Add(info.id);
                    }
                }
                foreach (var info in infos.removed)
                {
                    m_MeshIds.Remove(info.id);
                }
            });
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(m_XROrigin);
            m_MockMeshSubsystem?.Dispose();
            m_MockMeshSubsystem = null;
        }

        struct args
        {
            public MeshId id;
            public uint elementsPerVector;
            public uint[] vertexIndexVectors;
            public uint[] classifications;
        };

        [UnityTest]
        public IEnumerator SubmeshClassificationRetrievalOnUpdateTest()
        {
            m_MockMeshSubsystem.ElementsPerVector = 1;
            var kArgs = new args[]
            {
                new args
                {
                    id = GetLegacyMeshId(new TrackableId(0, 1)),
                    elementsPerVector = m_MockMeshSubsystem.ElementsPerVector,
                    vertexIndexVectors = new uint[] { 0, 1, 2, 3 },
                    classifications = new uint[] { 1, 2, 1, 2 }
                },
                new args
                {
                    id = GetLegacyMeshId(new TrackableId(0, 2)),
                    elementsPerVector = m_MockMeshSubsystem.ElementsPerVector,
                    vertexIndexVectors = new uint[] { 0, 1, 2, 3 },
                    classifications = new uint[] { 3, 2, 5, 2 }
                },
                new args
                {
                    id = GetLegacyMeshId(new TrackableId(0, 3)),
                    elementsPerVector = m_MockMeshSubsystem.ElementsPerVector,
                    vertexIndexVectors = new uint[] { 0, 1, 2, 3 },
                    classifications = new uint[] { 8, 6, 2, 6 }
                }
            };

            for (int i = 0; i < kArgs.Length; i++)
            {
                var argsN = kArgs[i];

                AddMesh(argsN.id, argsN.vertexIndexVectors, argsN.classifications);

                Assert.That(
                    m_MockMeshSubsystem.TryGetSubmeshClassifications(
                        argsN.id,
                        Allocator.Temp,
                        out var elementsPerVectorResult,
                        out var vertexIndexVectorsResult,
                        out var classificationsResult
                    )
                );

                Assert.AreEqual(elementsPerVectorResult, 1);

                for (var j = 0; j < argsN.vertexIndexVectors.Length; j++)
                {
                    Assert.AreEqual(argsN.vertexIndexVectors[j], vertexIndexVectorsResult[j]);
                }

                for (var j = 0; j < argsN.classifications.Length; j++)
                {
                    Assert.AreEqual(argsN.classifications[j], classificationsResult[j]);
                }

                yield return null;
            }
        }

        unsafe LegacyMeshId GetRandomMeshId()
        {
            return GetLegacyMeshId(GetRandomTrackableId());
        }

        TrackableId GetRandomTrackableId()
        {
            return new TrackableId(
                (ulong)Random.Range(0, int.MaxValue),
                (ulong)Random.Range(0, int.MaxValue)
            );
        }

        static unsafe LegacyMeshId GetLegacyMeshId(TrackableId trackableId)
        {
            return *(LegacyMeshId*)&trackableId;
        }

        static unsafe TrackableId GetTrackableId(LegacyMeshId meshId)
        {
            return *(TrackableId*)&meshId;
        }

        void AddMesh(LegacyMeshId id, uint[] indices, uint[] classifications)
        {
            var info = new MockMeshInfo();
            info.info = new MeshInfo();
            info.info.MeshId = id;
            info.info.ChangeState = MeshChangeState.Added;
            info.vertexIndexVectors = new NativeArray<uint>(indices, Allocator.Persistent);
            info.classifications = new NativeArray<uint>(classifications, Allocator.Persistent);
            m_MockMeshSubsystem.AddMesh(info);
        }
    }
}
#endif // UNITY_6000_4_OR_NEWER
