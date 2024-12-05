namespace UnityEngine.XR.ARFoundation.Tests
{
    class ARPlaneManagerSamples
    {
        #region DescriptorChecks
        void CheckForOptionalFeatureSupport()
        {
            // This is inefficient. You should re-use a saved reference instead.
            var manager = Object.FindAnyObjectByType<ARPlaneManager>();

            // Use manager.descriptor to determine which optional features
            // are supported on the device. For example:

            if (manager.descriptor.supportsClassification)
            {
                // Classification is supported.
            }
        }
        #endregion
    }
}
