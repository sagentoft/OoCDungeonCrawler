using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimator : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetMovement(Vector2 direction, bool isMoving)
    {
        if (direction.sqrMagnitude > 0.001f)
        {
            Vector2 facing = Mathf.Abs(direction.x) > Mathf.Abs(direction.y) ? new Vector2(Mathf.Sign(direction.x), 0) :
                new Vector2(0, Mathf.Sign(direction.y));
        
            animator.SetFloat("MoveX", facing.x);
            animator.SetFloat("MoveY", facing.y);
        }
        
        animator.SetBool("IsMoving", isMoving);
    }
    
    
}
