using UnityEngine;
using System.Collections;
using ECM.Components; 

public class PlayerAnimatorHandler : MonoBehaviour {

    private CharacterMovement movement; 
    private Animator animator; 
	// Use this for initialization
	void Start () {
        movement = GetComponent<CharacterMovement>();
        animator = GetComponentInChildren<Animator>(); 
    }
	
	// Update is called once per frame
	void Update () {
        if (movement.isGrounded && movement.velocity.sqrMagnitude > 0.01f)
        {
            animator.SetBool("IsWalking", true);
        }
        else
        {
            animator.SetBool("IsWalking", false);
        }
        // Debug.Log("is grounded: " + movement.isGrounded);
    }
}
