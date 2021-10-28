namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// Helper for compile-time constant strings for the [HelpURL](xref:UnityEngine.HelpURLAttribute) attribute.
    /// </summary>
    static class HelpUrls
    {
        const string Version = "5.0";

        const string BaseUrl = "https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@" + Version;

        public const string Manual = BaseUrl + "/manual/arsubsystems/";

        public const string Api = BaseUrl + "/api/";
    }
}
