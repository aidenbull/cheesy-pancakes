using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingCameraScript : MonoBehaviour
{
    public float speed = 0.05f;
    public Vector3 movement = new Vector3();
    public float currentMouseX;
    public float currentMouseY;
    public float mouseSensitivity = 2f;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    bool inPortal = false;
    // Update is called once per frame
    void FixedUpdate()
    {
        float sideMovement = Input.GetAxis("Horizontal");
        float forwardMovement = Input.GetAxis("Vertical");
        movement = Quaternion.Euler(transform.localEulerAngles) * new Vector3(sideMovement, 0f, forwardMovement);
        transform.position += movement * speed;

        currentMouseX = Input.GetAxis("Mouse X");
        currentMouseY = Input.GetAxis("Mouse Y");

        float rotationY = transform.localEulerAngles.y;
        rotationY += currentMouseX * mouseSensitivity;
        float rotationX = transform.localEulerAngles.x;
        rotationX -= currentMouseY * mouseSensitivity;
        transform.localEulerAngles = new Vector3(rotationX, rotationY, 0f);
    }
}
