#if VISUALSCRIPTING_1_8_OR_NEWER

using System;
using Unity.VisualScripting;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARFoundation.VisualScripting;
using UnityEngine.XR.ARSubsystems;

namespace UnityEditor.XR.ARFoundation.VisualScripting
{
    /// <summary>
    /// Unit descriptor for the <see cref="EnvironmentProbesChangedEventUnit"/>.
    /// </summary>
    /// <seealso href="https://docs.unity3d.com/Packages/com.unity.visualscripting@1.8/manual/vs-create-custom-node-add-docs.html"/>
    [Descriptor(typeof(EnvironmentProbesChangedEventUnit))]
    [Obsolete("EnvironmentProbesChangedEventUnitDescriptor has been deprecated in AR Foundation version 6.0.", false)]
    public sealed class EnvironmentProbesChangedEventUnitDescriptor : TrackablesChangedEventUnitDescriptor<
        AREnvironmentProbeManager,
        XREnvironmentProbeSubsystem,
        XREnvironmentProbeSubsystemDescriptor,
        XREnvironmentProbeSubsystem.Provider,
        XREnvironmentProbe,
        AREnvironmentProbe,
        AREnvironmentProbesChangedEvent,
        AREnvironmentProbeManagerListener>
    {
        /// <inheritdoc/>
        public EnvironmentProbesChangedEventUnitDescriptor(EnvironmentProbesChangedEventUnit target) : base(target)
        {
        }
    }
}

#endif // VISUALSCRIPTING_1_8_OR_NEWER
