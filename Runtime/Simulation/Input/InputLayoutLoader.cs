using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.ARSubsystems;

using Inputs = UnityEngine.InputSystem.InputSystem;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEngine.XR.Simulation
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    class InputLayoutLoader
    {
#if UNITY_EDITOR
        static InputLayoutLoader()
        {
#if ENABLE_INPUT_SYSTEM && (ENABLE_VR || UNITY_GAMECORE)
            RegisterLayouts();
#endif // ENABLE_INPUT_SYSTEM && (ENABLE_VR || UNITY_GAMECORE)
        }
#endif //UNITY_EDITOR

#if ENABLE_INPUT_SYSTEM && (ENABLE_VR || UNITY_GAMECORE)
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void RegisterLayouts()
        {
            if (!Api.supported || !Api.loaderPresent)
                return;

            Inputs.RegisterLayout<HandheldARInputDevice>(
                matches: new InputDeviceMatcher()
                    .WithInterface(XRUtilities.InterfaceMatchAnyVersion)
                    .WithProduct("(XR Simulation)")
                );
        }
#endif // ENABLE_INPUT_SYSTEM && (ENABLE_VR || UNITY_GAMECORE)
    }
}
