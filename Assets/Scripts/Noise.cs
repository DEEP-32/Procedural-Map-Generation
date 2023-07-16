using UnityEngine;


namespace FirstProceduralGeneration
{
    public static class Noise
    {
        public enum NormalizeMode
        {
            Local,
            Gloabl
        }

        public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacumarity, Vector2 offset, NormalizeMode normalizeMode)
        {
            float[,] noiseMap = new float[mapWidth, mapHeight];

            System.Random prng = new System.Random(seed);
            Vector2[] octaveOffset = new Vector2[octaves];

            float maxPossibleHeight = 0;
            float amplitude = 1f;
            float frequency = 1f;

            for (int i = 0; i < octaves; i++)
            {
                float offsetX = prng.Next(-1000, 1000) + offset.x;
                float offsetY = prng.Next(-1000, 1000) - offset.y;
                octaveOffset[i] = new Vector2(offsetX, offsetY);

                maxPossibleHeight += amplitude;
                amplitude *= persistance;
            }

            if (scale <= 0)
            {
                scale = .001f;
            }

            float maxLocalNoiseHeight = float.MinValue;
            float minLocalNoiseHeight = float.MaxValue;


            float halfWidth = mapWidth / 2f;
            float halfHeight = mapHeight / 2f;

            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    amplitude = 1f;
                    frequency = 1f;
                    float noiseHeight = 0;


                    for (int i = 0; i < octaves; i++)
                    {
                        float sampleX = (x - halfWidth) / scale * frequency  + octaveOffset[i].x * frequency;
                        float sampleY = (y - halfHeight) / scale * frequency - octaveOffset[i].y * frequency;

                        float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                        noiseHeight += perlinValue * amplitude;

                        amplitude *= persistance;
                        frequency *= lacumarity;
                    }
                    if (noiseHeight > maxLocalNoiseHeight)
                    {
                        maxLocalNoiseHeight = noiseHeight;
                    }
                    else if (noiseHeight < minLocalNoiseHeight)
                    {
                        minLocalNoiseHeight = noiseHeight;
                    }
                    noiseMap[y, x] = noiseHeight;
                }

            }

            for (int i = 0; i < mapHeight; i++)
            {
                for (int j = 0; j < mapWidth; j++)
                {
                    if (normalizeMode == NormalizeMode.Local)
                    {
                        noiseMap[i, j] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[i, j]);
                    }
                    else
                    {
                        float normalizedHeight = (noiseMap[i, j] + 1) / (maxPossibleHeight);
                        noiseMap[i, j] = Mathf.Clamp(normalizedHeight,0,int.MaxValue);
                    }
                }

            }




            return noiseMap;
        }
    }
}
