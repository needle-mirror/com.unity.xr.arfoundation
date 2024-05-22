using System.Collections.Generic;

namespace UnityEngine.XR.Simulation
{
    /// <summary>
    /// A component that can be used to tag a light for use in lighting estimation, for XR Simulation.
    /// </summary>
    [RequireComponent(typeof(Light))]
    [DisallowMultipleComponent]
    public class SimulatedLight : MonoBehaviour
    {
        [SerializeField, HideInInspector]
        Light m_SimulatedLight;

        /// <summary>
        /// The Light component that will be used in calculating light estimation.
        /// </summary>
        public Light simulatedLight => m_SimulatedLight;

        void Reset() => m_SimulatedLight = GetComponent<Light>();

        void Awake()
        {
            if (m_SimulatedLight == null)
            {
                m_SimulatedLight = GetComponent<Light>();
            }
        }

        void OnEnable()
        {
            if (SimulationUtils.IsInSimulationEnvironment(gameObject))
            {
                SimulationSessionSubsystem.simulationSceneManager.TrackLight(this);
            }
        }

        void OnDisable()
        {
            if (SimulationUtils.IsInSimulationEnvironment(gameObject))
            {
                SimulationSessionSubsystem.simulationSceneManager.UntrackLight(this);
            }
        }
    }
}
