namespace UnityEngine.XR.ARFoundation.Tests
{
    class ARTrackableManagerSamples
    {
        #region PlanesChanged
        public void OnTrackablesChanged(ARTrackablesChangedEventArgs<ARPlane> changes)
        {
            foreach (var plane in changes.added)
            {
                // handle added planes
            }

            foreach (var plane in changes.updated)
            {
                // handle updated planes
            }

            foreach (var plane in changes.removed)
            {
                // handle removed planes
            }
        }
        #endregion

        #region PlanesSubscribe
        void SubscribeToPlanesChanged()
        {
            // This is inefficient. You should re-use a saved reference instead.
            var manager = Object.FindAnyObjectByType<ARPlaneManager>();

            manager.trackablesChanged.AddListener(OnTrackablesChanged);
        }
        #endregion

        #region BoundingBoxesChanged
        public void OnTrackablesChanged(ARTrackablesChangedEventArgs<ARBoundingBox> changes)
        {
            foreach (var boundingBox in changes.added)
            {
                // handle added bounding boxes
            }

            foreach (var boundingBox in changes.updated)
            {
                // handle updated bounding boxes
            }

            foreach (var boundingBox in changes.removed)
            {
                // handle removed bounding boxes
            }
        }
        #endregion

        #region BoundingBoxSubscribe
        void SubscribeToBoundingBoxesChanged()
        {
            // This is inefficient. You should re-use a saved reference instead.
            var manager = Object.FindAnyObjectByType<ARBoundingBoxManager>();

            manager.trackablesChanged.AddListener(OnTrackablesChanged);
        }
        #endregion

        #region FacesChanged
        public void OnTrackablesChanged(ARTrackablesChangedEventArgs<ARFace> changes)
        {
            foreach (var face in changes.added)
            {
                // handle added faces
            }

            foreach (var face in changes.updated)
            {
                // handle updated faces
            }

            foreach (var face in changes.removed)
            {
                // handle removed faces
            }
        }
        #endregion

        #region FaceSubscribe
        void SubscribeToFacesChanged()
        {
            // This is inefficient. You should re-use a saved reference instead.
            var manager = Object.FindAnyObjectByType<ARFaceManager>();

            manager.trackablesChanged.AddListener(OnTrackablesChanged);
        }
        #endregion

        #region AnchorsChanged
        public void OnTrackablesChanged(
            ARTrackablesChangedEventArgs<ARAnchor> changes)
        {
            foreach (var anchor in changes.added)
            {
                // handle added anchors
            }

            foreach (var anchor in changes.updated)
            {
                // handle updated anchors
            }

            foreach (var anchor in changes.removed)
            {
                // handle removed anchors
            }
        }
        #endregion

        #region AnchorsSubscribe
        void SubscribeToAnchorsChanged()
        {
            // This is inefficient. You should re-use a saved reference instead.
            var manager = Object.FindAnyObjectByType<ARAnchorManager>();

            manager.trackablesChanged.AddListener(OnTrackablesChanged);
        }
        #endregion

        #region RaycastsChanged
        public void OnTrackablesChanged(
            ARTrackablesChangedEventArgs<ARRaycast> changes)
        {
            foreach (var raycast in changes.added)
            {
                // handle added raycasts
            }

            foreach (var raycast in changes.updated)
            {
                // handle updated raycasts
            }

            foreach (var raycast in changes.removed)
            {
                // handle removed raycasts
            }
        }
        #endregion

        #region RaycastsSubscribe
        void SubscribeToRaycastsChanged()
        {
            // This is inefficient. You should re-use a saved reference instead.
            var manager = Object.FindAnyObjectByType<ARRaycastManager>();

            manager.trackablesChanged.AddListener(OnTrackablesChanged);
        }
        #endregion
    }
}
