using FirstProceduralGeneration;
using UnityEngine;

[CreateAssetMenu(fileName = "TerrainData", menuName = "Data/TerrainData")]
public class MeshSettings : UpdatableData
{
    public float scale = 1f;
    public bool useFlatShading;

    public const int numSupportedLODs = 5;
    public const int numSupportedChunkSize = 9;
    public const int numSupportedFlatShadedChunkSize = 3;
    public static readonly int[] supportedChunkSize = { 48, 72, 96, 128, 144, 192, 216, 240 };

    [Range(0f, numSupportedChunkSize - 1)]
    public int chunkSizeIndex;

    [Range(0f, numSupportedFlatShadedChunkSize - 1)]
    public int flatShadedChunkSizeIndex;


    /// <summary>
    /// num verices per line of mesh render at LOD = 0. Includes the two extra vertices that are exclude from final mesh, used for calculating normals
    /// </summary>
    public int numVerticesPerLine
    {
        get
        {
            return supportedChunkSize[(useFlatShading) ? flatShadedChunkSizeIndex : chunkSizeIndex] + 5;

        }
    }

    public float meshWorldSize
    {
        get
        {
            return (numVerticesPerLine - 3) * scale;
        }
    }

}
