using System;
using System.Threading.Tasks;
using Unity.XR.CoreUtils;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.Simulation
{
    static class SimulationUtils
    {
        internal static async Task RunWithoutCancellationExceptions(Task task)
        {
            try
            {
                await task;
            }
            catch (TaskCanceledException) { }
            catch (AggregateException aggregateException)
            {
                foreach (var e in aggregateException.InnerExceptions)
                {
                    if (e is not TaskCanceledException)
                        throw e;
                }
            }
        }

        internal static async Awaitable RunWithoutCancellationExceptions(Awaitable awaitable)
        {
            try
            {
                await awaitable;
            }
            catch (OperationCanceledException) { }
        }

        internal static TrackableId GenerateTrackableId()
        {
            Guid.NewGuid().Decompose(out var sub1, out var sub2);
            return new TrackableId(sub1, sub2);
        }

        /// <summary>
        /// Returns <see langword="true"/> if <paramref name="gameObject"/> is in the simulation environment scene.
        /// Otherwise, returns <see langword="false"/>.
        /// </summary>
        internal static bool IsInSimulationEnvironment(GameObject gameObject)
        {
            if (gameObject == null)
                return false;

            return gameObject.scene == SimulationSessionSubsystem.simulationSceneManager?.environmentScene;
        }

#if UNITY_EDITOR
        internal static Guid GetTextureGuid(Texture texture)
        {
            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(texture.GetInstanceID(), out string assetGuidString, out var _);
            var guid = new Guid(assetGuidString);
            return guid;
        }
#endif
    }
}
