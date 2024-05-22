using System;
using System.Collections.Generic;

namespace UnityEngine.XR.Simulation
{
    /// <summary>
    /// Marks a GameObject in a simulation environment as a source from which to provide an <see cref="UnityEngine.XR.ARSubsystems.XRAnchor"/>.
    /// This component is required by the <see cref="UnityEngine.XR.Simulation.SimulationAnchorSubsystem"/> on all GameObjects that represent
    /// a tracked anchor in a simulated environment.
    /// </summary>
    [DisallowMultipleComponent]
    public class SimulatedAnchor : MonoBehaviour
    {
        void OnEnable()
        {
            if (SimulationUtils.IsInSimulationEnvironment(gameObject))
            {
                SimulationSessionSubsystem.simulationSceneManager.TrackAnchor(this);
            }
        }

        void OnDisable()
        {
            if (SimulationUtils.IsInSimulationEnvironment(gameObject))
            {
                SimulationSessionSubsystem.simulationSceneManager.UntrackAnchor(this);
            }
        }
    }
}
