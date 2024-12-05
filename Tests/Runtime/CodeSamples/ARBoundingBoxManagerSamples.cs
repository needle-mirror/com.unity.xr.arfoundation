namespace UnityEngine.XR.ARFoundation.Tests
{
    class ARBoundingBoxManagerSamples
    {
        #region DescriptorChecks
        void CheckForOptionalFeatureSupport()
        {
            // This is inefficient. You should re-use a saved reference instead.
            var manager = Object.FindAnyObjectByType<ARBoundingBoxManager>();

            // Use manager.descriptor to determine which optional features
            // are supported on the device. For example:
            if (manager.descriptor.supportsClassifications)
            {
                // Classification is supported.
            }
        }
        #endregion
    }
}
