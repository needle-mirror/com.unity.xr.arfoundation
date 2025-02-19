using NUnit.Framework;
using UnityEngine;

namespace UnityEditor.XR.ARFoundation.Tests
{
    class OcclusionShaderTests
    {
        [Test]
        public void OcclusionPreprocessingShader_Exists()
        {
            Assert.IsNotNull(Shader.Find(ARShaderOcclusionEditor.softOcclusionPreprocessShaderName));
        }
    }
}
