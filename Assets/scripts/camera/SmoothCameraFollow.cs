using UnityEngine;

public class SmoothCameraFollow : MonoBehaviour
{
    public Transform target;

    [Range(0.001f, 1f)]
    public float smoothSpeed = 0.125f;

    public Vector3 offsetPosition;
    private Vector3 velocity;

    [Range(-15f, -1000f)]
    public float cameraDistance = -80f;

    private void Awake()
    {
        offsetPosition = new Vector3(0, 0, cameraDistance);
        velocity = Vector3.zero;
    }

    void FixedUpdate()
    {
        SmoothCameraPositionToTarget();
    }

    private void SmoothCameraPositionToTarget()
    {
        Vector3 desiredPosition = target.position + offsetPosition;
        Vector3 smoothPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothSpeed);
        transform.position = smoothPosition;
    }
}
