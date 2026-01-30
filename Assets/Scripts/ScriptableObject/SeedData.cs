using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "SeedData", menuName = "ItemData/Seed", order = 0)]
public class SeedData : ItemData
{
    public Tile seedTile;
    public GameObject[] growthPrefabs;
    public int dayPerStage = 2;
    public Tile plowedTile;
    public Tile wateredTile;

    [Header("Harvest")]
    public Item harvestItem;
}
