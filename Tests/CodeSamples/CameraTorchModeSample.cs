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
                loader != null ? loader.GetLoadedSubsystem<XRSessionSubsystem>() : null;
            var configurations =
                sessionSubsystem.GetConfigurationDescriptors(Unity.Collections.Allocator.Temp);
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
            var cameraSubsystem = loader != null ? loader.GetLoadedSubsystem<XRCameraSubsystem>() : null;
            if (cameraSubsystem.DoesCurrentCameraSupportTorch())
            {
                cameraSubsystem.requestedCameraTorchMode = XRCameraTorchMode.On;
            }
        }
        #endregion
    }
}
