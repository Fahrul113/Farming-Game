using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    [SerializeField] private Tilemap interactableMap;
    public Tilemap InteractableMap => interactableMap;

    [SerializeField] private Tile hiddenInteractableTile;
    [SerializeField] private Tile plowedTile;
    [SerializeField] private Transform cropParent;

    private List<Crop> crops = new List<Crop>();

    void Start()
    {
        foreach(var position in interactableMap.cellBounds.allPositionsWithin)
        {
            TileBase tile = interactableMap.GetTile(position);

            if(tile != null && tile.name == "Interactable_Visible")
            {
                interactableMap.SetTile(position, hiddenInteractableTile);
            }
        }    
    }

    public void Plowing(Vector3Int position)
    {
        interactableMap.SetTile(position, plowedTile);
    }

    public string GetTileName(Vector3Int position)
    {
        if (interactableMap != null)
        {
            TileBase tile = interactableMap.GetTile(position);

            if (tile != null)
            {
                return tile.name;
            }
        }

        return "";
    }

    public void PlantSeed(Vector3Int cellPosition, SeedData seed)
    {
        if (GetCrop(cellPosition) != null) return;

        interactableMap.SetTile(cellPosition, seed.seedTile);
        crops.Add(new Crop(cellPosition, seed)); 
    }

    public void WaterCrop(Vector3Int cellPosition)
    {
        Crop crop = GetCrop(cellPosition);
        if (crop != null)
        {
            crop.Water(interactableMap);
        }
    }

    public bool HarvestCrop(Vector3Int cellPosition, out Item harvestedSeed)
    {
        harvestedSeed = null;
        Crop crop = GetCrop(cellPosition);
        
        if (crop != null && crop.IsHarvestable())
        {
            harvestedSeed = crop.seedData.harvestItem;

            if (crop.currentStageObject != null)
            {
                Destroy(crop.currentStageObject);
            }

            interactableMap.SetTile(cellPosition, hiddenInteractableTile);

            crops.Remove(crop);
            return true;
        }
        return false;
    }

    public void OnDayPassed()
    {
        foreach (Crop crop in crops)
        {
            crop.AdvanceDay(interactableMap, cropParent);
        }
    }

    private Crop GetCrop(Vector3Int cellPosition)
    {
        foreach (Crop crop in crops)
        {
            if (crop.position == cellPosition)
            {
                return crop;
            }
        }
        return null;
    }
}
