using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class Crop
{
    public Vector3Int position;
    public SeedData seedData;
    public int stage;
    public bool watered;
    public GameObject currentStageObject;

    public Crop(Vector3Int pos, SeedData seed)
    {
        position = pos;
        seedData = seed;
        stage = 0;
        watered = false;
        currentStageObject = null;
    }

    public void AdvanceDay(Tilemap tilemap, Transform parent)
    {
        if (!watered) return;
        
        stage = Mathf.Min(stage + 1, seedData.growthPrefabs.Length -1);

        if (stage == 1)
        {
            tilemap.SetTile(position, seedData.plowedTile);
            currentStageObject = GameObject.Instantiate(seedData.growthPrefabs[0], tilemap.GetCellCenterWorld(position), Quaternion.identity, parent);
        }
        else if (stage > 1)
        {
            if (currentStageObject != null)
            {
                GameObject.Destroy(currentStageObject);
            }

            currentStageObject = GameObject.Instantiate(seedData.growthPrefabs[stage -1], tilemap.GetCellCenterWorld(position), Quaternion.identity, parent);
        }

        watered = false;
        
        if (stage > 0)
        {
            tilemap.SetTile(position, seedData.plowedTile);
        }
    }

    public void Water(Tilemap tilemap)
    {
        watered = true;
        if (stage > 0)
        {
            tilemap.SetTile(position, seedData.wateredTile);
        }
    }

    public bool IsHarvestable()
    {
        return stage >= seedData.growthPrefabs.Length - 1;
    }
}
