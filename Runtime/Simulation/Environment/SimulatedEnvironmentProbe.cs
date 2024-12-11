namespace UnityEngine.XR.Simulation
{
    /// <summary>
    /// Marks an object in a simulation environment as a source from which to provide an environment probe.
    /// This component is required by the <see cref="SimulationEnvironmentProbeSubsystem"/> on all GameObjects
    /// that represent tracked reflection probes in an environment.
    /// </summary>
    [DisallowMultipleComponent]
    public class SimulatedEnvironmentProbe : MonoBehaviour
    {
        /// <summary>
        /// An optional pre-existing user-generated Cubemap asset. If not set, a cubemap will be generated when the environment probe is discovered during simulation.
        /// </summary>
        [SerializeField]
        [Tooltip("An optional pre-existing user-generated Cubemap asset. If not set, a cubemap will be generated when the environment probe is discovered during simulation.")]
        Cubemap m_Cubemap;

        /// <summary>
        /// Size in meters of the simulated environment probe. Sets both the box size of the underlying [Reflection Probe](xref:UnityEngine.ReflectionProbe) and the bounds for discovery.
        /// </summary>
        [SerializeField]
        [Tooltip("Size in meters of the simulated environment probe. Sets both the box size of the underlying Reflection Probe and the bounds for discovery.")]
        Vector3 m_Size = new(1.0f, 1.0f, 1.0f);

        bool m_HasChanged;

        /// <summary>
        /// Get whether the <see cref="cubemap"/> value or size have changed since the last <see cref="SimulationEnvironmentProbeSubsystem"/> update.
        /// </summary>
        /// <value><see langword="true"/> if the `cubemap` value or size have changed. Otherwise, <see langword="false"/>.</value>
        public bool hasChanged
        {
            get => m_HasChanged;
            internal set => m_HasChanged = value;
        }

        /// <summary>
        /// Get either a user-assigned cubemap reference or a generated one if loaded into the <see cref="SimulationEnvironmentProbeSubsystem"/>.
        /// </summary>
        /// <value>The <see cref="Cubemap"/> for this probe.</value>
        public Cubemap cubemap
        {
            get => m_Cubemap;
            internal set
            {
                m_Cubemap = value;
                m_HasChanged = true;
            }
        }

        /// <summary>
        /// Get the size of the simulated probe area.
        /// </summary>
        /// <value>The size.</value>
        public Vector3 size
        {
            get => m_Size;
            internal set
            {
                m_Size = value;
                m_HasChanged = true;
            }
        }
    }
}
