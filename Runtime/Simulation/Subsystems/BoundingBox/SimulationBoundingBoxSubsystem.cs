using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.Simulation
{
    /// <summary>
    /// Simulation implementation of <see cref="XRBoundingBoxSubsystem"/>.
    /// Do not create this directly. Use the <c>SubsystemManager</c> instead.
    /// </summary>
    public sealed class SimulationBoundingBoxSubsystem : XRBoundingBoxSubsystem
    {
        internal const string k_SubsystemId = "XRSimulation-BoundingBox";

        class SimulationProvider : Provider, ISimulationSessionResetHandler
        {
            readonly SimulationBoundingBoxDiscoverer m_Discoverer = new();

            BoundingBoxRaycaster m_BoundingBoxRaycaster;

            public override void Start()
            {
#if UNITY_EDITOR
                SimulationSubsystemAnalytics.SubsystemStarted(k_SubsystemId);
#endif
                m_Discoverer.Start();
                m_BoundingBoxRaycaster = new BoundingBoxRaycaster(GetBoundingBoxes());
                SimulationRaycasterRegistry.instance.RegisterRaycaster(m_BoundingBoxRaycaster);

                SimulationSessionSubsystem.s_SimulationSessionReset += OnSimulationSessionReset;
            }

            public override void Stop()
            {
                SimulationSessionSubsystem.s_SimulationSessionReset -= OnSimulationSessionReset;
                m_Discoverer.Stop();
            }

            public override void Destroy()
            {
                m_Discoverer.Destroy();
            }

            public void OnSimulationSessionReset()
            {
                m_Discoverer.Restart();
            }

            public override TrackableChanges<XRBoundingBox> GetChanges(
                XRBoundingBox defaultXRBoundingBox,
                Allocator allocator)
            {
                return m_Discoverer.GetChanges(defaultXRBoundingBox, allocator);
            }

            IReadOnlyCollection<XRBoundingBox> GetBoundingBoxes()
            {
                return m_Discoverer.GetBoundingBoxes();
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Register()
        {
            var cInfo = new XRBoundingBoxSubsystemDescriptor.Cinfo()
            {
                id = k_SubsystemId,
                providerType = typeof(SimulationProvider),
                subsystemTypeOverride = typeof(SimulationBoundingBoxSubsystem),
                supportsClassification = true
            };

            XRBoundingBoxSubsystemDescriptor.Register(cInfo);
        }
    }
}
