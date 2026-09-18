using NUnit.Framework;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace UnityEditor.XR.ARFoundation.Tests
{
    class ConstantsTests
    {
        /// <summary>
        /// The <c>[Icon]</c> attribute silently falls back to a default icon if its path is wrong, so a broken
        /// path would otherwise go unnoticed until someone looked at a component in the Inspector.
        /// </summary>
        [Test]
        public void ArvrIconPath_ResolvesToTextureAsset()
        {
            var icon = AssetDatabase.LoadAssetAtPath<Texture2D>(Constants.arvrIconPath);
            Assert.IsNotNull(
                icon,
                $"No texture asset was found at '{Constants.arvrIconPath}'. Every AR Foundation component that "
                + "uses the [Icon] attribute depends on this asset, so update the constant or restore the asset.");
        }
    }
}
