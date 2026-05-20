using System;
using Unity.Collections;
using UnityEngine.SubsystemsImplementation;

namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// Subsystem for managing the participants in a multi-user collaborative session.
    /// </summary>
    public class XRParticipantSubsystem
        : TrackingSubsystem<XRParticipant, XRParticipantSubsystem, XRParticipantSubsystemDescriptor, XRParticipantSubsystem.Provider>
    {
        /// <summary>
        /// Do not call this directly. Call create on a valid <see cref="XRParticipantSubsystemDescriptor"/> instead.
        /// </summary>
        public XRParticipantSubsystem() { }

        /// <summary>
        /// Get the changed participants (added, updated, and removed) since the last call to <see cref="GetChanges(Allocator)"/>.
        /// </summary>
        /// <param name="allocator">An <c>Allocator</c> to use when allocating the returned <c>NativeArray</c>s.</param>
        /// <returns>
        /// <see cref="TrackableChanges{T}"/> that describes the participants that have been added, updated, and removed
        /// since the last call to <see cref="GetChanges(Allocator)"/>. The caller owns the memory allocated with <c>Allocator</c>.
        /// </returns>
        public override TrackableChanges<XRParticipant> GetChanges(Allocator allocator)
        {
            var changes = provider.GetChanges(XRParticipant.defaultParticipant, allocator);
#if !UNITY_6000_6_OR_NEWER || UNITY_ENABLE_CHECKS
#if !UNITY_6000_6_OR_NEWER
            if (m_ValidationUtility != null)
#endif
            {
                m_ValidationUtility.ValidateAndDisposeIfThrown(changes);
            }
#endif
            return changes;
        }

        /// <summary>
        /// The API this subsystem uses to interact with different provider implementations.
        /// </summary>
        public abstract class Provider : SubsystemProvider<XRParticipantSubsystem>
        {
            /// <summary>
            /// Get the changed participants (added, updated, and removed) since the last call to
            /// <see cref="GetChanges(XRParticipant,Allocator)"/>.
            /// </summary>
            /// <param name="defaultParticipant">
            /// The default participant. This should be used to initialize the returned <c>NativeArray</c>s for backwards compatibility.
            /// See <see cref="TrackableChanges{T}.TrackableChanges(void*, int, void*, int, void*, int, T, int, Allocator)"/>.
            /// </param>
            /// <param name="allocator">An <c>Allocator</c> to use when allocating the returned <c>NativeArray</c>s.</param>
            /// <returns>
            /// <see cref="TrackableChanges{T}"/> that describes the participants that have been added, updated, and removed
            /// since the last call to <see cref="GetChanges(XRParticipant,Allocator)"/>. The changes should be allocated using
            /// <paramref name="allocator"/>.
            /// </returns>
            public abstract TrackableChanges<XRParticipant> GetChanges(
                XRParticipant defaultParticipant,
                Allocator allocator);
        }

#if !UNITY_6000_6_OR_NEWER || UNITY_ENABLE_CHECKS
#if UNITY_6000_6_OR_NEWER
        ValidationUtility<XRParticipant> m_ValidationUtility = new();
#else
        ValidationUtility<XRParticipant> m_ValidationUtility = Debug.isDebugBuild ? new ValidationUtility<XRParticipant>() : null;
#endif
#endif
    }
}
