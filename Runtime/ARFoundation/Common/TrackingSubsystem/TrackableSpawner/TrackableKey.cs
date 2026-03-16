using System;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation
{
    readonly struct TrackableKey : IEquatable<TrackableKey>
    {
        internal TrackableId trackableId { get; }
        internal Type trackableType { get; }

        internal TrackableKey(TrackableId id, Type type)
        {
            trackableType = type;
            trackableId = id;
        }

        public void Deconstruct(out TrackableId id, out Type type)
        {
            id = trackableId;
            type = trackableType;
        }

        public bool Equals(TrackableKey other)
        {
            return trackableType == other.trackableType && trackableId.Equals(other.trackableId);
        }

        public override bool Equals(object obj)
        {
            return obj is TrackableKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(trackableType, trackableId);
        }

        public override string ToString()
        {
            return $"{trackableType}: {trackableId.ToString()}";
        }
    }
}
