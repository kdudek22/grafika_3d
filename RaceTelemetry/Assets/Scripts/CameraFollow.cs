using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;    // The target GameObject to follow
    public float height = 100f; // Height of the camera above the target
    public float followSpeed = 5f; // Speed at which the camera follows

    private void LateUpdate()
    {
        if (target != null)
        {
            // Calculate the desired position
            Vector3 desiredPosition = new Vector3(target.position.x, target.position.y + height, target.position.z);

            // Smoothly interpolate the camera's position towards the desired position
            transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

            // Make the camera look straight down at the target
            transform.rotation = Quaternion.Euler(90f, 0f, 0f); // Directly looking straight down
        }
    }
}