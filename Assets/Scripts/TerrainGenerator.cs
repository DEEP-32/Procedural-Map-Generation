using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace FirstProceduralGeneration
{
    public class TerrainGenerator : MonoBehaviour
    {
        const float viewerMoveThresholdForChunkUpdate = 25f;
        const float sqrViewerMoveThresholdForChunkUpdate = viewerMoveThresholdForChunkUpdate * viewerMoveThresholdForChunkUpdate;
       

        public int colliderLODIndex;
        public LODInfo[] detailLevels;

        public HeightMapSettings heightSetting;
        public MeshSettings meshSetting;
        public TextureData textureSetting;

        public Transform viewer;
        public Material mapMaterial;

        Vector2 viewerPosition;
        Vector2 viewerPositionOld;
        float meshWorldSize;
        int chunksVisibleInViewDst;

        Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
        List<TerrainChunk> visibleTerrainChunk = new List<TerrainChunk>();

        void Start()
        {
            textureSetting.ApplyToMaterial(mapMaterial);
            textureSetting.UpdateMeshHeights(mapMaterial, heightSetting.minHeight, heightSetting.maxHeight);
            float maxViewDst = detailLevels[detailLevels.Length - 1].visibleDstThreshold;
            meshWorldSize = meshSetting.meshWorldSize;
            chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDst / meshWorldSize);

            UpdateVisibleChunks();
        }

        void Update()
        {
            viewerPosition = new Vector2(viewer.position.x, viewer.position.z);

            if (viewerPosition != viewerPositionOld)
            {
                foreach (var chunk in visibleTerrainChunk)
                {
                    chunk.UpdateCollisionMesh();
                }
            }

            if ((viewerPositionOld - viewerPosition).sqrMagnitude > sqrViewerMoveThresholdForChunkUpdate)
            {
                viewerPositionOld = viewerPosition;
                UpdateVisibleChunks();
            }
        }

        void UpdateVisibleChunks()
        {
            HashSet<Vector2> alreadyUpdatedChunkCoord = new HashSet<Vector2>();
            for (int i = visibleTerrainChunk.Count - 1; i >= 0; i--)
            {
                alreadyUpdatedChunkCoord.Add(visibleTerrainChunk[i].coord);
                visibleTerrainChunk[i].UpdateTerrainChunk();
            }

            int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / meshWorldSize);
            int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / meshWorldSize);

            for (int yOffset = -chunksVisibleInViewDst; yOffset <= chunksVisibleInViewDst; yOffset++)
            {
                for (int xOffset = -chunksVisibleInViewDst; xOffset <= chunksVisibleInViewDst; xOffset++)
                {
                    Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);
                    if (!alreadyUpdatedChunkCoord.Contains(viewedChunkCoord))
                    {
                        if (terrainChunkDictionary.ContainsKey(viewedChunkCoord))
                        {
                            terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();
                        }
                        else
                        {
                            TerrainChunk newChunk = new TerrainChunk(viewedChunkCoord, heightSetting, meshSetting, detailLevels, colliderLODIndex, transform, mapMaterial, viewer);
                            terrainChunkDictionary.Add(viewedChunkCoord, newChunk);
                            newChunk.OnVisibiltyChanged += OnTerrainChunkVisibilityChanged;
                            newChunk.Load();
                        }
                    }

                }
            }
        }

       
        void OnTerrainChunkVisibilityChanged(TerrainChunk chunk, bool visible)
        {
            if (visible)
            {
                visibleTerrainChunk.Add(chunk);
            }
            else
            {
                visibleTerrainChunk.Remove(chunk);
            }
        }
       
    }

    [System.Serializable]
    public struct LODInfo
    {

        [Range(0, MeshSettings.numSupportedLODs - 1)]
        public int lod;
        public float visibleDstThreshold;
        /* public bool useForCollider;*/

        public float sqrVisibleDstThreshold
        {
            get
            {
                return visibleDstThreshold * visibleDstThreshold;
            }
        }
    }
}