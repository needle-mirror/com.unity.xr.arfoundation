using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Tests
{
    static class ObjectTrackingSamples
    {
        /// <summary>
        /// Minimal <see cref="XRReferenceObjectEntry"/> for documentation samples only. Shipping code should use a
        /// provider-specific type (for example <c>ARKitReferenceObjectEntry</c> for ARKit object tracking).
        /// </summary>
        sealed class DocumentationReferenceObjectEntry : XRReferenceObjectEntry
        {
        }

        class ReferenceObjectLibraryAtRuntime : MonoBehaviour
        {
            #region RuntimeCreateReferenceObjectLibrary
            void CreateAndAssignLibraryAtRuntimeExample()
            {
                var myLibrary = ScriptableObject.CreateInstance<XRReferenceObjectLibrary>();
                GetComponent<ARTrackedObjectManager>().referenceLibrary = myLibrary;
            }
            #endregion
        }

        class AddReferenceObjectAtRuntime : MonoBehaviour
        {
            #region RuntimeAddReferenceObject
            void AddReferenceObjectAtRuntimeExample()
            {
                var referenceObject = new XRReferenceObject("My reference object");

                // In production, use your provider-specific XRReferenceObjectEntry (for example ARKitReferenceObjectEntry).
                XRReferenceObjectEntry providerEntry = ScriptableObject.CreateInstance<DocumentationReferenceObjectEntry>();
                referenceObject.AddEntry(providerEntry);

                GetComponent<ARTrackedObjectManager>().referenceLibrary.Add(referenceObject);
            }
            #endregion
        }

        class TrackedObjectManagerAtRuntime : MonoBehaviour
        {
            #region RuntimeAddTrackedObjectManager
            void AddTrackedObjectManagerAtRuntimeExample()
            {
                var myLibrary = ScriptableObject.CreateInstance<XRReferenceObjectLibrary>();
                var manager = gameObject.AddComponent<ARTrackedObjectManager>();
                manager.referenceLibrary = myLibrary;
                manager.enabled = true;
            }
            #endregion
        }
    }
}
