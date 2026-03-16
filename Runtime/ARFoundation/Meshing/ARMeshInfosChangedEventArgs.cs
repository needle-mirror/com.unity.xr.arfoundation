using System;
using System.Collections.Generic;
using Unity.XR.CoreUtils.Collections;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation
{
    /// <summary>
    /// Event arguments for the <see cref="ARMeshManager.meshInfosChanged"/> event.
    /// </summary>
    public readonly struct ARMeshInfosChangedEventArgs
    {
        /// <summary>
        /// The list of <c>MeshUpdateInfo</c>s added since the last event.
        /// </summary>
        public ReadOnlyList<MeshUpdateInfo> added { get; }

        /// <summary>
        /// The list of <c>MeshUpdateInfo</c>s updated since the last event.
        /// </summary>
        public ReadOnlyList<MeshUpdateInfo> updated { get; }

        /// <summary>
        /// The list of <c>MeshUpdateInfo</c>s removed since the last event.
        /// </summary>
        public ReadOnlyList<MeshUpdateInfo> removed { get; }

        /// <summary>
        /// Constructs an <see cref="ARMeshInfosChangedEventArgs"/>.
        /// </summary>
        /// <param name="added">The list of <c>MeshUpdateInfo</c>s added since the last event.</param>
        /// <param name="updated">The list of <c>MeshUpdateInfo</c>s updated since the last event.</param>
        /// <param name="removed">The list of <c>MeshUpdateInfo</c>s removed since the last event.</param>
        public ARMeshInfosChangedEventArgs(
            ReadOnlyList<MeshUpdateInfo> added,
            ReadOnlyList<MeshUpdateInfo> updated,
            ReadOnlyList<MeshUpdateInfo> removed
        )
        {
            this.added = added;
            this.updated = updated;
            this.removed = removed;
        }
    }

    /// <summary>
    /// The data bundle returned with <see cref="ARMeshInfosChangedEventArgs"/>.
    /// </summary>
    public readonly struct MeshUpdateInfo
    {
        /// <summary>
        /// The unique identifier of the mesh
        /// </summary>
        public TrackableId id { get; }

        /// <summary>
        /// The MeshFilter with mesh component information
        /// </summary>
        public MeshFilter meshFilter { get; }

        /// <summary>
        /// Construct a new <see cref="MeshUpdateInfo"/>.
        /// </summary>
        /// <param name="id">The ID of the changed mesh</param>
        /// <param name="meshFilter">The mesh information for the changed mesh</param>
        public MeshUpdateInfo(TrackableId id, MeshFilter meshFilter)
        {
            this.id = id;
            this.meshFilter = meshFilter;
        }
    }
}
