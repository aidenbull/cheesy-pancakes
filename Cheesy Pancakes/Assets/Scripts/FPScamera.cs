using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPScamera : MonoBehaviour
{
	
	public float mouseSensetivity = 1000f;
	
	public Transform playerBody;
	
	float xRotation = 0f; // rotation on Unity's X axis
	
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensetivity * Time.deltaTime;
		float mouseY = Input.GetAxis("Mouse Y") * mouseSensetivity * Time.deltaTime;
		
		xRotation -= mouseY;
		xRotation = Mathf.Clamp(xRotation, -90f, 90f);
		
		transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f); // this seems to negate horizontal rotations...
		playerBody.Rotate(Vector3.up * mouseX); // rotate around y axis (left and right)
    }
}
