using System;
using System.Collections.Generic;
using Unity.XR.CoreUtils.Collections;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation
{
    /// <summary>
    /// Event arguments for the `ARTrackableManager.trackablesChanged` event.
    /// </summary>
    /// <typeparam name="TTrackable">The trackable type.</typeparam>
    public readonly struct ARTrackablesChangedEventArgs<TTrackable> : IEquatable<ARTrackablesChangedEventArgs<TTrackable>>
        where TTrackable : ARTrackable
    {
        /// <summary>
        /// The collection of trackables that have been added.
        /// </summary>
        public ReadOnlyList<TTrackable> added { get; }

        /// <summary>
        /// The collection of trackables that have been updated.
        /// </summary>
        public ReadOnlyList<TTrackable> updated { get; }

        /// <summary>
        /// The collection of trackables that have been removed.
        /// </summary>
        public ReadOnlyList<KeyValuePair<TrackableId, TTrackable>> removed { get; }

        /// <summary>
        /// Constructs an <see cref="ARTrackablesChangedEventArgs&lt;TTrackable&gt;"/>.
        /// </summary>
        /// <param name="added">The collection of trackables that have been added.</param>
        /// <param name="updated">The collection of trackables that have been updated.</param>
        /// <param name="removed">The collection of trackables that have been removed.</param>
        public ARTrackablesChangedEventArgs(
            ReadOnlyList<TTrackable> added,
            ReadOnlyList<TTrackable> updated,
            ReadOnlyList<KeyValuePair<TrackableId, TTrackable>> removed)
        {
            this.added = added ?? new ReadOnlyList<TTrackable>(new List<TTrackable>(0));
            this.updated = updated ?? new ReadOnlyList<TTrackable>(new List<TTrackable>(0));
            this.removed = removed ?? new ReadOnlyList<KeyValuePair<TrackableId, TTrackable>>(
                new List<KeyValuePair<TrackableId, TTrackable>>(0));
        }

        /// <summary>
        /// Generates a hash suitable for use with containers like `HashSet` and `Dictionary`.
        /// </summary>
        /// <returns>A hash code generated from this object's fields.</returns>
        public override int GetHashCode() => HashCodeUtil.Combine(
            HashCodeUtil.ReferenceHash(added),
            HashCodeUtil.ReferenceHash(updated),
            HashCodeUtil.ReferenceHash(removed));

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <param name="obj">The `object` to compare against.</param>
        /// <returns>Returns <see langword="true"/> if <paramref name="obj"/> is of type
        /// <see cref="ARTrackablesChangedEventArgs&lt;TTrackable&gt;"/> and
        /// <see cref="Equals(ARTrackablesChangedEventArgs&lt;TTrackable&gt;)"/> also returns <see langword="true"/>.
        /// Otherwise, returns <see langword="false"/>.</returns>
        public override bool Equals(object obj)
        {
            return obj is ARTrackablesChangedEventArgs<TTrackable> args && Equals(args);
        }

        /// <summary>
        /// Generates a string representation of this <see cref="ARTrackablesChangedEventArgs&lt;TTrackable&gt;"/>.
        /// </summary>
        /// <returns>A string representation of this <see cref="ARTrackablesChangedEventArgs&lt;TTrackable&gt;"/>.</returns>
        public override string ToString()
        {
            return $"Added: {added?.Count ?? 0}, Updated: {updated?.Count ?? 0}, Removed: {removed?.Count ?? 0}";
        }

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <param name="other">The other <see cref="ARTrackablesChangedEventArgs&lt;TTrackable&gt;"/> to compare against.</param>
        /// <returns>Returns <see langword="true"/> if every field in <paramref name="other"/> is equal to this
        /// <see cref="ARTrackablesChangedEventArgs&lt;TTrackable&gt;"/>, otherwise returns <see langword="false"/>.</returns>
        public bool Equals(ARTrackablesChangedEventArgs<TTrackable> other)
        {
            return
                added == other.added &&
                updated == other.updated &&
                removed == other.removed;
        }

        /// <summary>
        /// Tests for equality. Same as <see cref="Equals(ARTrackablesChangedEventArgs&lt;TTrackable&gt;)"/>.
        /// </summary>
        /// <param name="lhs">The left-hand side of the comparison.</param>
        /// <param name="rhs">The right-hand side of the comparison.</param>
        /// <returns>Returns <see langword="true"/> if <paramref name="lhs"/> is equal to <paramref name="rhs"/>.
        /// Otherwise, returns <see langword="false"/>.</returns>
        public static bool operator ==(ARTrackablesChangedEventArgs<TTrackable> lhs, ARTrackablesChangedEventArgs<TTrackable> rhs)
        {
            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Tests for inequality. Same as `!`<see cref="Equals(ARTrackablesChangedEventArgs&lt;TTrackable&gt;)"/>.
        /// </summary>
        /// <param name="lhs">The left-hand side of the comparison.</param>
        /// <param name="rhs">The right-hand side of the comparison.</param>
        /// <returns>Returns <see langword="true"/> if <paramref name="lhs"/> is not equal to <paramref name="rhs"/>.
        /// Otherwise, returns <see langword="false"/>.</returns>
        public static bool operator !=(ARTrackablesChangedEventArgs<TTrackable> lhs, ARTrackablesChangedEventArgs<TTrackable> rhs)
        {
            return !lhs.Equals(rhs);
        }
    }
}
