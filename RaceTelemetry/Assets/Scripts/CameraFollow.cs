using UnityEngine;




public class CameraFollow : MonoBehaviour
{

    private CameraType cameraType = CameraType.Follow;
    public Transform target;    // The target GameObject to follow
    public float height = 100f; // Height of the camera above the target
    public float followSpeed = 5f; // Speed at which the camera follows

    private float movementSpeed = 50f;
    private float lookSpeed = 2f;
    private float sprintMultiplier = 5f;
    private float yaw = 0f;
    private float pitch = 0f;

    private float lastTimeCameraChange = 0f;
    private float cameraTypeChangeCooldown = 0.5f;

    void Update()
    {
        if(Input.GetKey(KeyCode.Space) && Time.time >= lastTimeCameraChange + cameraTypeChangeCooldown){
            cameraType = cameraType == CameraType.Free ? CameraType.Follow : CameraType.Free;
            lastTimeCameraChange = Time.time;
        }

        if(cameraType == CameraType.Free){
            FreeCameraUpdate();
        }
        else{
            FollowCameraUpdate();
        }
    }

    public void FreeCameraUpdate(){
        // --- Mouse look ---
        yaw += lookSpeed * Input.GetAxis("Mouse X");
        pitch -= lookSpeed * Input.GetAxis("Mouse Y");
        pitch = Mathf.Clamp(pitch, -89f, 89f); // Prevent flipping

        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);

        // --- Movement ---
        float speed = movementSpeed;
        if (Input.GetKey(KeyCode.LeftShift)) speed *= sprintMultiplier;

        Vector3 forward = transform.forward;
        Vector3 right = transform.right;
        Vector3 up = transform.up;

        Vector3 move = (Input.GetKey(KeyCode.W) ? forward : Vector3.zero) +
                       (Input.GetKey(KeyCode.S) ? -forward : Vector3.zero) +
                       (Input.GetKey(KeyCode.A) ? -right : Vector3.zero) +
                       (Input.GetKey(KeyCode.D) ? right : Vector3.zero);

        transform.position += move.normalized * speed * Time.deltaTime;
    }


    public void FollowCameraUpdate(){
        // Calculate the desired position
        Vector3 desiredPosition = new Vector3(target.position.x, target.position.y + height, target.position.z);

        // Smoothly interpolate the camera's position towards the desired position
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        // Make the camera look straight down at the target
        transform.rotation = Quaternion.Euler(90f, 0f, 0f); // Directly looking straight down
    }
}