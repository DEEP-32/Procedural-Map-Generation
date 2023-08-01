using FirstProceduralGeneration;
using UnityEngine;

[CreateAssetMenu(fileName ="NoiseData",menuName ="Data/NoiseData")]
public class NoiseData : UpdatableData
{
    public Noise.NormalizeMode normalizeMode;
    public float noiseScale;

    public int octaves;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;

    public int seed;
    public Vector2 offset;

    override protected void OnValidate()
    {
        if (lacunarity < 1)
        {
            lacunarity = 1;
        }
        if (octaves < 0)
        {
            octaves = 0;
        }

        base.OnValidate();
    }
}
