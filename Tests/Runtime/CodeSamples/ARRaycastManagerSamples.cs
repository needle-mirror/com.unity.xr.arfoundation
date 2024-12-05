namespace UnityEngine.XR.ARFoundation.Tests
{
    class ARRaycastManagerSamples
    {
        #region DescriptorChecks
        void CheckForOptionalFeatureSupport()
        {
            // This is inefficient. You should re-use a saved reference instead.
            var manager = Object.FindAnyObjectByType<ARRaycastManager>();

            // Use manager.descriptor to determine which optional features
            // are supported on the device. For example:

            if (manager.descriptor.supportsWorldBasedRaycast)
            {
                // World based ray casts are supported.
            }
        }
        #endregion
    }
}
