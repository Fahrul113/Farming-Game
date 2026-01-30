using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Room : MonoBehaviour
{
    [SerializeField] private TilemapRenderer roofRenderer;

    public int playerInside = 0;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player"))
        {
            playerInside++;
            roofRenderer.enabled = false;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player"))
        {
            playerInside--;
            if (playerInside <= 0)
            {
                roofRenderer.enabled = true;
                playerInside = 0;
            }
        }
    }
}
