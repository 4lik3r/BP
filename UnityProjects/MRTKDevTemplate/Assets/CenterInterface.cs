using UnityEngine;

public class CenterInterface : MonoBehaviour
{
    public Transform playerCamera; // Reference to the player's camera or head-tracked transform
    public float distanceFromCamera = 1.5f; // Set distance from the camera for the canvas

    void Update()
    {
        // Keep the canvas in front of the player at the specified distance
        Vector3 targetPosition = playerCamera.position + playerCamera.forward * distanceFromCamera;
        transform.position = targetPosition;

        // Make the canvas face the player
        transform.rotation = Quaternion.LookRotation(transform.position - playerCamera.position);
    }
}