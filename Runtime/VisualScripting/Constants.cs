using System;
using System.Collections.Generic;

namespace UnityEngine.XR.ARFoundation.VisualScripting
{
    static class Constants
    {
        internal const string k_ARFoundation_Unit_Category = "AR Foundation";
        internal const string k_Event_Unit_Category = "Events/AR Foundation";

        internal enum ARFoundationFeatureOrder
        {
            Session,
            DeviceTracking,
            Camera,
            PlaneDetection,
            ImageTracking,
            ObjectTracking,
            FaceTracking,
            BodyTracking,
            PointClouds,
            Raycasts,
            Anchors,
            Meshing,
            EnvironmentProbes,
            Occlusion,
            Participants,
        }

        // These Event Hook Names can be deleted once the deprecated units that use them are deleted
        internal static class DeprecatedEventHookNames
        {
            public const string planesChanged = nameof(planesChanged);
            public const string trackedImagesChanged = nameof(trackedImagesChanged);
            public const string facesChanged = nameof(facesChanged);
            public const string humanBodiesChanged = nameof(humanBodiesChanged);
            public const string trackedObjectsChanged = nameof(trackedObjectsChanged);
            public const string anchorsChanged = nameof(anchorsChanged);
            public const string pointCloudsChanged = nameof(pointCloudsChanged);
            public const string environmentProbesChanged = nameof(environmentProbesChanged);
            public const string participantsChanged = nameof(participantsChanged);
        }

        internal static readonly Dictionary<Type, string> EventHookNames = new()
        {
            { typeof(ARTrackablesChangedEventArgs<ARPlane>), "ARPlanesChanged" },
            { typeof(ARTrackablesChangedEventArgs<ARTrackedImage>), "ARTrackedImagesChanged" },
            { typeof(ARTrackablesChangedEventArgs<ARTrackedObject>), "ARTrackedObjectsChanged" },
            { typeof(ARTrackablesChangedEventArgs<ARFace>), "ARFacesChanged" },
            { typeof(ARTrackablesChangedEventArgs<ARHumanBody>), "ARHumanBodiesChanged" },
            { typeof(ARTrackablesChangedEventArgs<ARPointCloud>), "ARPointCloudsChanged" },
            { typeof(ARTrackablesChangedEventArgs<ARRaycast>), "ARRaycastsChanged" },
            { typeof(ARTrackablesChangedEventArgs<ARAnchor>), "ARAnchorsChanged" },
            { typeof(ARTrackablesChangedEventArgs<AREnvironmentProbe>), "AREnvironmentProbesChanged" },
            { typeof(ARTrackablesChangedEventArgs<ARParticipant>), "ARParticipantsChanged" },
            { typeof(ARSessionStateChangedEventArgs), "ARSessionStateChanged" },
            { typeof(ARFaceUpdatedEventArgs), "ARFaceUpdated" },
            { typeof(ARCameraFrameEventArgs), "ARCameraFrameReceived" },
        };
    }
}
