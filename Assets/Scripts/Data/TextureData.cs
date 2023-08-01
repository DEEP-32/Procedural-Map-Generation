using UnityEngine;

[CreateAssetMenu(fileName ="TextureData",menuName ="Data/TextureData")]
public class TextureData : UpdatableData
{
    public void ApplyToMaterial(Material material)
    {

    }

    public void UpdateMeshHeights(Material mat,float minHeight,float maxHeight)
    {
        mat.SetFloat("minHeight",minHeight);
        mat.SetFloat("maxHeight",maxHeight);
    }
}
