using System;
using System.Collections.Generic;
using Unity.Collections;
using System.Threading;
using UnityEngine.XR.ARSubsystems;
using static UnityEngine.XR.Simulation.SimulationUtils;

namespace UnityEngine.XR.Simulation
{
    /// <summary>
    /// Helper class for <see cref="SimulationBoundingBoxSubsystem"/> which finds and manages
    /// the bounding boxes in the simulation environment, and fires events correspondingly.
    /// </summary>
    class SimulationBoundingBoxDiscoverer
    {
        struct SimulatedBoundingBoxState
        {
            internal Vector3 position;
            internal Quaternion rotation;
            internal Vector3 size;
            internal TrackingState trackingState;
            internal BoundingBoxClassifications classifications;
        }

        public bool isInitialized => m_Added != null && m_Updated != null && m_Removed != null;

        readonly List<SimulatedBoundingBox> m_BoundingBoxesInScene = new();
        readonly List<SimulatedBoundingBoxState> m_BoundingBoxesStates = new();

        float m_TrackingUpdateIntervalSeconds = 0.1f;
        bool m_Initialized;
        bool m_IsRunning;

        XRBoundingBox[] m_Added;
        int m_NumAdded;
        Dictionary<TrackableId, XRBoundingBox> m_Updated;
        XRBoundingBox[] m_UpdatedCopyBuffer;
        TrackableId[] m_Removed;
        int m_NumRemoved;

        readonly Dictionary<TrackableId, XRBoundingBox> m_AllBoundingBoxes = new();

        CancellationTokenSource m_UpdateCancellationTokenSource;

        public void Start()
        {
            if (!m_Initialized)
            {
                BaseSimulationSceneManager.environmentSetupFinished += Initialize;
                return;
            }

            BeginUpdateLoop();
        }

        public void Stop()
        {
            BaseSimulationSceneManager.environmentSetupFinished -= Initialize;

            m_NumAdded = 0;
            m_NumRemoved = 0;

            if (!m_IsRunning)
                return;

            m_UpdateCancellationTokenSource?.Cancel();

            for (var i = 0; i < m_BoundingBoxesInScene.Count; i++)
            {
                SubsystemRemoveBoundingBoxAtIndex(i);
            }

            m_IsRunning = false;
        }

        async void BeginUpdateLoop()
        {
            m_IsRunning = true;
            using (m_UpdateCancellationTokenSource = new CancellationTokenSource())
            {
                var updateAwaitable = UpdateTracking(m_UpdateCancellationTokenSource.Token);
                await RunWithoutCancellationExceptions(updateAwaitable);
            }
        }

        public void Restart()
        {
            if (!m_Initialized)
                return;

            if (m_IsRunning)
                Stop();

            Reset();

            for (var i = 0; i < m_BoundingBoxesStates.Count; i++)
            {
                m_BoundingBoxesStates[i] = new SimulatedBoundingBoxState()
                {
                    trackingState = TrackingState.None
                };
            }

            BeginUpdateLoopAfterDelay();
        }

        public void Destroy()
        {
            m_AllBoundingBoxes.Clear();
        }

        void Reset()
        {
            ValidateChanges();
            m_NumAdded = 0;
            Array.Clear(m_Added, 0, m_Added.Length);
            m_Updated.Clear();
            m_AllBoundingBoxes.Clear();
        }

        /// <summary>
        /// Necessary to allow the ARManager-side to catch up to the removed trackables,
        /// otherwise we could end up with a duplicate guid entry and an exception will be thrown.
        /// </summary>
        async void BeginUpdateLoopAfterDelay()
        {
            await RunWithoutCancellationExceptions(
                Awaitable.WaitForSecondsAsync(m_TrackingUpdateIntervalSeconds, CancellationToken.None));

            BeginUpdateLoop();
        }

        void Initialize()
        {
            if (m_IsRunning)
                Stop();

            m_BoundingBoxesInScene.Clear();
            m_BoundingBoxesStates.Clear();

            var sceneManager = SimulationSessionSubsystem.simulationSceneManager;
            var simScene = SimulationSessionSubsystem.simulationSceneManager.environmentScene;

            if (!simScene.IsValid())
                throw new InvalidOperationException("The scene loaded for simulation is not valid.");

            var simPhysicsScene = simScene.GetPhysicsScene();

            if (!simPhysicsScene.IsValid())
                throw new InvalidOperationException("The physics scene loaded for simulation is not valid.");

            m_TrackingUpdateIntervalSeconds = XRSimulationRuntimeSettings.Instance.boundingBoxDiscoveryParams.trackingUpdateInterval;

            var simulationBoundingBoxes = sceneManager.simulationEnvironmentBoundingBoxes;
            foreach (var box in simulationBoundingBoxes)
            {
                m_BoundingBoxesInScene.Add(box);
                m_BoundingBoxesStates.Add(CalculateStateOfBoundingBox(box, TrackingState.None));
            }

            InitializeWithCount(m_BoundingBoxesInScene.Count);
            m_Initialized = true;
            BeginUpdateLoop();
        }

        void InitializeWithCount(int numBoundingBoxes)
        {
            m_Added = new XRBoundingBox[numBoundingBoxes];
            m_NumAdded = 0;
            m_Updated = new Dictionary<TrackableId, XRBoundingBox>(numBoundingBoxes);
            m_UpdatedCopyBuffer = new XRBoundingBox[numBoundingBoxes];
            m_Removed = new TrackableId[numBoundingBoxes];
            m_NumRemoved = 0;
        }

        async Awaitable UpdateTracking(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (!m_Initialized)
                {
                    await RunWithoutCancellationExceptions(
                        Awaitable.WaitForSecondsAsync(m_TrackingUpdateIntervalSeconds, cancellationToken));
                    continue;
                }

                using (new ScopedProfiler("XRBoundingBoxTracking"))
                {
                    for (var i = 0; i < m_BoundingBoxesInScene.Count; i++)
                    {
                        var boundingBox = m_BoundingBoxesInScene[i];
                        var prevTrackingState = m_BoundingBoxesStates[i];
                        var newBoundingBoxState = CalculateStateOfBoundingBox(boundingBox, TrackingState.Tracking);

                        if (prevTrackingState.trackingState is TrackingState.None)
                            SubsystemAddBoundingBoxAtIndex(i, newBoundingBoxState);
                        else if (!DoesStateMatch(boundingBox, prevTrackingState))
                        {
                            SubsystemUpdateBoundingBoxAtIndex(i, newBoundingBoxState);
                        }
                    }
                }

                await RunWithoutCancellationExceptions(
                    Awaitable.WaitForSecondsAsync(m_TrackingUpdateIntervalSeconds, cancellationToken));
            }
        }

        static SimulatedBoundingBoxState CalculateStateOfBoundingBox(
            SimulatedBoundingBox boundingBox,
            TrackingState trackingState)
        {
            var pose = boundingBox.GetWorldSpaceCenterPose();
            var state = new SimulatedBoundingBoxState();
            state.position = pose.position;
            state.rotation = pose.rotation;
            state.size = boundingBox.GetWorldSpaceSize();
            state.trackingState = trackingState;
            state.classifications = boundingBox.classifications;
            return state;
        }

        static bool DoesStateMatch(SimulatedBoundingBox boundingBox, SimulatedBoundingBoxState state)
        {
            var pose = boundingBox.GetWorldSpaceCenterPose();
            if (state.position != pose.position)
                return false;

            if (state.rotation != pose.rotation)
                return false;

            if (state.size != boundingBox.GetWorldSpaceSize())
                return false;

            if (state.classifications != boundingBox.classifications)
                return false;

            return true;
        }

        void SubsystemAddBoundingBoxAtIndex(int boxIndex, SimulatedBoundingBoxState simulatedBoundingBoxState)
        {
            var box = m_BoundingBoxesInScene[boxIndex];
            m_BoundingBoxesStates[boxIndex] = simulatedBoundingBoxState;
            var boundingBox = CreateXRBoundingBox(box, simulatedBoundingBoxState.trackingState);
            m_Added[m_NumAdded++] = boundingBox;
            m_AllBoundingBoxes.Add(boundingBox.trackableId, boundingBox);
        }

        void SubsystemUpdateBoundingBoxAtIndex(int boxIndex, SimulatedBoundingBoxState simulatedBoundingBoxState)
        {
            var box = m_BoundingBoxesInScene[boxIndex];
            m_BoundingBoxesStates[boxIndex] = simulatedBoundingBoxState;
            var boundingBox = CreateXRBoundingBox(box, simulatedBoundingBoxState.trackingState);
            m_Updated[boundingBox.trackableId] = boundingBox;
            m_AllBoundingBoxes[boundingBox.trackableId] = boundingBox;
        }

        void SubsystemRemoveBoundingBoxAtIndex(int boxIndex)
        {
            var boundingBox = m_BoundingBoxesInScene[boxIndex];
            m_Removed[m_NumRemoved++] = boundingBox.trackableId;
            m_AllBoundingBoxes.Remove(boundingBox.trackableId);
        }

        static XRBoundingBox CreateXRBoundingBox(SimulatedBoundingBox boundingBox, TrackingState boundingBoxState)
        {
            return new XRBoundingBox(
                trackableId: boundingBox.trackableId,
                pose: boundingBox.GetWorldSpaceCenterPose(),
                size: boundingBox.GetWorldSpaceSize(),
                trackingState: boundingBoxState,
                classifications: boundingBox.classifications,
                nativePtr: IntPtr.Zero);
        }

        /// <summary>
        /// If the same <c>TrackableId</c> is present in more than one list per frame, the
        /// <see cref="ValidationUtility{T}"/> will throw an exception in Editor and development builds.
        /// Because subsystem tracking doesn't update every frame, it's possible that the same trackable has
        /// appeared in multiple lists. Use the most recent update as an add if this happens.
        /// </summary>
        void ValidateChanges()
        {
            for (var i = 0; i < m_NumAdded; i++)
            {
                var trackableId = m_Added[i].trackableId;
                if (!m_Updated.TryGetValue(trackableId, out var latestUpdatedBoundingBox))
                    continue;

                m_Added[i] = latestUpdatedBoundingBox;
                m_Updated.Remove(trackableId);
            }
        }

        public TrackableChanges<XRBoundingBox> GetChanges(
            XRBoundingBox defaultXRBoundingBox,
            Allocator allocator)
        {
            if (!isInitialized)
                return new TrackableChanges<XRBoundingBox>(0, 0, 0, allocator);

            ValidateChanges();

            var numUpdated = m_Updated.Count;

            var changes = new TrackableChanges<XRBoundingBox>(
                m_NumAdded,
                numUpdated,
                m_NumRemoved,
                allocator,
                defaultXRBoundingBox);

            if (m_NumAdded > 0)
            {
                NativeArray<XRBoundingBox>.Copy(m_Added, 0, changes.added, 0, m_NumAdded);
                m_NumAdded = 0;
            }

            if (numUpdated > 0)
            {
                m_Updated.Values.CopyTo(m_UpdatedCopyBuffer, 0);
                NativeArray<XRBoundingBox>.Copy(m_UpdatedCopyBuffer, 0, changes.updated, 0, m_Updated.Count);
                m_Updated.Clear();
            }

            if (m_NumRemoved > 0)
            {
                NativeArray<TrackableId>.Copy(m_Removed, 0, changes.removed, 0, m_NumRemoved);
                m_NumRemoved = 0;
            }

            return changes;
        }

        public IReadOnlyCollection<XRBoundingBox> GetBoundingBoxes()
        {
            return m_AllBoundingBoxes.Values;
        }
    }
}
