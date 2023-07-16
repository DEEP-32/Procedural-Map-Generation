using UnityEngine;


namespace FirstProceduralGeneration
{
    public static class TextureGenerator
    {
        public static Texture2D TextureFromColorMap(Color[] map, int widht, int height)
        {
            Texture2D tex = new Texture2D(widht, height);

            tex.filterMode = FilterMode.Point;
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.SetPixels(map);
            tex.Apply();

            return tex;
        }


        public static Texture2D TextureFromHeightMap(float[,] heightMap)
        {
            int width = heightMap.GetLength(0);
            int height = heightMap.GetLength(1);

            Texture2D texture = new Texture2D(width, height);
            Color[] colorMap = new Color[width * height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, heightMap[x, y]);
                }
            }

            return TextureFromColorMap(colorMap, width, height);
        }
    }
}
