using System;
using System.Collections.Generic;

namespace UnityEngine.XR.ARFoundation
{
    class TrackableEntry
    {
        internal ARTrackable trackable { get; set; }
        internal List<TrackableKey> childKeys { get; }
        internal TrackableKey? parentKey { get; set; }

        internal Type trackableType => trackable.GetType();
        internal TrackableKey key => new(trackable.trackableId, trackableType);

        internal TrackableEntry(ARTrackable trackable)
        {
            this.trackable = trackable;
            childKeys = new();
            parentKey = null;
        }

        internal void Reset()
        {
            trackable = null;
            childKeys.Clear();
            parentKey = null;
        }
    }
}
