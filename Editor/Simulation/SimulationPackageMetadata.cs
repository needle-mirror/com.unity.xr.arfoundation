using System.Collections.Generic;
using UnityEditor.XR.Management.Metadata;
using UnityEngine;
using UnityEngine.XR.Simulation;

namespace UnityEditor.XR.Simulation
{
    class XRPackage : IXRPackage
    {
        class SimulationLoaderMetadata : IXRLoaderMetadata
        {
            public string loaderName { get; set; }
            public string loaderType { get; set; }
            public List<BuildTargetGroup> supportedBuildTargets { get; set; }
        }

        class SimulationPackageMetadata : IXRPackageMetadata
        {
            public string packageName { get; set; }
            public string packageId { get; set; }
            public string settingsType { get; set; }
            public List<IXRLoaderMetadata> loaderMetadata { get; set; }
        }

        static IXRPackageMetadata s_Metadata = new SimulationPackageMetadata()
        {
            packageName = "AR Foundation",
            packageId = "com.unity.xr.arfoundation",
            settingsType = typeof(SimulationSettings).FullName,
            loaderMetadata = new List<IXRLoaderMetadata>()
            {
                new SimulationLoaderMetadata()
                {
                    loaderName = "XR Simulation (Experimental)",
                    loaderType = typeof(SimulationLoader).FullName,
                    supportedBuildTargets = new List<BuildTargetGroup>()
                    {
                        BuildTargetGroup.Standalone,
                    }
                },
            }
        };

        public IXRPackageMetadata metadata => s_Metadata;

        public bool PopulateNewSettingsInstance(ScriptableObject obj) { return true; }
    }
}
