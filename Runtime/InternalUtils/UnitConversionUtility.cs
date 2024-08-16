namespace UnityEngine.XR.ARFoundation.InternalUtils
{
    static class UnitConversionUtility
    {
        const float k_MaxLuminosity = 2000.0f;

        public static float ConvertBrightnessToLumens(float brightness)
        {
            return Mathf.Clamp(brightness * k_MaxLuminosity, 0f, k_MaxLuminosity);
        }

        public static float ConvertLumensToBrightness(float lumens)
        {
            return Mathf.Clamp(lumens / k_MaxLuminosity, 0f, 1f);
        }
    }
}
