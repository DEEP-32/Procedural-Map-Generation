using FirstProceduralGeneration;
using UnityEngine;

public static class HeightMapGenerator
{
    public static HeightMap GenerateHeightMap(int width,int height,HeightMapSettings settings,Vector2 sampleCenter)
    {
        AnimationCurve heightCurve_threadSafe = new AnimationCurve(settings.heightCurve.keys);
        float[,] values = Noise.GenerateNoiseMap(width, height, settings.noisesetting, sampleCenter);

        float minValue = float.MaxValue;
        float maxValue = float.MinValue;
        
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                values[i, j] *= heightCurve_threadSafe.Evaluate(values[i, j]) * settings.heightMultiplier;
                if (values[i, j] < minValue) { 
                    minValue = values[i, j];
                };
                if (values[i, j] > maxValue) {
                    maxValue = values[i, j];
                };
            }
        }

        return new HeightMap(values, minValue, maxValue);
    }
}

public struct HeightMap
{
    public readonly float[,] values;
    public readonly float minValue;
    public readonly float maxValue;
    public HeightMap(float[,] heightMap, float minValue, float maxValue)
    {
        this.values = heightMap;
        this.minValue = minValue;
        this.maxValue = maxValue;
    }
}
