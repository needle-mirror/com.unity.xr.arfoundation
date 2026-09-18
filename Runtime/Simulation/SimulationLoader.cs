using System.Collections.Generic;
#if UNITY_6000_5_OR_NEWER
using Unity.Scripting.LifecycleManagement;
#endif
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.Management;

namespace UnityEngine.XR.Simulation
{
    /// <summary>
    /// Manages the lifecycle of Simulation subsystems.
    /// </summary>
    public class SimulationLoader : XRLoaderHelper
    {
#if UNITY_6000_5_OR_NEWER
        [NoAutoStaticsCleanup]
#endif
        static List<XRSessionSubsystemDescriptor> s_SessionDescriptors = new();

#if UNITY_6000_5_OR_NEWER
        [NoAutoStaticsCleanup]
#endif
        static List<XRCameraSubsystemDescriptor> s_CameraDescriptors = new();

#if UNITY_6000_5_OR_NEWER
        [NoAutoStaticsCleanup]
#endif
        static List<XRInputSubsystemDescriptor> s_InputDescriptors = new();

#if UNITY_6000_5_OR_NEWER
        [NoAutoStaticsCleanup]
#endif
        static List<XRPlaneSubsystemDescriptor> s_PlaneDescriptors = new();

#if UNITY_6000_5_OR_NEWER
        [NoAutoStaticsCleanup]
#endif
        static List<XRPointCloudSubsystemDescriptor> s_PointCloudDescriptors = new();

#if UNITY_6000_5_OR_NEWER
        [NoAutoStaticsCleanup]
#endif
        static List<XRImageTrackingSubsystemDescriptor> s_ImageTrackingDescriptors = new();

#if UNITY_6000_5_OR_NEWER
        [NoAutoStaticsCleanup]
#endif
        static List<XRRaycastSubsystemDescriptor> s_RaycastDescriptors = new();

#if UNITY_6000_5_OR_NEWER
        [NoAutoStaticsCleanup]
#endif
        static List<XRMeshSubsystemDescriptor> s_MeshDescriptors  = new();

#if UNITY_6000_5_OR_NEWER
        [NoAutoStaticsCleanup]
#endif
        static List<XREnvironmentProbeSubsystemDescriptor> s_ProbeDescriptors = new();

#if UNITY_6000_5_OR_NEWER
        [NoAutoStaticsCleanup]
#endif
        static List<XRAnchorSubsystemDescriptor> s_AnchorDescriptors = new();

#if UNITY_6000_5_OR_NEWER
        [NoAutoStaticsCleanup]
#endif
        static List<XROcclusionSubsystemDescriptor> s_OcclusionDescriptors = new();

#if UNITY_6000_5_OR_NEWER
        [NoAutoStaticsCleanup]
#endif
        static List<XRBoundingBoxSubsystemDescriptor> s_BoundingBoxDescriptors = new();

        /// <summary>
        /// Initializes the loader.
        /// </summary>
        /// <returns>`true` if the session subsystem was successfully created. Otherwise, `false`.</returns>
        public override bool Initialize()
        {
            CreateSubsystem<XRSessionSubsystemDescriptor, XRSessionSubsystem>(s_SessionDescriptors, SimulationSessionSubsystem.k_SubsystemId);
            CreateSubsystem<XRCameraSubsystemDescriptor, XRCameraSubsystem>(s_CameraDescriptors, SimulationCameraSubsystem.k_SubsystemId);
            CreateSubsystem<XRInputSubsystemDescriptor, XRInputSubsystem>(s_InputDescriptors, "XRSimulation-Input");
            CreateSubsystem<XRPlaneSubsystemDescriptor, XRPlaneSubsystem>(s_PlaneDescriptors, SimulationPlaneSubsystem.k_SubsystemId);
            CreateSubsystem<XRPointCloudSubsystemDescriptor, XRPointCloudSubsystem>(s_PointCloudDescriptors, SimulationPointCloudSubsystem.k_SubsystemId);
            CreateSubsystem<XRImageTrackingSubsystemDescriptor, XRImageTrackingSubsystem>(s_ImageTrackingDescriptors, SimulationImageTrackingSubsystem.k_SubsystemId);
            CreateSubsystem<XRRaycastSubsystemDescriptor, XRRaycastSubsystem>(s_RaycastDescriptors, SimulationRaycastSubsystem.k_SubsystemId);
            CreateSubsystem<XRMeshSubsystemDescriptor, XRMeshSubsystem>(s_MeshDescriptors, SimulationMeshSubsystem.k_SubsystemId);
            CreateSubsystem<XREnvironmentProbeSubsystemDescriptor, XREnvironmentProbeSubsystem>(s_ProbeDescriptors, SimulationEnvironmentProbeSubsystem.k_SubsystemId);
            CreateSubsystem<XRAnchorSubsystemDescriptor, XRAnchorSubsystem>(s_AnchorDescriptors, SimulationAnchorSubsystem.k_SubsystemId);
            CreateSubsystem<XROcclusionSubsystemDescriptor, XROcclusionSubsystem>(s_OcclusionDescriptors, SimulationOcclusionSubsystem.k_SubsystemId);
            CreateSubsystem<XRBoundingBoxSubsystemDescriptor, XRBoundingBoxSubsystem>(s_BoundingBoxDescriptors, SimulationBoundingBoxSubsystem.k_SubsystemId);

            var sessionSubsystem = GetLoadedSubsystem<XRSessionSubsystem>();
            if (sessionSubsystem == null)
                Debug.LogError("Failed to load session subsystem.");

            return sessionSubsystem != null;
        }

        /// <summary>
        /// Destroys each subsystem.
        /// </summary>
        /// <returns>Always returns `true`.</returns>
        public override bool Deinitialize()
        {
            DestroySubsystem<XRBoundingBoxSubsystem>();
            DestroySubsystem<XROcclusionSubsystem>();
            DestroySubsystem<XRAnchorSubsystem>();
            DestroySubsystem<XREnvironmentProbeSubsystem>();
            DestroySubsystem<XRMeshSubsystem>();
            DestroySubsystem<XRRaycastSubsystem>();
            DestroySubsystem<XRImageTrackingSubsystem>();
            DestroySubsystem<XRPointCloudSubsystem>();
            DestroySubsystem<XRPlaneSubsystem>();
            DestroySubsystem<XRInputSubsystem>();
            DestroySubsystem<XRCameraSubsystem>();
            DestroySubsystem<XRSessionSubsystem>();

            return true;
        }
    }
}
