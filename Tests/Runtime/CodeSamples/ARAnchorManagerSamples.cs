using System;
using System.Collections.Generic;
using System.Threading;
using Unity.Collections;
using Unity.XR.CoreUtils.Collections;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Tests
{
    class ARAnchorManagerSamples
    {
        #region DescriptorChecks
        void CheckForOptionalFeatureSupport(ARAnchorManager manager)
        {
            // Use manager.descriptor to determine which optional features
            // are supported on the device. For example:

            if (manager.descriptor.supportsTrackableAttachments)
            {
                // Trackable attachments are supported.
            }
        }
        #endregion

        #region TryAddAnchorAsync
        async void CreateAnchorAsync(ARAnchorManager manager)
        {
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
        async void SaveAnchorAsync(ARAnchorManager manager, ARAnchor anchor)
        {
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

        #region TrySaveAnchorsAsync
        async void SaveAnchorsAsync(
            ARAnchorManager manager,
            IEnumerable<ARAnchor> anchors)
        {
            var results = new List<ARSaveOrLoadAnchorResult>();
            await manager.TrySaveAnchorsAsync(anchors, results);

            foreach (var saveAnchorResult in results)
            {
                if (saveAnchorResult.resultStatus.IsSuccess())
                {
                    // Save the `savedAnchorGuid`s stored in `saveAnchorResults`,
                    // then use them as input for TryLoadAnchorsAsync or
                    // `TryEraseAnchorsAsync`.
                }
                else
                {
                    // Anchor failed to save. Handle error.
                }
            }
        }
        #endregion

        #region TryLoadAnchorAsync
        async void LoadAnchorAsync(ARAnchorManager manager, SerializableGuid guid)
        {
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

        #region TryLoadAnchorsAsync
        async void LoadAnchorsAsync(
            ARAnchorManager manager,
            IEnumerable<SerializableGuid> savedAnchorGuids)
        {
            var results = new List<ARSaveOrLoadAnchorResult>();
            await manager.TryLoadAnchorsAsync(
                savedAnchorGuids,
                results,
                OnIncrementalResultsAvailable);

            foreach (var loadAnchorResult in results)
            {
                if (loadAnchorResult.resultStatus.IsSuccess())
                {
                    // Anchor with results.savedAnchorGuid was successfully loaded.
                }
                else
                {
                    // Anchor with results.savedAnchorGuid failed to load.
                }
            }
        }

        void OnIncrementalResultsAvailable(ReadOnlyListSpan<ARSaveOrLoadAnchorResult> loadAnchorResults)
        {
            foreach (var loadAnchorResult in loadAnchorResults)
            {
                // You can use these anchors immediately without waiting for the
                // entire batch to finish loading.
                // loadAnchorResult.resultStatus.IsSuccess() will always be true
                // for anchors passed to the incremental results callback.
                ARAnchor loadedAnchor = loadAnchorResult.anchor;
            }
        }
        #endregion

        #region TryEraseAnchorAsync
        async void EraseAnchorAsync(ARAnchorManager manager, SerializableGuid guid)
        {
            var status = await manager.TryEraseAnchorAsync(guid);
            if (status.IsError())
            {
                // handle error
                return;
            }

            // The anchor was successfully erased.
        }
        #endregion

        #region TryEraseAnchorsAsync
        async void EraseAnchorsAsync(
            ARAnchorManager manager,
            IEnumerable<SerializableGuid> savedAnchorGuids)
        {
            var eraseAnchorResults = new List<XREraseAnchorResult>();
            await manager.TryEraseAnchorsAsync(savedAnchorGuids, eraseAnchorResults);
            foreach (var eraseAnchorResult in eraseAnchorResults)
            {
                if (eraseAnchorResult.resultStatus.IsSuccess())
                {
                    // anchor was successfully erased.
                }
                else
                {
                    // anchor failed to erase
                }
            }
        }
        #endregion

        #region TryGetSavedAnchorIdsAsync
        async void GetSavedAnchorIdsAsync(ARAnchorManager manager)
        {
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
        void AsyncCancellation(ARAnchorManager manager)
        {
            // Create a CancellationTokenSource to serve our CancellationToken
            var cts = new CancellationTokenSource();

            // Use one of the other methods in the persistent anchor API
            var awaitable = manager.TryGetSavedAnchorIdsAsync(Allocator.Temp, cts.Token);

            // Cancel the async operation before it completes
            cts.Cancel();
        }
        #endregion
    }
}
