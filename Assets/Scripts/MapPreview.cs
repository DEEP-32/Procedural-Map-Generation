using UnityEngine;


namespace FirstProceduralGeneration
{
    public class MapPreview : MonoBehaviour
    {
        public enum DrawMode { NoiseMap, Mesh, FalloffMap };
        public DrawMode drawMode;


        public MeshSettings meshSettings;
        public HeightMapSettings heightMapSettings;
        public TextureData textureData;

        public Material terrainMaterial;



        [Range(0, MeshSettings.numSupportedLODs - 1)]
        public int editorPreviewLOD;

        public bool autoUpdate;

        public Renderer textureRender;
        public MeshFilter meshFilter;
        public MeshRenderer meshRenderer;


        public void DrawTexture(Texture2D texture)
        {
            textureRender.sharedMaterial.mainTexture = texture;
            textureRender.transform.localScale = new Vector3(texture.width, 1, texture.height) / 10f;

            textureRender.gameObject.SetActive(true);
            meshFilter.gameObject.SetActive(false);
        }

        public void DrawMesh(MeshData meshData)
        {
            meshFilter.sharedMesh = meshData.CreateMesh();
            textureRender.gameObject.SetActive(false);
            meshFilter.gameObject.SetActive(true);
        }

        public void DrawMapInEditor()
        {
            textureData.ApplyToMaterial(terrainMaterial);
            textureData.UpdateMeshHeights(terrainMaterial, heightMapSettings.minHeight, heightMapSettings.maxHeight);
            HeightMap heightMap = HeightMapGenerator.GenerateHeightMap(meshSettings.numVerticesPerLine, meshSettings.numVerticesPerLine, heightMapSettings, Vector2.zero);

           
            if (drawMode == DrawMode.NoiseMap)
            {
                DrawTexture(TextureGenerator.TextureFromHeightMap(heightMap));
            }
            else if (drawMode == DrawMode.Mesh)
            {
                DrawMesh(MeshGenerator.GenerateTerrainMesh(heightMap.values, meshSettings, editorPreviewLOD));
            }
            else if (drawMode == DrawMode.FalloffMap)
            {
                DrawTexture(TextureGenerator.TextureFromHeightMap(new HeightMap(FallOfGenerator.GenerateFalloffMap(meshSettings.numVerticesPerLine),0,1)));
            }
        }


        void OnTextureValuesUpdated()
        {
            textureData.ApplyToMaterial(terrainMaterial);
        }
        void OnValidate()
        {
            if (meshSettings != null)
            {
                meshSettings.OnValueUpdated -= OnValuesUpdated;
                meshSettings.OnValueUpdated += OnValuesUpdated;
            }
            if (heightMapSettings != null)
            {
                heightMapSettings.OnValueUpdated -= OnValuesUpdated;
                heightMapSettings.OnValueUpdated += OnValuesUpdated;
            }

            if (textureData != null)
            {
                textureData.OnValueUpdated -= OnTextureValuesUpdated;
                textureData.OnValueUpdated += OnTextureValuesUpdated;
            }

        }

        void OnValuesUpdated()
        {
            if (!Application.isPlaying)
            {
                DrawMapInEditor();
            }

        }
    }
}

