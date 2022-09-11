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
            var xrOrigin = Object.FindObjectOfType<XROrigin>();
            Assert.IsNotNull(xrOrigin);

            var xrCamera = xrOrigin.Camera;
            Assert.IsNotNull(xrCamera);

            var poseProvider = Object.FindObjectOfType<SimulationCamera>();
            Assert.IsNotNull(poseProvider, $"No active {nameof(SimulationCamera)} is available.");
        }
    }
}
