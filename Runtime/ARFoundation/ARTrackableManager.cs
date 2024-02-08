using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine.XR.ARSubsystems;
using Unity.XR.CoreUtils;
using Unity.XR.CoreUtils.Collections;
using UnityEngine.Events;
using UnityEngine.SubsystemsImplementation;

namespace UnityEngine.XR.ARFoundation
{
    /// <summary>
    /// A base class for trackable managers. Trackable managers use data from tracking subsystems to create and maintain
    /// trackable components and their GameObjects. Refer to
    /// [Trackables and trackable managers](xref:arfoundation-managers#trackables-and-trackable-managers) for more information.
    /// </summary>
    /// <typeparam name="TSubsystem">The tracking subsystem type.</typeparam>
    /// <typeparam name="TSubsystemDescriptor">The subsystem descriptor type.</typeparam>
    /// <typeparam name="TProvider">The subsystem provider type.</typeparam>
    /// <typeparam name="TSessionRelativeData">The subsystem data type.</typeparam>
    /// <typeparam name="TTrackable">The type of component that this component will manage (that is, create, update, and destroy).</typeparam>
    [RequireComponent(typeof(XROrigin))]
    [DisallowMultipleComponent]
    public abstract class ARTrackableManager<TSubsystem, TSubsystemDescriptor, TProvider, TSessionRelativeData, TTrackable>
        : SubsystemLifecycleManager<TSubsystem, TSubsystemDescriptor, TProvider>, ITrackablesChanged<TTrackable>
        where TSubsystem : TrackingSubsystem<TSessionRelativeData, TSubsystem, TSubsystemDescriptor, TProvider>, new()
        where TSubsystemDescriptor : SubsystemDescriptorWithProvider<TSubsystem, TProvider>
        where TProvider : SubsystemProvider<TSubsystem>
        where TSessionRelativeData : struct, ITrackable
        where TTrackable : ARTrackable<TSessionRelativeData, TTrackable>
    {
        static List<TTrackable> s_Added = new();
        static List<TTrackable> s_Updated = new();

        // The deprecated trackable manager APIs ARPlaneManager.planesChanged, ARAnchorManager.anchorsChanged, etc,
        // require a list of TTrackable to be served to the user. Once these deprecated APIs are removed, this
        // List<TTrackable> should be removed as well. Until then, removed trackables are doubly saved in
        // different formats.
        static List<TTrackable> s_Removed = new();
        static List<KeyValuePair<TrackableId, TTrackable>> s_RemovedTrackables = new();

        static ReadOnlyList<TTrackable> s_AddedReadOnly = new(s_Added);
        static ReadOnlyList<TTrackable> s_UpdatedReadOnly = new(s_Updated);
        static ReadOnlyList<KeyValuePair<TrackableId, TTrackable>> s_RemovedReadOnly = new(s_RemovedTrackables);

        /// <summary>
        /// Invoked when trackables have changed (been added, updated, or removed).
        /// </summary>
        [field: SerializeField]
        public UnityEvent<ARTrackablesChangedEventArgs<TTrackable>> trackablesChanged { get; private set; } = new();

        /// <summary>
        /// A dictionary of all trackables keyed by <c>TrackableId</c>.
        /// </summary>
        protected Dictionary<TrackableId, TTrackable> m_Trackables = new();

        /// <summary>
        /// A dictionary of trackables added via <see cref="CreateTrackableImmediate(TSessionRelativeData)"/> but not
        /// yet reported as added.
        /// </summary>
        protected Dictionary<TrackableId, TTrackable> m_PendingAdds = new();

        /// <summary>
        /// The XR Origin component that will be used to instantiate detected trackables.
        /// </summary>
        protected XROrigin origin { get; private set; }

        /// <summary>
        /// The name prefix that should be used when instantiating new GameObjects.
        /// </summary>
        protected abstract string gameObjectName { get; }

        internal static ARTrackableManager<TSubsystem, TSubsystemDescriptor, TProvider, TSessionRelativeData, TTrackable> instance { get; private set; }

        /// <summary>
        /// A collection of all trackables managed by this component.
        /// </summary>
        public TrackableCollection<TTrackable> trackables => new(m_Trackables);

        /// <summary>
        /// The Prefab that should be instantiated when adding a trackable. Can be `null`.
        /// </summary>
        /// <returns>The prefab should be instantiated when adding a trackable.</returns>
        protected virtual GameObject GetPrefab() => null;

        /// <summary>
        /// Saves a reference to the XR Origin component.
        /// </summary>
        protected virtual void Awake()
        {
            origin = GetComponent<XROrigin>();
        }

        /// <inheritdoc />
        protected override void OnEnable()
        {
            base.OnEnable();
            instance = this;
            origin.TrackablesParentTransformChanged += OnTrackablesParentTransformChanged;
        }

        /// <inheritdoc />
        protected override void OnDisable()
        {
            base.OnDisable();
            origin.TrackablesParentTransformChanged -= OnTrackablesParentTransformChanged;
        }

        void OnTrackablesParentTransformChanged(ARTrackablesParentTransformChangedEventArgs eventArgs)
        {
            foreach (var trackable in trackables)
            {
                var trackableTransform = trackable.transform;
                if (trackableTransform.parent != eventArgs.TrackablesParent)
                {
                    var desiredPose = eventArgs.TrackablesParent.TransformPose(trackable.pose);
                    trackableTransform.SetPositionAndRotation(desiredPose.position, desiredPose.rotation);
                }
            }
        }

        /// <summary>
        /// Iterates over every instantiated <see cref="ARTrackable{TSessionRelativeData,TTrackable}"/> and
        /// activates or deactivates its GameObject based on the value of <paramref name="active"/>.
        /// </summary>
        /// <param name="active">If <see langword="true"/>, each trackable's GameObject is activated.
        /// Otherwise, they are deactivated.</param>
        public void SetTrackablesActive(bool active)
        {
            foreach (var trackable in trackables)
            {
                trackable.gameObject.SetActive(active);
            }
        }

        /// <summary>
        /// Determines whether an existing <see cref="ARTrackable{TSessionRelativeData,TTrackable}"/> can be added
        /// to the underlying subsystem.
        /// </summary>
        /// <remarks>
        /// If <paramref name="trackable"/> has not been reported as added yet, and this component is either disabled or
        /// does not have a valid subsystem, then the <paramref name="trackable"/>'s
        /// <see cref="ARTrackable{TSessionRelativeData,TTrackable}.pending"/> state is set to <see langword="true"/>.
        /// </remarks>
        /// <param name="trackable">An existing trackable to add to the subsystem.</param>
        /// <returns>Returns <see langword="true"/> if this manager is enabled, has a valid subsystem, and
        /// <paramref name="trackable"/> is not already being tracked by this manager.
        /// Otherwise, returns <see langword="false"/> otherwise.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="trackable"/> is <see langword="null"/>.
        /// </exception>
        protected bool CanBeAddedToSubsystem(TTrackable trackable)
        {
            if (trackable == null)
                throw new ArgumentNullException(nameof(trackable));

            // If it already has a valid trackableId, then don't re-add it
            if (!trackable.trackableId.Equals(TrackableId.invalidId))
                return false;

            // If we already know about it, then early out
            if (m_Trackables.ContainsKey(trackable.trackableId))
                return false;

            if (!enabled || subsystem == null)
            {
                // If the manager is disabled or there is no subsystem, and we don't already know about
                // this trackable, then it must be pending.
                trackable.pending = true;
                return false;
            }

            // Finally, we can only add it if we have a XR origin.
            return origin && origin.TrackablesParent;
        }

        /// <summary>
        /// Update is called once per frame. This component first updates its internal state, then invokes the
        /// [trackablesChanged](xref:UnityEngine.XR.ARFoundation.ARTrackableManager`5.trackablesChanged) event.
        /// </summary>
        protected virtual void Update()
        {
            if (subsystem == null || !subsystem.running)
                return;

            using (new ScopedProfiler("GetChanges"))
            using (var changes = subsystem.GetChanges(Allocator.Temp))
            {
                using (new ScopedProfiler("ProcessAdded"))
                {
                    s_Added.Clear();
                    foreach (var added in changes.added)
                    {
                        s_Added.Add(CreateOrUpdateTrackable(added));
                    }
                }

                using (new ScopedProfiler("ProcessUpdated"))
                {
                    s_Updated.Clear();
                    foreach (var updated in changes.updated)
                    {
                        s_Updated.Add(CreateOrUpdateTrackable(updated));
                    }
                }

                using (new ScopedProfiler("ProcessRemoved"))
                {
                    s_RemovedTrackables.Clear();
                    s_Removed.Clear();

                    foreach (var trackableId in changes.removed)
                    {
                        if (m_Trackables.Remove(trackableId, out var trackable))
                        {
                            if (trackable == null)
                            {
                                s_RemovedTrackables.Add(new KeyValuePair<TrackableId, TTrackable>(
                                    trackableId, null));
                            }
                            else
                            {
                                s_Removed.Add(trackable);
                                s_RemovedTrackables.Add(new KeyValuePair<TrackableId, TTrackable>(
                                    trackableId, trackable));
                            }
                        }
                    }
                }
            }

            try
            {
                // User events
                bool addedOrUpdated = s_Added.Count > 0 || s_Updated.Count > 0;
                if (addedOrUpdated || s_Removed.Count > 0)
                {
#pragma  warning disable CS0618 // disables warning from deprecation
                    OnTrackablesChanged(s_Added, s_Updated, s_Removed);
#pragma  warning restore CS0618
                }

                if (addedOrUpdated || s_RemovedTrackables.Count > 0)
                {
                    trackablesChanged?.Invoke(new ARTrackablesChangedEventArgs<TTrackable>(
                        s_AddedReadOnly, s_UpdatedReadOnly, s_RemovedReadOnly));
                }
            }
            finally
            {
                // Make sure destroy happens even if a user callback throws an exception
                foreach (var removed in s_Removed)
                    AfterTrackableRemoved(removed);
            }
        }

        /// <summary>
        /// Invoked when trackables have changed (that is, they were added, updated, or removed).
        /// Use this to perform additional logic, or to invoke public events related to your trackables.
        /// </summary>
        /// <param name="added">A list of trackables added this frame.</param>
        /// <param name="updated">A list of trackables updated this frame.</param>
        /// <param name="removed">A list of trackables removed this frame.
        /// The trackable components are not destroyed until after this method returns.</param>
        [Obsolete("OnTrackablesChanged() has been deprecated in AR Foundation version 6.0.", false)]
        protected virtual void OnTrackablesChanged(
            List<TTrackable> added,
            List<TTrackable> updated,
            List<TTrackable> removed)
        { }

        /// <summary>
        /// Invoked after creating the trackable. The trackable's
        /// [sessionRelativeData](xref:UnityEngine.XR.ARFoundation.ARTrackable`2.sessionRelativeData) will already be set.
        /// </summary>
        /// <param name="trackable">The newly created trackable.</param>
        protected virtual void OnCreateTrackable(TTrackable trackable) { }

        /// <summary>
        /// Invoked just after session-relative data has been set on a trackable.
        /// </summary>
        /// <param name="trackable">The trackable that has just been updated.</param>
        /// <param name="sessionRelativeData">The session relative data used to update the trackable.</param>
        protected virtual void OnAfterSetSessionRelativeData(
            TTrackable trackable,
            TSessionRelativeData sessionRelativeData)
        { }

        /// <summary>
        /// Creates a <typeparamref name="TTrackable"/> immediately in a "pending" state.
        /// The trackable will appear in the <see cref="trackables"/> collection immediately, but will not be reported
        /// as added until the subsystem successfully adds it. This is useful for subsystems that deal
        /// with trackables that can be both automatically detected and manually created.
        /// </summary>
        /// <param name="sessionRelativeData">The data associated with the trackable.</param>
        /// <returns>A new trackable.</returns>
        protected TTrackable CreateTrackableImmediate(TSessionRelativeData sessionRelativeData)
        {
            var trackable = CreateOrUpdateTrackable(sessionRelativeData);
            trackable.pending = true;
            m_PendingAdds.Add(trackable.trackableId, trackable);
            return trackable;
        }

        /// <summary>
        /// If the trackable with id <paramref name="trackableId"/> is in a "pending" state, and
        /// <see cref="ARTrackable{TSessionRelativeData, TTrackable}.destroyOnRemoval"/> is <see langword="true"/>,
        /// this method destroys the trackable's GameObject. Otherwise, this method has no effect.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method will immediately remove a trackable only if it was created by
        /// <see cref="CreateTrackableImmediate(TSessionRelativeData)"/>
        /// and has not yet been reported as added by the
        /// <see cref="SubsystemLifecycleManager{TSubsystem,TSubsystemDescriptor,TProvider}.subsystem"/>.
        /// </para>
        /// <para>
        /// This can happen if the trackable is created and removed within the same frame, as the subsystem might never
        /// have a chance to report its existence. Derived classes should use this if they support the concept of manual
        /// addition and removal of trackables.
        /// </para>
        /// </remarks>
        /// <param name="trackableId">The id of the trackable to destroy.</param>
        /// <returns><see langword="true"/> if the trackable was "pending" and is now destroyed.
        /// Otherwise, <see langword="false"/>.</returns>
        protected bool DestroyPendingTrackable(TrackableId trackableId)
        {
            if (m_PendingAdds.TryGetValue(trackableId, out var trackable))
            {
                m_PendingAdds.Remove(trackableId);
                m_Trackables.Remove(trackableId);
                AfterTrackableRemoved(trackable);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Creates the native counterpart for an existing <see cref="ARTrackable{TSessionRelativeData,TTrackable}"/>
        /// added with a call to [AddComponent](xref:UnityEngine.GameObject.AddComponent), for example.
        /// </summary>
        /// <param name="existingTrackable">The existing trackable component.</param>
        /// <param name="sessionRelativeData">The AR data associated with the trackable.</param>
        protected void CreateTrackableFromExisting(TTrackable existingTrackable, TSessionRelativeData sessionRelativeData)
        {
            // Same as CreateOrUpdateTrackable
            var trackableId = sessionRelativeData.trackableId;
            m_Trackables.Add(trackableId, existingTrackable);
            SetSessionRelativeData(existingTrackable, sessionRelativeData);
            OnCreateTrackable(existingTrackable);
            OnAfterSetSessionRelativeData(existingTrackable, sessionRelativeData);
            existingTrackable.OnAfterSetSessionRelativeData();

            // Remaining logic from CreateTrackableImmediate
            m_PendingAdds.Add(trackableId, existingTrackable);
            existingTrackable.pending = true;
        }

        string GetTrackableName(TrackableId trackableId)
        {
            return gameObjectName + " " + trackableId;
        }

        (GameObject gameObject, bool shouldBeActive) CreateGameObjectDeactivated()
        {
            var prefab = GetPrefab();
            if (prefab == null)
            {
                var newGameObject = new GameObject();
                newGameObject.SetActive(false);
                newGameObject.transform.parent = origin.TrackablesParent;
                return (newGameObject, true);
            }

            var active = prefab.activeSelf;
            prefab.SetActive(false);
            var prefabInstance = Instantiate(prefab, origin.TrackablesParent);
            prefab.SetActive(active);
            return (prefabInstance, active);
        }

        (GameObject gameObject, bool shouldBeActive) CreateGameObjectDeactivated(string name)
        {
            var tuple = CreateGameObjectDeactivated();
            tuple.gameObject.name = name;
            return tuple;
        }

        (GameObject gameObject, bool shouldBeActive) CreateGameObjectDeactivated(TrackableId trackableId)
        {
            using (new ScopedProfiler("CreateGameObject"))
            {
                return CreateGameObjectDeactivated(GetTrackableName(trackableId));
            }
        }

        TTrackable CreateTrackable(TSessionRelativeData sessionRelativeData)
        {
            var (trackableGameObject, shouldBeActive) = CreateGameObjectDeactivated(sessionRelativeData.trackableId);
            var trackable = trackableGameObject.GetComponent<TTrackable>();
            if (trackable == null)
            {
                trackable = trackableGameObject.AddComponent<TTrackable>();
            }

            m_Trackables.Add(sessionRelativeData.trackableId, trackable);
            SetSessionRelativeData(trackable, sessionRelativeData);
            trackableGameObject.SetActive(shouldBeActive);

            return trackable;
        }

        void SetSessionRelativeData(TTrackable trackable, TSessionRelativeData data)
        {
            trackable.SetSessionRelativeData(data);
            var worldSpacePose = origin.TrackablesParent.TransformPose(data.pose);
            trackable.transform.SetPositionAndRotation(worldSpacePose.position, worldSpacePose.rotation);
        }

        TTrackable CreateOrUpdateTrackable(TSessionRelativeData sessionRelativeData)
        {
            var trackableId = sessionRelativeData.trackableId;
            if (m_Trackables.TryGetValue(trackableId, out var trackable))
            {
                m_PendingAdds.Remove(trackableId);
                trackable.pending = false;
                SetSessionRelativeData(trackable, sessionRelativeData);
            }
            else
            {
                trackable = CreateTrackable(sessionRelativeData);
                OnCreateTrackable(trackable);
            }

            OnAfterSetSessionRelativeData(trackable, sessionRelativeData);
            trackable.OnAfterSetSessionRelativeData();
            return trackable;
        }

        static void AfterTrackableRemoved(TTrackable trackable)
        {
            if (trackable.destroyOnRemoval)
            {
                Destroy(trackable.gameObject);
            }
        }
    }
}
