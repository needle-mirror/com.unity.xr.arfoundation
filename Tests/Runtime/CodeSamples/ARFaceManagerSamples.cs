namespace UnityEngine.XR.ARFoundation.Tests
{
    class ARFaceManagerSamples
    {
        #region DescriptorChecks
        void CheckForOptionalFeatureSupport()
        {
            // This is inefficient. You should re-use a saved reference instead.
            var manager = Object.FindAnyObjectByType<ARFaceManager>();

            // Use manager.descriptor to determine which optional features
            // are supported on the device. For example:

            if (manager.descriptor.supportsFacePose)
            {
                // Face pose is supported.
            }
        }
        #endregion
    }
}
