using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalColliderScript : MonoBehaviour
{
    public bool IsTouchingPlayer = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            IsTouchingPlayer = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            IsTouchingPlayer = false;
        }
    }
}
