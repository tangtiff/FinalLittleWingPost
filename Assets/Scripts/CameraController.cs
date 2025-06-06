using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    public float smoothTime = 0.3f;
    public Vector3 offset; // Initial offset set by scene camera

    private Vector3 currentVelocity = Vector3.zero;

    void Start()
    {
        offset = transform.position - player.transform.position;
    }

    void LateUpdate()
    {
        // Use player's yaw to rotate offset
        Quaternion yawOnlyRotation = Quaternion.Euler(0f, player.transform.eulerAngles.y, 0f);
        Vector3 rotatedOffset = yawOnlyRotation * offset;

        // Desired camera position
        Vector3 desiredPosition = player.transform.position + rotatedOffset;

        // Smooth follow
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, smoothTime);

        // Have camera look at the player
        transform.LookAt(player.transform.position + Vector3.up * 1.5f); // offset upward for better framing
    }
}