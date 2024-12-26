using System.Text;

namespace UnityEngine.XR.ARFoundation.InternalUtils
{
    static class TextureExtensions
    {
        internal static string ToDebugString(this Texture texture)
        {
            if (texture == null)
                return "null";

            var sb = new StringBuilder();
            sb.AppendLine("{");
            sb.AppendLine($"  width: {texture.width},");
            sb.AppendLine($"  height: {texture.height},");
            sb.AppendLine($"  dimension: {texture.dimension},");
            sb.AppendLine($"  graphicsFormat: {texture.graphicsFormat.ToString()},");
            sb.AppendLine($"  nativeTexturePtr: {texture.GetNativeTexturePtr()}");
            sb.Append("}");
            return sb.ToString();
        }
    }
}
