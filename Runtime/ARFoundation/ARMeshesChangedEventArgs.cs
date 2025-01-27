using System;
using System.Collections.Generic;

namespace UnityEngine.XR.ARFoundation
{
    /// <summary>
    /// Event arguments for the <see cref="ARMeshManager.meshesChanged"/> event.
    /// </summary>
    public struct ARMeshesChangedEventArgs : IEquatable<ARMeshesChangedEventArgs>
    {
        /// <summary>
        /// The list of <c>MeshFilter</c>s added since the last event.
        /// </summary>
        public List<MeshFilter> added { get; }

        /// <summary>
        /// The list of <c>MeshFilter</c>s updated since the last event.
        /// </summary>
        public List<MeshFilter> updated { get; }

        /// <summary>
        /// The list of <c>MeshFilter</c>s removed since the last event.
        /// </summary>
        public List<MeshFilter> removed { get; }

        /// <summary>
        /// Constructs an <see cref="ARMeshesChangedEventArgs"/>.
        /// </summary>
        /// <param name="added">The list of <c>MeshFilter</c>s added since the last event.</param>
        /// <param name="updated">The list of <c>MeshFilter</c>s updated since the last event.</param>
        /// <param name="removed">The list of <c>MeshFilter</c>s removed since the last event.</param>
        public ARMeshesChangedEventArgs(
            List<MeshFilter> added,
            List<MeshFilter> updated,
            List<MeshFilter> removed)
        {
            this.added = added;
            this.updated = updated;
            this.removed = removed;
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
        /// <c>IEquatable</c> interface.
        /// </summary>
        /// <param name="obj">The object to compare for equality.</param>
        /// <returns><c>True</c> if <paramref name="obj"/> is of type <see cref="ARMeshesChangedEventArgs"/>
        /// and compares equal using <see cref="Equals(ARMeshesChangedEventArgs)"/>.</returns>
        public override bool Equals(object obj)
        {
            return obj is ARMeshesChangedEventArgs args && Equals(args);
        }

        /// <summary>
        /// Generates a string representation of this struct, including the number of
        /// added, updated, and removed meshes.
        /// </summary>
        /// <returns>A string representation of this struct.</returns>
        public override string ToString()
        {
            return $"Added: {added?.Count ?? 0}, Updated: {updated?.Count ?? 0}, Removed: {removed?.Count ?? 0}";
        }

        /// <summary>
        /// Compares <paramref name="other"/> for equality.
        /// </summary>
        /// <param name="other">The <see cref="ARMeshesChangedEventArgs"/> to compare for equality.</param>
        /// <returns><c>True</c> if <see cref="added"/>, <see cref="updated"/>, and <see cref="removed"/>
        /// have the same <c>List</c> references as the corresponding properties of <paramref name="other"/>.</returns>
        public bool Equals(ARMeshesChangedEventArgs other)
        {
            return
                ReferenceEquals(added, other.added) &&
                ReferenceEquals(updated, other.updated) &&
                ReferenceEquals(removed, other.removed);
        }

        /// <summary>
        /// Compares for equality. Same as <see cref="Equals(ARMeshesChangedEventArgs)"/>.
        /// </summary>
        /// <param name="lhs">The first <see cref="ARMeshesChangedEventArgs"/> to compare.</param>
        /// <param name="rhs">The second <see cref="ARMeshesChangedEventArgs"/> to compare.</param>
        /// <returns>The same value as <see cref="Equals(ARMeshesChangedEventArgs)"/></returns>
        public static bool operator ==(ARMeshesChangedEventArgs lhs, ARMeshesChangedEventArgs rhs)
        {
            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Compares for inequality. Same as <c>!</c><see cref="Equals(ARMeshesChangedEventArgs)"/>.
        /// </summary>
        /// <param name="lhs">The first <see cref="ARMeshesChangedEventArgs"/> to compare.</param>
        /// <param name="rhs">The second <see cref="ARMeshesChangedEventArgs"/> to compare.</param>
        /// <returns>The same value as <c>!</c><see cref="Equals(ARMeshesChangedEventArgs)"/></returns>
        public static bool operator !=(ARMeshesChangedEventArgs lhs, ARMeshesChangedEventArgs rhs)
        {
            return !lhs.Equals(rhs);
        }
    }
}
