using System;
using Unity.Collections;
using Unity.XR.CoreUtils;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.Simulation
{
    /// <summary>
    /// Simulation implementation of
    /// [XRPointCloudSubsystem](xref:UnityEngine.XR.ARSubsystems.XRPointCloudSubsystem).
    /// Do not create this directly. Use the <c>SubsystemManager</c> instead.
    /// </summary>
    public sealed class SimulationPointCloudSubsystem : XRPointCloudSubsystem
    {
        internal const string k_SubsystemId = "XRSimulation-PointCloud";

        /// <summary>
        /// Invoked just after the subsystem provider starts.
        /// </summary>
        protected override void OnStart()
        {
            SimulationEnvironmentScanner.RegisterSubsystem(this);
            base.OnStart();
        }

        /// <summary>
        /// Invoked just after the subsystem provider stops.
        /// </summary>
        protected override void OnStop()
        {
            SimulationEnvironmentScanner.UnregisterSubsystem(this);
            base.OnStop();
        }

        class SimulationProvider : Provider
        {
            const int k_DefaultConversionBufferCapacity = 8;

            TrackableId m_TrackableId;
            TrackableId m_PreviousTrackableId;

            NativeArray<ulong> m_Identifiers;
            NativeArray<Vector3> m_Positions;
            NativeArray<float> m_ConfidenceValues;

            int m_PointCount;
            PointCloudRaycaster m_Raycaster;
            ulong m_PointIdentifier;

            float m_LastScanTime;

            public override void Start()
            {
#if UNITY_EDITOR
                SimulationSubsystemAnalytics.SubsystemStarted(k_SubsystemId);
#endif

                m_TrackableId = GenerateTrackableId();
                m_PointIdentifier = 0;

                m_PreviousTrackableId = TrackableId.invalidId;

                m_Identifiers = new NativeArray<ulong>(k_DefaultConversionBufferCapacity, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
                m_Positions = new NativeArray<Vector3>(k_DefaultConversionBufferCapacity, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
                m_ConfidenceValues = new NativeArray<float>(k_DefaultConversionBufferCapacity, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
                m_Raycaster = new PointCloudRaycaster(m_TrackableId, GetReadOnlyPositions);
            }

            public override void Stop()
            {
                m_TrackableId = TrackableId.invalidId;

                m_Raycaster.Stop();
                m_Raycaster = null;
                m_Identifiers.Dispose();
                m_Positions.Dispose();
                m_ConfidenceValues.Dispose();
            }

            public override TrackableChanges<XRPointCloud> GetChanges(XRPointCloud defaultPointCloud, Allocator allocator)
            {
                UpdatePointCloudData();

                NativeArray<XRPointCloud> added = new NativeArray<XRPointCloud>(0, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
                NativeArray<XRPointCloud> updated = new NativeArray<XRPointCloud>(0, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
                NativeArray<TrackableId> removed = new NativeArray<TrackableId>(0, Allocator.Temp, NativeArrayOptions.UninitializedMemory);

                TrackableChanges<XRPointCloud> changes = new TrackableChanges<XRPointCloud>();

                try
                {
                    if (m_PreviousTrackableId != m_TrackableId)
                    {
                        if (m_PreviousTrackableId != TrackableId.invalidId)
                        {
                            removed.Dispose();
                            removed = new NativeArray<TrackableId>(1, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
                            removed[0] = m_PreviousTrackableId;
                        }

                        added.Dispose();
                        added = new NativeArray<XRPointCloud>(1, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
                        added[0] = new XRPointCloud(
                            m_TrackableId,
                            Pose.identity,
                            TrackingState.Tracking,
                            IntPtr.Zero);

                        m_PreviousTrackableId = m_TrackableId;
                    }
                    else
                    {
                        updated.Dispose();
                        updated = new NativeArray<XRPointCloud>(1, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
                        updated[0] = new XRPointCloud(
                            m_TrackableId,
                            Pose.identity,
                            TrackingState.Tracking,
                            IntPtr.Zero);
                    }

                    changes = TrackableChanges<XRPointCloud>.CopyFrom(added, updated, removed, allocator);
                }
                finally
                {
                    added.Dispose();
                    updated.Dispose();
                    removed.Dispose();
                }

                return changes;
            }

            public override XRPointCloudData GetPointCloudData(TrackableId trackableId, Allocator allocator)
            {
                if (trackableId != m_TrackableId || m_PointCount <= 0)
                    return default;

                var result = new XRPointCloudData();
                if (m_PointCount > 0)
                {
                    result.identifiers = new NativeArray<ulong>(m_PointCount, allocator, NativeArrayOptions.UninitializedMemory);
                    NativeArray<ulong>.Copy(m_Identifiers, result.identifiers, m_PointCount);

                    result.positions = new NativeArray<Vector3>(m_PointCount, allocator, NativeArrayOptions.UninitializedMemory);
                    NativeArray<Vector3>.Copy(m_Positions, result.positions, m_PointCount);

                    result.confidenceValues = new NativeArray<float>(m_PointCount, allocator, NativeArrayOptions.UninitializedMemory);
                    NativeArray<float>.Copy(m_ConfidenceValues, result.confidenceValues, m_PointCount);
                }

                return result;
            }

            void UpdatePointCloudData()
            {
                var currentScanTime = SimulationEnvironmentScanner.instance.lastScanTime;
                if (currentScanTime <= m_LastScanTime)
                    return;

                var points = SimulationEnvironmentScanner.instance.GetPoints(Allocator.Temp);

                try
                {
                    m_PointCount = points.Length;

                    NativeArrayUtils.EnsureCapacity(ref m_Identifiers, m_PointCount, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
                    NativeArrayUtils.EnsureCapacity(ref m_Positions, m_PointCount, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
                    NativeArrayUtils.EnsureCapacity(ref m_ConfidenceValues, m_PointCount, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);

                    for (var i = 0; i < m_PointCount; i++)
                    {
                        m_Identifiers[i] = m_PointIdentifier + (ulong)i;
                        m_Positions[i] = points[i];
                        m_ConfidenceValues[i] = Random.Range(0f, 1f);
                    }

                    m_PointIdentifier += (ulong)m_PointCount;
                    m_LastScanTime = currentScanTime;
                }
                finally
                {
                    points.Dispose();
                }
            }

            NativeArray<Vector3>.ReadOnly GetReadOnlyPositions() => m_Positions.AsReadOnly();

            static TrackableId GenerateTrackableId()
            {
                Guid.NewGuid()
                    .Decompose(out var subId1, out var subId2);

                return new TrackableId(subId1, subId2);
            }
        }

        // this method is run on startup of the app to register this provider with XR Subsystem Manager
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void RegisterDescriptor()
        {
            var cinfo = new XRPointCloudSubsystemDescriptor.Cinfo
            {
                id = k_SubsystemId,
                providerType = typeof(SimulationPointCloudSubsystem.SimulationProvider),
                subsystemTypeOverride = typeof(SimulationPointCloudSubsystem),
                supportsFeaturePoints = true,
                supportsUniqueIds = true,
                supportsConfidence = true
            };

            XRPointCloudSubsystemDescriptor.RegisterDescriptor(cinfo);
        }
    }
}
