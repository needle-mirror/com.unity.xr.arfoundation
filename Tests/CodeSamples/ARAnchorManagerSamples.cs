using System.Threading;
using Unity.Collections;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Tests
{
    class ARAnchorManagerSamples
    {
        #region DescriptorChecks
        void CheckForOptionalFeatureSupport()
        {
            // This is inefficient. You should re-use a saved reference instead.
            var manager = Object.FindAnyObjectByType<ARAnchorManager>();

            // Use manager.descriptor to determine which optional features
            // are supported on the device. For example:

            if (manager.descriptor.supportsTrackableAttachments)
            {
                // Trackable attachments are supported.
            }
        }
        #endregion

        #region TryAddAnchorAsync
        async void CreateAnchorAsync()
        {
            // This is inefficient. You should re-use a saved reference instead.
            var manager = Object.FindAnyObjectByType<ARAnchorManager>();

            // This is a "dummy" pose value. You should use a pose that is meaningful
            // to your app, such as from a raycast hit or another trackable.
            var pose = new Pose(Vector3.zero, Quaternion.identity);

            var result = await manager.TryAddAnchorAsync(pose);
            if (result.status.IsSuccess())
            {
                var anchor = result.value;
                // Do something with the newly created anchor.
            }
        }
        #endregion

        #region AttachAnchor
        void AttachAnchor(ARAnchorManager manager, ARPlane plane, Pose pose)
        {
            if (manager.descriptor.supportsTrackableAttachments)
            {
                var anchor = manager.AttachAnchor(plane, pose);
                // Do something with the newly created anchor.
            }
        }
        #endregion

        #region TrySaveAnchorAsync
        async void TrySaveAnchorAsync(ARAnchor anchor)
        {
            // This is inefficient. You should re-use a saved reference instead.
            var manager = Object.FindAnyObjectByType<ARAnchorManager>();

            var result = await manager.TrySaveAnchorAsync(anchor);
            if (result.status.IsError())
            {
                // handle error
                return;
            }

            // Save this value, then use it as an input parameter
            // to TryLoadAnchorAsync or TryEraseAnchorAsync
            SerializableGuid guid = result.value;
        }
        #endregion

        #region TryLoadAnchorAsync
        async void TryLoadAnchorAsync(SerializableGuid guid)
        {
            // This is inefficient. You should re-use a saved reference instead.
            var manager = Object.FindAnyObjectByType<ARAnchorManager>();

            var result = await manager.TryLoadAnchorAsync(guid);
            if (result.status.IsError())
            {
                // handle error
                return;
            }

            // You can use this anchor as soon as it's returned to you.
            ARAnchor anchor = result.value;
        }
        #endregion

        #region TryEraseAnchorAsync
        async void TryEraseAnchorAsync(SerializableGuid guid)
        {
            // This is inefficient. You should re-use a saved reference instead.
            var manager = Object.FindAnyObjectByType<ARAnchorManager>();

            var status = await manager.TryEraseAnchorAsync(guid);
            if (status.IsError())
            {
                // handle error
                return;
            }

            // The anchor was successfully erased.
        }
        #endregion

        #region TryGetSavedAnchorIdsAsync
        async void TryGetSavedAnchorIdsAsync()
        {
            // This is inefficient. You should re-use a saved reference instead.
            var manager = Object.FindAnyObjectByType<ARAnchorManager>();

            // If you need to keep the saved anchor IDs longer than a frame, use
            // Allocator.Persistent instead, then remember to Dispose the array.
            var result = await manager.TryGetSavedAnchorIdsAsync(Allocator.Temp);

            if (result.status.IsError())
            {
                // handle error
                return;
            }

            // Do something with the saved anchor IDs
            NativeArray<SerializableGuid> anchorIds = result.value;
        }
        #endregion

        #region AsyncCancellation
        void AsyncCancellation()
        {
            // This is inefficient. You should re-use a saved reference instead.
            var manager = Object.FindAnyObjectByType<ARAnchorManager>();

            // Create a CancellationTokenSource to serve our CancellationToken
            var cts = new CancellationTokenSource();

            // Use one of the other methods in the persistent anchor API
            var task = manager.TryGetSavedAnchorIdsAsync(Allocator.Temp, cts.Token);

            // Cancel the async operation before it completes
            cts.Cancel();
        }
        #endregion
    }
}
