namespace UnityEngine.XR.ARFoundation.Tests
{
    class ARTrackedImageManagerSamples
    {
        #region DescriptorChecks
        void CheckForOptionalFeatureSupport()
        {
            // This is inefficient. You should re-use a saved reference instead.
            var manager = Object.FindAnyObjectByType<ARTrackedImageManager>();

            // Use manager.descriptor to determine which optional features
            // are supported on the device. For example:

            if (manager.descriptor.supportsMovingImages)
            {
                // Moving images are supported.
            }
        }
        #endregion
    }
}
