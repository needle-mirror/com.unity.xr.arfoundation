using System;

namespace UnityEngine.XR.ARFoundation
{
    /// <summary>
    /// Helper for compile-time constant strings for the [HelpURL](xref:UnityEngine.HelpURLAttribute) attribute.
    /// </summary>
    /// <remarks>When a relevant manual page exists, that is normally the best link to use.</remarks>
    class HelpURLAttribute : UnityEngine.HelpURLAttribute
    {
        const string k_Version = "6.1";
        const string k_BaseURL = "https://docs.unity3d.com/Packages/com.unity.xr.arfoundation";

        /// <summary>
        /// Links to the Script Reference page for the specified type.
        /// </summary>
        /// <param name="type">The name of the type.</param>
        public HelpURLAttribute(Type type)
            : base($"{k_BaseURL}@{k_Version}/api/{type.FullName}.html")
        { }

        /// <summary>
        /// Links to a manual page.
        /// </summary>
        /// <param name="manualPage">The file name of the page without extension. Include folders relative to the package `Documentation~
        /// folder, separated with a forward slash (/).</param>
        public HelpURLAttribute(string manualPage)
            : base($"{k_BaseURL}@{k_Version}/manual/{manualPage}.html")
        { }

        /// <summary>
        /// Links to a manual page with anchor.
        /// </summary>
        /// <param name="manualPage">The file name of the page without extension. Include folders relative to the package `Documentation~
        /// folder, separated with a forward slash (/).</param>
        /// <param name="anchor">An anchor on the page (without the # symbol).</param>
        public HelpURLAttribute(string manualPage, string anchor)
            : base($"{k_BaseURL}@{k_Version}/manual/{manualPage}.html#{anchor}")
        { }
    }
}
