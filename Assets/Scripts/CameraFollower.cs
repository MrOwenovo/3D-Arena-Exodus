using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    private float distance = 17.0f;    // Distance from the camera to the target
    private float height = 17.0f;      // Height of the camera from the target
    private float rotationSpeed = 50.0f;  // Speed at which the camera rotates

    private Vector3 currentVelocity; // For smooth damping
    private float smoothTime = 0.3f; // Smooth time for dampening

    private float currentAngle = 0.0f; // To keep track of current angle for smooth rotation

    private Vector3 centerPosition; // Position of the center block or default position



    void Update()
    {
        if ((GameManager.instance.curStatus == Status.Game||GameManager.instance.curStatus == Status.Training))
        {
            LocateCenterBlock();
            HandleCameraPositionAndRotation();
        }
    }

    void LocateCenterBlock()
    {
        GameObject centerBlock = GameObject.Find("MapBlock_20_1_20");
        if (centerBlock != null)
        {
            centerPosition = centerBlock.transform.position;
        }
        else
        {
            Debug.LogError("Center block 'MapBlock_20_1_20' not found. Using a default position.");
            centerPosition = new Vector3(20, 1, 20); // Default if center block not found
        }
    }

    void HandleCameraPositionAndRotation()
    {
        currentAngle += (Input.GetKey(KeyCode.Q) ? -1 : (Input.GetKey(KeyCode.E) ? 1 : 0)) * rotationSpeed * Time.deltaTime;
        Quaternion rotation = Quaternion.Euler(0f, currentAngle, 0f);
        Vector3 direction = rotation * -Vector3.forward;

        Vector3 targetPosition = centerPosition + direction * distance + Vector3.up * height;
        
        // Smoothly move the camera to the target position
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothTime);

        // Ensure the camera is always looking at the center position
        transform.LookAt(centerPosition);
    }
}
