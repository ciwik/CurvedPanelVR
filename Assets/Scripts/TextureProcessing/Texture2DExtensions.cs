using UnityEngine;

namespace TextureProcessing
{
    public static class Texture2DExtensions {
        /// <summary>
        /// Copy all pixels from two textures into the another one
        /// </summary>
        /// <param name="lower"></param>
        /// <param name="upper"></param>
        /// <returns></returns>
        public static Texture2D Merge(this Texture2D lower, Texture2D upper)
        {
            Texture2D result = new Texture2D(lower.width, upper.height + lower.height);
            for (int x = 0; x < lower.width; x++)
            {
                for (int y = 0; y < lower.height; y++)
                {
                    result.SetPixel(x, y, lower.GetPixel(x, y));
                }

                for (int y = 0; y < upper.height; y++)
                {
                    result.SetPixel(x, lower.height + y, upper.GetPixel(x, y));
                }
            }
            result.Apply();
            return result;
        }
    }
}
