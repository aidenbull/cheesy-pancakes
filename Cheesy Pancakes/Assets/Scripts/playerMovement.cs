using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
	
	// this is the video i watched for reference:
	//https://www.youtube.com/watch?v=_QajrabyTJc
	
	// seems his implementation was out of date compared to this unity verion
	// cause i followed it exactly, and it doesnt work that great.
	
	public CharacterController controller;
	
	public float speed = 12f;

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
		float z = Input.GetAxis("Vertical");
		
		Vector3 move = transform.right * x + transform.forward * z; 
		// trans.right is along the right axis of the player
		// .forward is forward and back fromt the players view		
		
		controller.Move(move * speed * Time.deltaTime);
		
    }
}