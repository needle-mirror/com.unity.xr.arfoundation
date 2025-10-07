using System;
using Unity.Collections;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Tests
{
    class ARMarkerManagerSamples
    {
        #region GetSupportedMarkerTypes
        void GetSupportedMarkerTypes(ARMarkerManager manager)
        {
            if (manager.subsystem is XRMarkerSubsystem markerSubsystem)
            {
                var result = markerSubsystem.subsystemDescriptor.supportedMarkerTypes;
                if (result.status.IsError())
                {
                    // Handle error
                    return;
                }

                var supportedMarkerTypes = result.value;
            }
        }
        #endregion

        #region GetMarkerDataFromMarker
        void GetMarkerData(ARMarker marker)
        {
            if (marker.markerType == XRMarkerType.QRCode ||
                marker.markerType == XRMarkerType.MicroQRCode)
            {
                switch (marker.dataBuffer.bufferType)
                {
                    case XRSpatialBufferType.String:
                        GetStringData(marker);
                        break;
                    case XRSpatialBufferType.Uint8:
                        GetBytesData(marker);
                        break;
                }
            }
        }

        void GetStringData(ARMarker marker)
        {
            var result = marker.TryGetStringData();
            if (result.status.IsError())
                return;

            string stringData = result.value;
        }

        void GetBytesData(ARMarker marker)
        {
            var result = marker.TryGetBytesData();
            if (result.status.IsError())
                return;

            byte[] bytesData = result.value;
        }
        #endregion

        #region SubscribeToMarkerChanges
        void SubscribeToMarkerChanges(ARMarkerManager manager)
        {
            manager.trackablesChanged.AddListener(OnMarkersChanged);
        }

        void OnMarkersChanged(ARTrackablesChangedEventArgs<ARMarker> changes)
        {
            foreach (var added in changes.added)
            {
                // Handle added markers, initial pose and marker ID
                // or encoded data
            }

            foreach (var updated in changes.updated)
            {
                // Handle updated markers, typically changes in pose
            }

            foreach (var removed in changes.removed)
            {
                // Handle removed markers
            }
        }

        #endregion

        #region GetMarkerDataFromManager
        void GetMarkerDataFromManager(ARMarkerManager manager, ARMarker marker)
        {
            if (marker.markerType == XRMarkerType.QRCode ||
                marker.markerType == XRMarkerType.MicroQRCode)
            {
                switch (marker.dataBuffer.bufferType)
                {
                    case XRSpatialBufferType.String:
                        GetStringDataFromManager(manager, marker);
                        break;
                    case XRSpatialBufferType.Uint8:
                        GetBytesDataFromManager(manager, marker);
                        break;
                }
            }
        }

        void GetStringDataFromManager(ARMarkerManager manager, ARMarker marker)
        {
            var result = manager.TryGetStringData(marker);
            if (result.status.IsError())
                return;

            string stringData = result.value;
        }

        void GetBytesDataFromManager(ARMarkerManager manager, ARMarker marker)
        {
            var result = manager.TryGetBytesData(marker);
            if (result.status.IsError())
                return;

            byte[] bytesData = result.value;
        }
        #endregion

        #region GetNativeArrayBytesData
        void GetNativeArrayBytesData(ARMarkerManager manager, ARMarker marker)
        {
            if (manager.subsystem is XRMarkerSubsystem markerSubsystem)
            {
                var result = markerSubsystem.TryGetBytesData(
                    marker.dataBuffer, Allocator.Temp);

                if (result.status.IsError())
                {
                    // Handle error
                    return;
                }

                NativeArray<byte> bytesData = result.value;
            }
        }
        #endregion
    }
}
