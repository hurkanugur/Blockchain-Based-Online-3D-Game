using System;
using UnityEngine;

public class PortalCamera : MonoBehaviour
{
    public Camera Camera_A, Camera_B;
    public Material cameraMatA, cameraMatB;
    public Transform thisPortal, otherPortal;
    private Transform playerCameraTransform = null;

    public void Start()
    {
        if (Camera_A.targetTexture != null)
            Camera_A.targetTexture.Release();
        if (Camera_B.targetTexture != null)
            Camera_B.targetTexture.Release();
        Camera_A.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
        Camera_B.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
        cameraMatA.mainTexture = Camera_A.targetTexture;
        cameraMatB.mainTexture = Camera_B.targetTexture;
    }


    public void LateUpdate()
    {
        if (playerCameraTransform == null)
        {
            try { playerCameraTransform = CameraController.instance.mainPlayerCamera.gameObject.transform; } catch (Exception) { }
        }
        else
        {
            Vector3 playerOffsetFromPortal = playerCameraTransform.position - thisPortal.position;
            transform.position = otherPortal.position + playerOffsetFromPortal;
            float angularDifferenceBetweenPortalRotations = Quaternion.Angle(otherPortal.rotation, thisPortal.rotation);
            Quaternion portalRotationalDifference = Quaternion.AngleAxis(angularDifferenceBetweenPortalRotations, -playerCameraTransform.forward);
            Vector3 newCameraDirection = portalRotationalDifference * playerCameraTransform.forward;
            transform.rotation = Quaternion.LookRotation(newCameraDirection, Vector3.up);
        }
    }
}
