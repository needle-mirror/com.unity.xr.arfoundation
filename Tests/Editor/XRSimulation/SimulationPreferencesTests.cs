using NUnit.Framework;
using UnityEngine.XR.Simulation;

namespace UnityEditor.XR.Simulation.Tests
{
    [TestFixture]
    public class SimulationPreferencesTests
    {
        [Test]
        public void GetDefaultInputActions_RetrievesActionsSuccessfully()
        {
            Assert.IsNotNull(XRSimulationPreferences.GetDefaultInputActions());
        }
    }
}
