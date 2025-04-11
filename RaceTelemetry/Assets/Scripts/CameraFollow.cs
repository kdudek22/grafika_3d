using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;    // The target GameObject to follow
    public float height = 100f;  // Height of the camera above the target

    private void Update()
    {
        if (target != null)
        {
            // Position the camera directly above the target at the specified height
            transform.position = new Vector3(target.position.x, target.position.y + height, target.position.z);

            // Make the camera look straight down at the target
            transform.rotation = Quaternion.Euler(90f, 0f, 0f);  // Directly looking straight down
        }
    }
}