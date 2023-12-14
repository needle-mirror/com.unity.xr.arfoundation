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
    }
}
