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
    }
}
