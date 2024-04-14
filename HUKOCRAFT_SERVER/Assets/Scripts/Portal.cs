using System;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public Transform otherTrigger;
    private Transform playerTransform;
    private bool playerThroughPortal = false;
    private bool isInfiniteFallTrigerred = false; //PLAYER IS AT ASYLUM AND TRIGGERED INFINITE PORTAL

    public void Update()
    {
        try
        {
            if (playerThroughPortal == true && playerTransform != null)
            {
                Vector3 portalToPlayer = playerTransform.position - transform.position;
                float dotProduct = Vector3.Dot(transform.up, portalToPlayer);

                if (dotProduct < 0f || isInfiniteFallTrigerred == true)
                {
                    float rotationalDiff = -Quaternion.Angle(transform.rotation, otherTrigger.rotation);
                    rotationalDiff += 180;
                    playerTransform.Rotate(Vector3.up, rotationalDiff);
                    Vector3 positionOffset = Quaternion.Euler(0f, rotationalDiff, 0f) * portalToPlayer;
                    playerTransform.position = otherTrigger.position + positionOffset;
                    
                    playerThroughPortal = false;
                }
            }
        }
        catch (Exception) { playerTransform = null; }
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            playerTransform = other.gameObject.transform;
            playerThroughPortal = true;

            if (gameObject.name.Equals("Plane_G [1]"))
                isInfiniteFallTrigerred = true;
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            playerThroughPortal = false;
            isInfiniteFallTrigerred = false;
        }
            
    }
}
