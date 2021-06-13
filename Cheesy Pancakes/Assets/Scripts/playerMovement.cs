using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Code referenced from a movement script from a previous jam

public class playerMovement : MonoBehaviour
{
	
	public CharacterController controller;
	
    public enum JumpState {falling, starting, rising, grounded};

    public JumpState jumpState = JumpState.falling;
    public bool jumpPressed = false;

	public float acceleration = 0.02f;
    public float maxSpeed = 0.4f;
    public Vector3 velocity;

    // Update is called once per frame
    void Update()
    {
        GatherButtonDownInput();
		
		// trans.right is along the right axis of the player
		// .forward is forward and back fromt the players view	
		
    }

    private void FixedUpdate()
    {
        HandleJumpingAndGravity();

        HandleHorizontalMovement();
    }

    void GatherButtonDownInput()
    {
        if (Input.GetButtonDown("Jump"))
        {
            jumpPressed = true;
        }
    }

    void ClearButtonDownInput()
    {
        jumpPressed = false;
    }

    void HandleJumpingAndGravity()
    {
        if (jumpPressed && jumpState == JumpState.grounded)
        {

        }
    }

    void HandleHorizontalMovement()
    {

    }
}