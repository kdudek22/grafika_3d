using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class CameraManager : MonoBehaviour
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

    public static CameraManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        Debug.Log(instance);

    }

    void Update()
    {
        // This is a simple way to switch bettwen the Free and Follow types in the gui
        if (Input.GetKey(KeyCode.Space) && Time.time >= lastTimeCameraChange + cameraTypeChangeCooldown)
        {
            cameraType = cameraType == CameraType.Free ? CameraType.Follow : CameraType.Free;
            lastTimeCameraChange = Time.time;
        }

        if (cameraType == CameraType.Free)
        {
            FreeCameraUpdate();
        }
        else if (cameraType == CameraType.Follow)
        {
            FollowCameraUpdate();
        }
        else if(cameraType == CameraType.LockedInPlace)
        {
            // Nothing really happens here, as there is no handling when the camera is locked in place :)
        }
    }

    public Dictionary<string, object> GetInfoMap()
    {
        return new Dictionary<string, object>(){
            { "x", transform.position.x },
            { "y", transform.position.y },
            { "z", transform.position.z },
            { "rotation_x", transform.rotation.eulerAngles.x },
            { "rotation_y", transform.rotation.eulerAngles.y },
            { "rotation_z", transform.rotation.eulerAngles.z },
            { "cameraType", cameraType }};
    }

    public void FreeCameraUpdate()
    {
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


    public void FollowCameraUpdate()
    {
        // Calculate the desired position
        Vector3 desiredPosition = new Vector3(target.position.x, target.position.y + height, target.position.z);

        // Smoothly interpolate the camera's position towards the desired position
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        // Make the camera look straight down at the target
        transform.rotation = Quaternion.Euler(90f, 0f, 0f); // Directly looking straight down
    }

    public void UpdateCameraType(CameraType newCameraType)
    {
        cameraType = newCameraType;
    }

    public void MoveCameraToPosition(Vector3 location, Vector3 rotation)
    {
        cameraType = CameraType.LockedInPlace;
        StartCoroutine(MoveCameraOverTime(location, rotation, 3f));
    }

    private IEnumerator MoveCameraOverTime(Vector3 targetPosition, Vector3 targetRotation, float duration)
    {
        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = Quaternion.Euler(targetRotation);

        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            transform.rotation = Quaternion.Lerp(startRotation, endRotation, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure final position and rotation are exactly set
        transform.position = targetPosition;
        transform.rotation = endRotation;
    }
}