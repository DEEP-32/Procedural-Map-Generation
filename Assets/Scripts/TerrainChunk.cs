using FirstProceduralGeneration;
using UnityEngine;
using static FirstProceduralGeneration.TerrainGenerator;

public class TerrainChunk
{
    const float colliderGenerationDstThreshold = 5;
    public event System.Action<TerrainChunk, bool> OnVisibiltyChanged;

    public Vector2 coord;
    GameObject meshObject;
    Vector2 smapleCenter;
    Bounds bounds;

    MeshRenderer meshRenderer;
    MeshFilter meshFilter;
    MeshCollider meshCollider;

    LODInfo[] detailLevels;
    LODMesh[] lodMeshes;
    int colliderLODIndex;

    HeightMap heightMap;
    bool heightMapReceived;
    int previousLODIndex = -1;
    bool hasSetCollider;
    float maxViewDistance;

    HeightMapSettings heightSetting;
    MeshSettings meshSetting;

    Transform viewer;

    public TerrainChunk(Vector2 coord, HeightMapSettings heightSetting, MeshSettings meshSetting, LODInfo[] detailLevels, int colliderLODIndex, Transform parent, Material material, Transform viewer)
    {
        this.viewer = viewer;
        this.coord = coord;
        this.detailLevels = detailLevels;
        this.colliderLODIndex = colliderLODIndex;

        smapleCenter = coord * meshSetting.meshWorldSize / meshSetting.scale;
        Vector2 position = coord * meshSetting.meshWorldSize;
        bounds = new Bounds(position, Vector2.one * meshSetting.meshWorldSize);

        meshObject = new GameObject("Terrain Chunk");
        meshRenderer = meshObject.AddComponent<MeshRenderer>();
        meshFilter = meshObject.AddComponent<MeshFilter>();
        meshCollider = meshObject.AddComponent<MeshCollider>();
        meshRenderer.material = material;

        meshObject.transform.position = new Vector3(position.x, 0, position.y);
        meshObject.transform.parent = parent;

        this.heightSetting = heightSetting;
        this.meshSetting = meshSetting;

        SetVisible(false);

        lodMeshes = new LODMesh[detailLevels.Length];
        for (int i = 0; i < detailLevels.Length; i++)
        {
            lodMeshes[i] = new LODMesh(detailLevels[i].lod);
            lodMeshes[i].updateCallback += UpdateTerrainChunk;
            if (i == colliderLODIndex)
            {
                lodMeshes[i].updateCallback += UpdateCollisionMesh;
            }
        }
        maxViewDistance = detailLevels[detailLevels.Length - 1].visibleDstThreshold;

        
    }

    Vector2 viewerPosition
    {
        get
        {
            return new Vector2(viewer.position.x, viewer.position.z);
        }
    }

    void OnHeightMapRecieved(object heightMapObject)
    {
        this.heightMap = (HeightMap)heightMapObject;
        heightMapReceived = true;

        UpdateTerrainChunk();
    }


    public void Load()
    {
        ThreadedDataRequester.RequestData(() => HeightMapGenerator.GenerateHeightMap(meshSetting.numVerticesPerLine, meshSetting.numVerticesPerLine, heightSetting, smapleCenter), OnHeightMapRecieved);
    }


    public void UpdateTerrainChunk()
    {
        if (heightMapReceived)
        {
            float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));


            bool wasVisible = IsVisible();
            bool visible = viewerDstFromNearestEdge <= maxViewDistance;

            if (visible)
            {
                int lodIndex = 0;

                for (int i = 0; i < detailLevels.Length - 1; i++)
                {
                    if (viewerDstFromNearestEdge > detailLevels[i].visibleDstThreshold)
                    {
                        lodIndex = i + 1;
                    }
                    else
                    {
                        break;
                    }
                }

                if (lodIndex != previousLODIndex)
                {
                    LODMesh lodMesh = lodMeshes[lodIndex];
                    if (lodMesh.hasMesh)
                    {
                        previousLODIndex = lodIndex;
                        meshFilter.mesh = lodMesh.mesh;
                    }
                    else if (!lodMesh.hasRequestedMesh)
                    {
                        lodMesh.RequestMesh(heightMap,meshSetting);
                    }
                }

            }

            if (wasVisible != visible)
            {
                SetVisible(visible);
                OnVisibiltyChanged?.Invoke(this,visible);

            }

        }
    }

    public void UpdateCollisionMesh()
    {
        if (!hasSetCollider)
        {
            float sqrDstFromViewerToEdge = bounds.SqrDistance(viewerPosition);

            if (sqrDstFromViewerToEdge < detailLevels[colliderLODIndex].sqrVisibleDstThreshold)
            {
                if (!lodMeshes[colliderLODIndex].hasRequestedMesh)
                {
                    lodMeshes[colliderLODIndex].RequestMesh(heightMap, meshSetting);
                }
            }

            if (sqrDstFromViewerToEdge <= colliderGenerationDstThreshold * colliderGenerationDstThreshold)
            {

                if (lodMeshes[colliderLODIndex].hasMesh)
                {
                    meshCollider.sharedMesh = lodMeshes[colliderLODIndex].mesh;
                    hasSetCollider = true;
                }
            }
        }


    }
    public void SetVisible(bool visible)
    {
        meshObject.SetActive(visible);
    }

    public bool IsVisible()
    {
        return meshObject.activeSelf;
    }

    class LODMesh
    {

        public Mesh mesh;
        public bool hasRequestedMesh;
        public bool hasMesh;
        int lod;
        public event System.Action updateCallback;

        public LODMesh(int lod)
        {
            this.lod = lod;
        }

        void OnMeshDataReceived(object meshDataObject)
        {
            mesh = ((MeshData)meshDataObject).CreateMesh();
            hasMesh = true;

            updateCallback();
        }

        public void RequestMesh(HeightMap heightMap, MeshSettings meshSetting)
        {
            hasRequestedMesh = true;
            ThreadedDataRequester.RequestData(() => MeshGenerator.GenerateTerrainMesh(heightMap.values, meshSetting, lod), OnMeshDataReceived);
        }

    }



}
