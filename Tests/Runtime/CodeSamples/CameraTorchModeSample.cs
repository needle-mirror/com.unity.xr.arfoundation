using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Tests
{
    public class CameraTorchModeSample : MonoBehaviour
    {
        #region CameraTorchModeSupport
        void ListSupportedConfigs()
        {
            var loader = LoaderUtility.GetActiveLoader();
            var sessionSubsystem =
                loader?.GetLoadedSubsystem<XRSessionSubsystem>();
            var configurations = sessionSubsystem.GetConfigurationDescriptors(
                Unity.Collections.Allocator.Temp);

            for (int i = 0; i < configurations.Length; i++)
            {
                var config = configurations[i];
                if ((config.capabilities & Feature.CameraTorch) != 0)
                {
                    // this configuration supports camera torch
                }
            }
        }
        #endregion

        #region EnableCameraTorch
        void EnableCameraTorch()
        {
            var loader = LoaderUtility.GetActiveLoader();
            var cameraSubsystem = loader?.GetLoadedSubsystem<XRCameraSubsystem>();
            if (cameraSubsystem.DoesCurrentCameraSupportTorch())
            {
                cameraSubsystem.requestedCameraTorchMode = XRCameraTorchMode.On;
            }
        }
        #endregion
    }
}
