namespace UnityEngine.XR.Simulation
{
    static class SimulationConstants
    {
        internal static readonly int[] reservedLayers = { 0, 1, 2, 3, 4, 5, 6, 7, 31 };

        internal const int firstValidLayer = 1;
        internal const int allLayerCount = 32;
        internal const int validLayerCount = allLayerCount - firstValidLayer;

        internal const float sixtyFps = 1.0f / 60;
        internal const float oneHundredTwentyFps = 1.0f / 120;

        internal const string userSettingsPath = "Assets/XR/UserSimulationSettings";
        internal const string runtimeSettingsPath = "Assets/XR";
    }
}
