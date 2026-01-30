using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public InventoryManager inventoryManager;
    private TileManager tileManager;
    private Animator animator;

    public IInteractable currentInteractable;

    private void Start() 
    {
        tileManager = GameManager.instance.tileManager;
        animator = gameObject.GetComponentInChildren<Animator>();
    }

    private void Awake()
    {
        inventoryManager = GetComponent<InventoryManager>();
    }

    private void Update()
    {
        Vector3Int cellPosition = tileManager.InteractableMap.WorldToCell(transform.position);
        string tileName = tileManager.GetTileName(cellPosition);
        var selected = inventoryManager.toolbar.selectedSlot;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (tileManager != null)
            {
                if (selected != null)
                {
                    string itemName = selected.itemName;

                    if (itemName == "Hoe" && !string.IsNullOrWhiteSpace(tileName))
                    {
                        animator.SetTrigger("isPlowing");
                        if (tileName == "Interactable") 
                        {
                            tileManager.Plowing(cellPosition);
                        }
                    }
                    else if (itemName.Contains("Seed") && tileName == "Summer_Plowed")
                    {
                        Item item = GameManager.instance.itemManager.GetItemByName(itemName);
                        if (item == null)
                        {
                            Debug.LogError($"Item not found in ItemManager: { itemName }");
                            return;
                        }
                        SeedData seed = item.data as SeedData;
                        if (seed != null)
                        {
                            tileManager.PlantSeed(cellPosition, seed);
                            selected.RemoveItem();

                            GameManager.instance.uiManager.RefreshAll();
                        }
                    }
                    else if (itemName == "WateringCan")
                    {
                        animator.SetTrigger("isWatering");
                        tileManager.WaterCrop(cellPosition);
                    }
                }
            }   
        }

        if(Input.GetKeyDown(KeyCode.E) )
        {
            if (tileManager.HarvestCrop(cellPosition, out Item harvestedItem))
            {
                inventoryManager.Add("Backpack", harvestedItem);

            } else if ( currentInteractable != null)
            {
                currentInteractable.Interact();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        IInteractable interactable = other.GetComponentInParent<IInteractable>();
        if (interactable != null)
        {
            currentInteractable = interactable;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        IInteractable interactable = other.GetComponentInParent<IInteractable>();
        if (interactable != null && currentInteractable == interactable)
        {
            currentInteractable = null;
        }
    }

    public void DropItem(Item item)
    {
        Vector2 spawnLocation = transform.position;

        Vector2 spawnOffset = Random.insideUnitCircle * 1.25f;

        Item droppedItem = Instantiate(item, spawnLocation + spawnOffset, Quaternion.identity);

        droppedItem.rb2d.AddForce(spawnOffset * 2f, ForceMode2D.Impulse);
    }

    public void DropItem(Item item, int numToDrop)
    {
        for (int i = 0; i < numToDrop; i++)
        {
            DropItem(item);
        }
    }
}
