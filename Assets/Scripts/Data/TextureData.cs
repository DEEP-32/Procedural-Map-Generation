using UnityEngine;

[CreateAssetMenu(fileName ="TextureData",menuName ="Data/TextureData")]
public class TextureData : UpdatableData
{
    public Color[] baseColors;

    [Range(0,1)]
    public float[] baseStartHeight;

    float savedMinHeight, savedMaxHeight;
    public void ApplyToMaterial(Material material)
    {
        Debug.Log("Material is applied");
        material.SetInt("baseColorCount",baseColors.Length);
        material.SetColorArray("baseColors",baseColors);
        material.SetFloatArray("baseStartHeights", baseStartHeight);
        UpdateMeshHeights(material,savedMinHeight,savedMaxHeight);
    }

    public void UpdateMeshHeights(Material mat,float minHeight,float maxHeight)
    {
        savedMinHeight = minHeight;
        savedMaxHeight = maxHeight;

        //Debug.Log("Has min height prop : " + mat.HasFloat("minHeight"));

        mat.SetFloat("minHeight", minHeight);
        mat.SetFloat("maxHeight", maxHeight);
    }
}
