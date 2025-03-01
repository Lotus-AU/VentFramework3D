using UnityEngine;
using System.Reflection;
using VentLib.Logging.Default;

namespace VentLib.Utilities;

internal static class AssetLoader
{
    internal static Sprite LoadSprite(string path, float pixelsPerUnit = 100f, bool linear = false, int mipMapLevel = 0)
    {
        var assembly = Assembly.GetExecutingAssembly();
        string assemblyName = "VentLib.";

        if (!path.StartsWith(assemblyName)) path = assemblyName + path;
        using (var stream = assembly.GetManifestResourceStream(path))
        {
            if (stream == null)
            {
                NoDepLogger.Fatal($"Resource '{path}' not found.");
                return null!;
            }

            var texture = new Texture2D(1, 1, TextureFormat.ARGB32, true, linear);
            texture.LoadImage(getthestuff(stream));

            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
            sprite.texture.requestedMipmapLevel = mipMapLevel;
            return sprite;
        }
        byte[] getthestuff(System.IO.Stream input)
        {
            using (var ms = new System.IO.MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}