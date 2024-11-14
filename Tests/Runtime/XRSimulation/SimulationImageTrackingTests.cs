#if UNITY_EDITOR
using System.Collections;
using System.Linq;
using System.Runtime.InteropServices;
using NUnit.Framework;
using Unity.Collections;
using Unity.XR.CoreUtils;
using UnityEngine.TestTools;
using UnityEngine.XR.ARSubsystems;
using SerializableGuid = UnityEngine.XR.ARSubsystems.SerializableGuid;

namespace UnityEngine.XR.Simulation.Tests
{
    [TestFixture]
    class SimulationImageTrackingTests : SimulationSessionTestSetup
    {
        private const string k_EnvironmentPrefabPath = "Packages/com.unity.xr.arfoundation/Assets/Prefabs/DefaultSimulationEnvironment.prefab";

        private const float k_AsyncOperationTimeout = 1.0F;

        private bool wasEnvironmentLoaded { get; set; }

        private void HandleEnvironmentLoaded() => wasEnvironmentLoaded = true;

        class TrackingDiscoveryParams : ITrackedImageDiscoveryParams
        {
            public float trackingUpdateInterval => 0.05F;
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            SetupLoader();
            AddXROrigin();

            wasEnvironmentLoaded = false;
            SimulationSessionSubsystem.simulationSceneManagerFactory = () =>
            {
                BaseSimulationSceneManager.environmentSetupFinished += HandleEnvironmentLoaded;
                return new SimulationTestSceneManager(k_EnvironmentPrefabPath);
            };

            SimulationTrackedImageDiscoverer.trackedImageDiscoveryParams = new TrackingDiscoveryParams();

            StartSubsystem<XRSessionSubsystem, SimulationSessionSubsystem>();
            StartSubsystem<XRInputSubsystem, XRInputSubsystem>();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            StopSubsystem<XRInputSubsystem, XRInputSubsystem>();
            StopSubsystem<XRSessionSubsystem, SimulationSessionSubsystem>();

            SimulationTrackedImageDiscoverer.trackedImageDiscoveryParams = null;

            SimulationSessionSubsystem.simulationSceneManagerFactory = null;
            BaseSimulationSceneManager.environmentSetupFinished -= HandleEnvironmentLoaded;

            RemoveXROrigin();
            TearDownLoader();
        }

        [UnityTest]
        public IEnumerator TestDynamicReferenceImageAddingWorks()
        {
            // wait until play mode has settled in, and the simulation environment
            // has been added to the scene
            for (var elapsed = 0.0F; !wasEnvironmentLoaded && elapsed < k_AsyncOperationTimeout; elapsed += Time.deltaTime)
            {
                yield return null;
            }
            Assert.IsTrue(wasEnvironmentLoaded);

            StartSubsystem<XRCameraSubsystem, SimulationCameraSubsystem>();

            // note: we cannot simply start the image tracking subsystem because before calling
            // 'Start()' on it, we first need to make sure that a runtime reference image
            // library is first associated with it.
            var imageTrackingSubsystem = GetSimulationSubsystem<XRImageTrackingSubsystem, SimulationImageTrackingSubsystem>();
            imageTrackingSubsystem.imageLibrary = imageTrackingSubsystem.CreateRuntimeLibrary(null);
            imageTrackingSubsystem.Start();

            var sessionSubsystem = GetSubsystem<XRSessionSubsystem>();

            // wait until the simulation session subsystem is tracking
            for (var elapsed = 0.0F; sessionSubsystem.trackingState != TrackingState.Tracking && elapsed < k_AsyncOperationTimeout; elapsed += Time.deltaTime)
            {
                UpdateTracking(sessionSubsystem);
                yield return null;
            }
            Assert.IsTrue(sessionSubsystem.trackingState == TrackingState.Tracking);

            // make sure that we have some simulation images in our environment
            var sceneManager = SimulationSessionSubsystem.simulationSceneManager;
            Assert.IsTrue(sceneManager.simulationEnvironmentImages.Count > 0);

            // make sure the first image is legit
            var firstSimulatedTrackedImage = sceneManager.simulationEnvironmentImages.First();
            Assert.IsNotNull(firstSimulatedTrackedImage);

            var image = firstSimulatedTrackedImage.texture;
            Assert.IsNotNull(image);

            // make sure that our reference library is mutable and is a simulation one
            var imageLibrary = imageTrackingSubsystem.imageLibrary;
            Assert.IsTrue(imageLibrary is MutableRuntimeReferenceImageLibrary);
            Assert.IsTrue(imageLibrary is SimulationRuntimeImageLibrary);

            // get the camera so that we can get it to face the track image
            var xrOrigin = Object.FindFirstObjectByType<XROrigin>();
            Assert.IsNotNull(xrOrigin);
            var xrCamera = xrOrigin.Camera;
            Assert.IsNotNull(xrCamera);
            var simulationPoseControls = Object.FindFirstObjectByType<SimulationCameraPoseProvider>();
            Assert.IsNotNull(simulationPoseControls);

            // note: the XROrigin creation utility does not guarantee an identity for
            // the transforms of XROrigin and camera offset.
            xrOrigin.CameraYOffset = 0.0F;
            MoveToWorldspaceOrigin(xrOrigin.transform);
            MoveToWorldspaceOrigin(xrCamera.transform);

            // turn off the simulation input pose controls, and have the camera face
            // directly at the track image so it gets tracked by the image subsystem
            simulationPoseControls.enabled = false;

            var trackImagePosition = firstSimulatedTrackedImage.transform.position;
            var trackImageNormal = firstSimulatedTrackedImage.transform.up;
            var desiredCameraPosition = trackImagePosition + trackImageNormal;
            var desiredCameraForward = -trackImageNormal;
            var desiredCameraRotation = Quaternion.LookRotation(desiredCameraForward);
            SetCameraPose(
                desiredCameraPosition.x, desiredCameraPosition.y, desiredCameraPosition.z,
                desiredCameraRotation.x, desiredCameraRotation.y, desiredCameraRotation.z, desiredCameraRotation.w);

            var changes = imageTrackingSubsystem.GetChanges(Allocator.Temp);

            // give the image tracking system time to execute and update image tracking
            for (var elapsed = 0.0F; elapsed < k_AsyncOperationTimeout; elapsed += Time.deltaTime)
            {
                UpdateTracking(sessionSubsystem);
                changes = imageTrackingSubsystem.GetChanges(Allocator.Temp);
                if (changes.added.Length == 1)
                    break;
                yield return null;
            }

            // make sure that our tracked image has a reference image with a fallback
            // source image since there is no reference image yet
            Assert.IsTrue(changes.added[0].sourceImageId == firstSimulatedTrackedImage.fallbackSourceImageId);

            // add our test reference image to the library
            // we use the asset that is present in the environment so that we will
            // get a match on the texture asset GUID
            var mutableLibrary = imageLibrary as SimulationRuntimeImageLibrary;
            // NOTE: we do not validate because tests are run in "headless mode" with
            // -nographics, and therefore textures are never really created in those
            // situations.
            mutableLibrary.validationOverride = () => false;
            // so we stand up a "dummy" image that will have the same asset GUID for
            // simulation tracking purposes
            var jobState = ScheduleDummyReferenceImageAddition(mutableLibrary, image);

            // wait until the reference image job completes
            for (var elapsed = 0.0F; !jobState.jobHandle.IsCompleted && elapsed < k_AsyncOperationTimeout; elapsed += Time.deltaTime)
            {
                yield return null;
            }
            Assert.IsTrue(jobState.jobHandle.IsCompleted);

            // give the image tracking system time to execute and update image tracking
            for (var elapsed = 0.0F; elapsed < k_AsyncOperationTimeout; elapsed += Time.deltaTime)
            {
                UpdateTracking(sessionSubsystem);
                changes = imageTrackingSubsystem.GetChanges(Allocator.Temp);
                if (changes.updated.Length == 1)
                    break;
                yield return null;
            }

            // now make sure that our tracked image has a source reference image
            // with a GUID that is legit
            Assert.IsTrue(changes.updated[0].sourceImageId == mutableLibrary[0].guid);
        }

        private void UpdateTracking(XRSessionSubsystem sessionSubsystem)
        {
            sessionSubsystem.Update(new XRSessionUpdateParams
            {
                screenOrientation = Screen.orientation,
                screenDimensions = new Vector2Int(Screen.width, Screen.height)
            });
        }

        private AddReferenceImageJobState ScheduleDummyReferenceImageAddition(SimulationRuntimeImageLibrary mutableLibrary, Texture2D image)
        {
            var imageBytes = new NativeArray<byte>(1, Allocator.Temp);
            var imageFormat = TextureFormat.Alpha8;
            var sizeInPixels = new Vector2Int(1, 1);
            var widthInMeters = 0.1F;
            var size = new Vector2(widthInMeters, widthInMeters);
            var imageName = "TestReferenceImage";
            var referenceImage = new XRReferenceImage(
                SerializableGuid.empty,
                SerializableGuid.empty,
                size, imageName, image);
            return mutableLibrary.ScheduleAddImageWithValidationJob(
                imageBytes,
                sizeInPixels,
                imageFormat,
                referenceImage);
        }

        private void MoveToWorldspaceOrigin(Transform transform)
        {
            if (transform.position != Vector3.zero)
                transform.position = Vector3.zero;

            if (transform.rotation != Quaternion.identity)
                transform.rotation = Quaternion.identity;
        }

        [DllImport("XRSimulationSubsystem", EntryPoint = "XRSimulationSubsystem_SetCameraPose")]
        static extern void SetCameraPose(
            float pos_x, float pos_y, float pos_z,
            float rot_x, float rot_y, float rot_z, float rot_w);
    }
}
#endif//UNITY_EDITOR
