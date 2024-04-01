using NUnit.Framework;
using Unity.XR.CoreUtils;

namespace UnityEngine.XR.Simulation.Tests
{
    [TestFixture]
    class SimulationCameraTestFixture : SimulationSessionTestSetup
    {
        [OneTimeSetUp]
        public void Setup()
        {
            SetupSession();
            SetupInput();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            TearDownInput();
            TearDownSession();
        }

        [Test]
        [Order(1)]
        public void PoseProviderAvailable()
        {
            var xrOrigin = Object.FindAnyObjectByType<XROrigin>();
            Assert.IsNotNull(xrOrigin);

            var xrCamera = xrOrigin.Camera;
            Assert.IsNotNull(xrCamera);

            var poseProvider = Object.FindAnyObjectByType<SimulationCameraPoseProvider>();
            Assert.IsNotNull(poseProvider, $"No active {nameof(SimulationCameraPoseProvider)} is available.");
        }
    }
}
