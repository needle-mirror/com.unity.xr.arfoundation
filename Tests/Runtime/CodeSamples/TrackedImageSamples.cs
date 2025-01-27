using NUnit.Framework;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Tests
{
    [TestFixture]
    class TrackedImageSamples
    {
        // Disable "field never assigned to"
        #pragma warning disable CS0649

        #region trackedimage_addmanager_to_gameobject
        void AddManagerToGameObject(GameObject g, XRReferenceImageLibrary library)
        {
            var manager = g.AddComponent<ARTrackedImageManager>();
            manager.referenceLibrary = library;
            manager.enabled = true;
        }
        #endregion

        #region trackedimage_setlibrary
        void SetLibrary(ARTrackedImageManager manager, XRReferenceImageLibrary library)
        {
            manager.referenceLibrary = library;
        }
        #endregion

        #region trackedimage_createruntimelibraryfromserialized
        void CreateRuntimeLibrary(ARTrackedImageManager m, XRReferenceImageLibrary lib)
        {
            var runtimeLibrary = m.CreateRuntimeLibrary(lib);
        }
        #endregion

        #region trackedimage_createemptyruntimelibrary
        void CreateEmptyRuntimeLibrary(ARTrackedImageManager m)
        {
            var runtimeLibrary = m.CreateRuntimeLibrary();
        }
        #endregion

        #region trackedimage_addtomutablelibrary
        void AddImage(
            ARTrackedImageManager m, RuntimeReferenceImageLibrary lib, Texture2D tex)
        {
            if (lib is MutableRuntimeReferenceImageLibrary mutableLibrary)
            {
                mutableLibrary.ScheduleAddImageWithValidationJob(
                    tex,
                    "my new image",
                    0.5f /* 50 cm */);
            }
        }
        #endregion

        class EnumerateTrackables
        {
            #region trackedimage_subscribe_to_events
            [SerializeField]
            ARTrackedImageManager m_ImageManager;

            void OnEnable() => m_ImageManager.trackablesChanged.AddListener(OnChanged);

            void OnDisable() => m_ImageManager.trackablesChanged.RemoveListener(OnChanged);

            void OnChanged(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs)
            {
                foreach (var newImage in eventArgs.added)
                {
                    // Handle added event
                }

                foreach (var updatedImage in eventArgs.updated)
                {
                    // Handle updated event
                }

                foreach (var removed in eventArgs.removed)
                {
                    // Handle removed event
                    TrackableId removedImageTrackableId = removed.Key;
                    ARTrackedImage removedImage = removed.Value;
                }
            }
            #endregion
        }

        #region trackedimage_DeallocateOnJobCompletion
        [SerializeField]
        ARTrackedImageManager m_Manager;

        struct DeallocateJob : IJob
        {
            [DeallocateOnJobCompletion]
            public NativeArray<byte> data;

            public void Execute() { }
        }

        void AddImage(NativeArray<byte> grayscaleImageBytes,
                      int widthInPixels, int heightInPixels, float widthInMeters)
        {
            if (m_Manager.referenceLibrary is MutableRuntimeReferenceImageLibrary lib)
            {
                var aspectRatio = (float)widthInPixels / (float)heightInPixels;
                var sizeInMeters = new Vector2(widthInMeters, widthInMeters * aspectRatio);
                var referenceImage = new XRReferenceImage(
                    SerializableGuid.empty, // Guid is assigned after image is added
                    SerializableGuid.empty, // We don't have a Texture2D
                    sizeInMeters, "My Image", null);

                var jobState = lib.ScheduleAddImageWithValidationJob(
                    grayscaleImageBytes,
                    new Vector2Int(widthInPixels, heightInPixels),
                    TextureFormat.R8,
                    referenceImage);

                // Schedule a job that deallocates the image bytes after the image
                // is added to the reference image library.
                new DeallocateJob { data = grayscaleImageBytes }
                    .Schedule(jobState.jobHandle);
            }
            else
            {
                // Cannot add the image, so dispose its memory.
                grayscaleImageBytes.Dispose();
            }
        }
        #endregion

        #pragma warning restore CS0649
    }
}
