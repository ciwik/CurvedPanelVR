using UnityEngine;

public static class Texture2DExtensions {

    public static Texture2D Merge(this Texture2D lower, Texture2D upper)
    {
        //TODO: fix it
        if (upper.width != lower.width)
        {
            Texture2D temp = new Texture2D(upper.width, lower.height);
            temp.LoadRawTextureData(lower.GetRawTextureData());
            temp.Resize(upper.width, lower.height);
            lower = temp;
        }

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
