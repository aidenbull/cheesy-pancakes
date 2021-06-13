using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Code referenced from a movement script from a previous jam

public class playerMovement : MonoBehaviour
{
	
    public enum JumpState {falling, rising, grounded};

    public JumpState jumpState = JumpState.falling;
    public bool jumpPressed = false;

	public float acceleration = 0.02f;
    public float maxSpeed = 3f;
    public float friction = 0.02f;
    public float jumpSpeed = 2f;
    public float gravity = 0;
    public Vector3 perceivedVelocity;
    public Vector3 velocity;

    public GameObject mainCamera;

    public float currentMouseX;
    public float currentMouseY;
    public float mouseSensitivity = 2f;

    GroundCollider groundCollider;

    private void Start()
    {
        perceivedVelocity = new Vector3();

        groundCollider = transform.Find("groundCollider").gameObject.GetComponent<GroundCollider>();

        groundCollider.OnGroundEnter += OnGroundEnter;
        groundCollider.OnGroundExit += OnGroundExit;

        mainCamera = transform.Find("Main Camera").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        GatherButtonDownInput();

        HandleCameraMovement();
    }

    private void FixedUpdate()
    {
        HandleJumpingAndGravity();

        HandleHorizontalMovement();

        ApplyMovement();

        ClearButtonDownInput();
    }

    void HandleCameraMovement()
    {
        currentMouseX = Input.GetAxis("Mouse X");

        float rotationY = transform.localEulerAngles.y;
        rotationY += currentMouseX * mouseSensitivity;

        transform.localEulerAngles = new Vector3(0f, rotationY, 0f);

        
        currentMouseY = Input.GetAxis("Mouse Y");

        float rotationX = mainCamera.transform.localEulerAngles.x;
        rotationX -= currentMouseY * mouseSensitivity;

        mainCamera.transform.localEulerAngles = new Vector3(rotationX, 0f, 0f);
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
            perceivedVelocity.y = jumpSpeed;
            jumpState = JumpState.falling;
        }

        if (jumpState == JumpState.rising || jumpState == JumpState.falling)
        {
            perceivedVelocity.y += gravity;
        }
        else
        {
            perceivedVelocity.y = 0;
        }
    }

    void HandleHorizontalMovement()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 currentHorizontalVelocity = new Vector3(perceivedVelocity.x, 0f, perceivedVelocity.z);
        Vector3 move = new Vector3();
        //Vector3 move = (transform.right * x + transform.forward * z) * acceleration;
        if (currentHorizontalVelocity.magnitude < maxSpeed)
        {
            move = new Vector3(x, 0f, z).normalized * acceleration;
        }    
        
        if (move.magnitude == 0 && currentHorizontalVelocity.magnitude < friction)
        {
            move = -currentHorizontalVelocity;
        }
        else
        {
            move -= (currentHorizontalVelocity.normalized) * friction;
        }

        perceivedVelocity += move;

        //Vector3 horizontalVelocity = new Vector3(velocity.x, 0f, velocity.z);

        //if (horizontalVelocity.magnitude > 0)
        //{
        //    if (horizontalVelocity.magnitude > friction)
        //    {
        //        velocity += (-horizontalVelocity).normalized * friction;
        //    }
        //    else
        //    {
        //        velocity -= horizontalVelocity;
        //    }
            
        //}
    }

    void ApplyMovement()
    {
        transform.position += transform.rotation * perceivedVelocity;
    }

    //Taken from previous jam because im very tired and cant think pls forgive
    private List<GameObject> currentGrounds = new List<GameObject>();
    private void OnGroundEnter(object _, Collider ground)
    {
        Debug.Log("entered ground");

        currentGrounds.Add(ground.gameObject);

        RefreshGroundState();
    }

    private void OnGroundExit(object _, Collider ground)
    {
        Debug.Log("left ground");

        currentGrounds.Remove(ground.gameObject);

        RefreshGroundState();
    }

    private void RefreshGroundState()
    {
        if (currentGrounds.Count > 0)
        {
            jumpState = JumpState.grounded;
        }
        else
        {
            jumpState = JumpState.falling;
        }
    }
}