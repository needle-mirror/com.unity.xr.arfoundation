using System;
using System.Collections.Generic;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation
{
    /// <summary>
    /// This class creates, maintains, and destroys environment probe GameObject components as the
    /// <c>XREnvironmentProbeSubsystem</c> provides updates from environment probes as they are detected in the
    /// environment.
    /// </summary>
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(ARUpdateOrder.k_EnvironmentProbeManager)]
    [HelpURL(typeof(AREnvironmentProbeManager))]
    public sealed class AREnvironmentProbeManager : ARTrackableManager<
        XREnvironmentProbeSubsystem,
        XREnvironmentProbeSubsystemDescriptor,
        XREnvironmentProbeSubsystem.Provider,
        XREnvironmentProbe,
        AREnvironmentProbe>
    {
        bool supportsAutomaticPlacement => descriptor?.supportsAutomaticPlacement == true;

        /// <summary>
        /// If enabled, requests automatic generation of environment probes for the scene.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if automatic environment probe placement is requested. Otherwise, <see langword="false"/>.
        /// </value>
        public bool automaticPlacementRequested
        {
            get => supportsAutomaticPlacement ? subsystem.automaticPlacementRequested : m_AutomaticPlacement;
            set
            {
                m_AutomaticPlacement = value;
                SetAutomaticPlacementStateOnSubsystem();
             }
        }

        /// <summary>
        /// <see langword="true"/> if automatic placement is enabled on the subsystem.
        /// </summary>
        public bool automaticPlacementEnabled => supportsAutomaticPlacement && subsystem.automaticPlacementEnabled;

        [SerializeField]
        [Tooltip("Whether environment probes should be automatically placed in the environment (if supported).")]
        bool m_AutomaticPlacement = true;

        /// <summary>
        /// Specifies the texture filter mode to be used with the environment texture.
        /// </summary>
        /// <value>
        /// The texture filter mode.
        /// </value>
        public FilterMode environmentTextureFilterMode
        {
            get => m_EnvironmentTextureFilterMode;
            set => m_EnvironmentTextureFilterMode = value;
        }

        [SerializeField]
        [Tooltip("The texture filter mode to be used with the reflection probe environment texture.")]
        FilterMode m_EnvironmentTextureFilterMode = FilterMode.Trilinear;

        [SerializeField]
        [Tooltip("Whether the environment textures should be returned as HDR textures.")]
        bool m_EnvironmentTextureHDR = true;

        bool supportsEnvironmentTextureHDR => descriptor?.supportsEnvironmentTextureHDR == true;

        /// <summary>
        /// Get or set whether high dynamic range environment textures are requested.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if high dynamic range environment textures are requested. Otherwise, <see langword="false"/>.
        /// </value>
        public bool environmentTextureHDRRequested
        {
            get => supportsEnvironmentTextureHDR ? subsystem.environmentTextureHDRRequested : m_EnvironmentTextureHDR;
            set
            {
                m_EnvironmentTextureHDR = value;
                SetEnvironmentTextureHDRStateOnSubsystem();
            }
        }

        /// <summary>
        /// Queries whether environment textures are provided with high dynamic range (HDR).
        /// </summary>
        /// <value>
        /// <see langword="true"/> if environment textures are HDR. Otherwise, <see langword="false"/>.
        /// </value>
        public bool environmentTextureHDREnabled => supportsEnvironmentTextureHDR && subsystem.environmentTextureHDREnabled;

        /// <summary>
        /// Specifies a debug prefab that will be attached to all environment probes.
        /// </summary>
        /// <value>The debug prefab.</value>
        /// <remarks>
        /// Setting a debug prefab allows for environment probes to be more readily visualized, but is not
        /// required for normal operation of this manager. This script will automatically create reflection probes for
        /// all environment probes reported by the <see cref="XREnvironmentProbeSubsystem"/>.
        /// </remarks>
        public GameObject debugPrefab
        {
            get => m_DebugPrefab;
            set => m_DebugPrefab = value;
        }

        [SerializeField]
        [Tooltip("A debug prefab that allows for these environment probes to be more readily visualized.")]
        GameObject m_DebugPrefab;

        /// <summary>
        /// Invoked once per frame with lists of environment probes that have been added, updated, and removed since the last frame.
        /// </summary>
        [Obsolete("environmentProbesChanged has been deprecated in AR Foundation version 6.0. Use trackablesChanged instead.", false)]
        public event Action<AREnvironmentProbesChangedEvent> environmentProbesChanged;

        /// <summary>
        /// Attempts to find the environment probe matching the trackable ID currently in the scene.
        /// </summary>
        /// <param name='trackableId'>The trackable ID of an environment probe to search for.</param>
        /// <returns>
        /// Environment probe in the scene matching the <paramref name="trackableId"/>, or <c>null</c> if no matching
        /// environment probe is found.
        /// </returns>
        public AREnvironmentProbe GetEnvironmentProbe(TrackableId trackableId)
        {
            if (m_Trackables.TryGetValue(trackableId, out var environmentProbe))
                return environmentProbe;

            return null;
        }

        internal bool TryAddEnvironmentProbe(AREnvironmentProbe probe)
        {
            if (!CanBeAddedToSubsystem(probe))
                return false;

            var reflectionProbe = probe.GetComponent<ReflectionProbe>();
            if (reflectionProbe == null)
                throw new InvalidOperationException($"Each {nameof(AREnvironmentProbe)} requires a {nameof(ReflectionProbe)} component.");

            if (!descriptor.supportsManualPlacement)
                throw new NotSupportedException("Manual environment probe placement is not supported by this subsystem.");

            var probeTransform = probe.transform;
            var trackablesParent = origin.TrackablesParent;
            var poseInSessionSpace = trackablesParent.InverseTransformPose(new Pose(probeTransform.position, probeTransform.rotation));

            var worldToLocalSession = trackablesParent.worldToLocalMatrix;
            var localToWorldProbe = probeTransform.localToWorldMatrix;

            // We want to calculate the "local-to-parent" of the probe if the XR origin were its parent.
            //     LTW_session * LTP_probe = LTW_probe
            // =>  LTP_probe = inverse(LTW_session) * LTW_probe
            var localToParentProbe = worldToLocalSession * localToWorldProbe;
            var sessionSpaceScale = localToParentProbe.lossyScale;

            if (subsystem.TryAddEnvironmentProbe(poseInSessionSpace, sessionSpaceScale, reflectionProbe.size, out var sessionRelativeData))
            {
                CreateTrackableFromExisting(probe, sessionRelativeData);
                probe.placementType = AREnvironmentProbePlacementType.Manual;
                return probe;
            }

            return false;
        }

        internal bool TryRemoveEnvironmentProbe(AREnvironmentProbe probe)
        {
            if (probe == null || subsystem == null)
                return false;

            var desc = descriptor;

            if (probe.placementType == AREnvironmentProbePlacementType.Manual && !desc.supportsRemovalOfManual)
                throw new InvalidOperationException("Removal of manually placed environment probes is not supported by this subsystem.");

            if (probe.placementType == AREnvironmentProbePlacementType.Automatic && !desc.supportsRemovalOfAutomatic)
                throw new InvalidOperationException("Removal of automatically placed environment probes is not supported by this subsystem.");

            if (subsystem.RemoveEnvironmentProbe(probe.trackableId))
            {
                if (m_PendingAdds.ContainsKey(probe.trackableId))
                {
                    m_PendingAdds.Remove(probe.trackableId);
                    m_Trackables.Remove(probe.trackableId);
                }

                probe.pending = false;
                return true;
            }

            return false;
        }

        /// <summary>
        /// The name of the `GameObject` for each instantiated <see cref="AREnvironmentProbe"/>.
        /// </summary>
        protected override string gameObjectName => nameof(AREnvironmentProbe);

        /// <summary>
        /// Gets the prefab that should be instantiated for each <see cref="AREnvironmentProbe"/>. May be `null`.
        /// </summary>
        /// <returns>The prefab that should be instantiated for each <see cref="AREnvironmentProbe"/>.</returns>
        protected override GameObject GetPrefab() => m_DebugPrefab;

        /// <summary>
        /// Enables the environment probe functionality by registering listeners for the environment probe events, if
        /// the <c>XREnvironmentProbeSubsystem</c> exists, and enabling environment probes in the AR subsystem manager.
        /// </summary>
        protected override void OnBeforeStart()
        {
            SetAutomaticPlacementStateOnSubsystem();
            SetEnvironmentTextureHDRStateOnSubsystem();
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            SetAutomaticPlacementStateOnSubsystem();
        }
#endif // UNITY_EDITOR

        /// <summary>
        /// Destroys any game objects created by this environment probe manager for each environment probe, and clears
        /// the mapping of environment probes.
        /// </summary>
        protected override void OnDestroy()
        {
            base.OnDestroy();
            foreach (var kvp in m_Trackables)
            {
                var environmentProbe = kvp.Value;
                if (environmentProbe != null)
                    Destroy(environmentProbe.gameObject);
            }
        }

        /// <summary>
        /// Invoked when the base class detects trackable changes.
        /// </summary>
        /// <param name="added">The list of added <see cref="AREnvironmentProbe"/>.</param>
        /// <param name="updated">The list of updated <see cref="AREnvironmentProbe"/>.</param>
        /// <param name="removed">The list of removed <see cref="AREnvironmentProbe"/>.</param>
        [Obsolete("OnTrackablesChanged() has been deprecated in AR Foundation version 6.0.", false)]
        protected override void OnTrackablesChanged(
            List<AREnvironmentProbe> added,
            List<AREnvironmentProbe> updated,
            List<AREnvironmentProbe> removed)
        {
            if (environmentProbesChanged != null)
            {
                using (new ScopedProfiler("OnEnvironmentProbesChanged"))
                {
                    environmentProbesChanged?.Invoke(new AREnvironmentProbesChangedEvent(added, updated, removed));
                }
            }
        }

        /// <summary>
        /// Invoked when an <see cref="AREnvironmentProbe"/> is created.
        /// </summary>
        /// <param name="probe">The <see cref="AREnvironmentProbe"/> that was just created.</param>
        protected override void OnCreateTrackable(AREnvironmentProbe probe)
        {
            probe.environmentTextureFilterMode = m_EnvironmentTextureFilterMode;
        }

        /// <summary>
        /// Sets the current state of the <see cref="automaticPlacementRequested"/> property to the
        /// <c>XREnvironmentProbeSubsystem</c>, if the subsystem exists and supports automatic placement.
        /// </summary>
        void SetAutomaticPlacementStateOnSubsystem()
        {
            if (enabled && supportsAutomaticPlacement)
            {
                subsystem.automaticPlacementRequested = m_AutomaticPlacement;
            }
        }

        /// <summary>
        /// Sets the current state of the <see cref="environmentTextureHDRRequested"/> property to the
        /// <c>XREnvironmentProbeSubsystem</c>, if the subsystem exists and supports HDR environment textures.
        /// </summary>
        void SetEnvironmentTextureHDRStateOnSubsystem()
        {
            if (enabled && supportsEnvironmentTextureHDR)
            {
                subsystem.environmentTextureHDRRequested = m_EnvironmentTextureHDR;
            }
        }
    }
}
