using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableTrigger : MonoBehaviour
{
    private IInteractable interactable;

    private void Awake() {
        interactable = GetComponentInParent<IInteractable>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (!other.CompareTag("Player") || interactable == null)
        {
            return;
        }
        
        Player player = other.GetComponent<Player>();
        player.currentInteractable = interactable;
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || interactable == null)
        {
            return;
        }
        Player player = other.GetComponent<Player>();
        if(player.currentInteractable == interactable)
        {
            player.currentInteractable = null;

            if (interactable is Door door)
            {
                door.PlayerExitCollider();
            }
        
        }
        
    }
}
