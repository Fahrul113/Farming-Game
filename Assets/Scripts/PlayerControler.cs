using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControler : MonoBehaviour
{
    public float speed;
    public Animator animator;
    
    // Update is called once per frame
    private void Update()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(horizontalInput, verticalInput);

        AnimateMovement(direction);

        transform.Translate(direction * speed * Time.deltaTime); 
    }

    void AnimateMovement(Vector3 direction)
    {
        if (animator != null)
        {  
            if (direction.sqrMagnitude > 0.0001f)
            {
                Vector3 dir = direction.normalized;

                animator.SetBool("isMoving", true);

                animator.SetFloat("moveX", dir.x);
                animator.SetFloat("moveY", dir.y);

                animator.SetFloat("lastMoveX", dir.x);
                animator.SetFloat("lastMoveY", dir.y);
            }
            else
            {
                animator.SetBool("isMoving", false);
            }
        }
    }
}
