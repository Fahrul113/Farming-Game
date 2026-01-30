using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private Collider2D doorCollider;
    private IDoor door;
    private Animator animator;

    private bool isOpen = false;

    private void Awake() {
        animator = GetComponent<Animator>();    
        
        if (doorCollider == null)
        {
            doorCollider = GetComponent<Collider2D>();
        }
    }

    public void Interact()
    {
        if (isOpen)
        {
            CloseDoor();
        }
        else
        {
            OpenDoor();
        }
    }

    public void PlayerExitCollider()
    {
        if (isOpen)
        {
            CloseDoor();
        }
    }

    public void OpenDoor()
    {
        isOpen = true;
        animator.SetBool("isOpen", true);
        doorCollider.enabled = false;   
    }

    public void CloseDoor()
    {
        isOpen = false;
        animator.SetBool("isOpen", false);
        doorCollider.enabled = true;
    }
    
}
