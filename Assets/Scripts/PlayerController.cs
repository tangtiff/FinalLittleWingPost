using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float maxSpeed = 5f;             // Maximum forward speed
    public float maxReverseSpeed = -2.5f;   // Maximum backward speed
    public float acceleration = 2f;         // Forward acceleration rate
    public float brakeDeceleration = 2f;    // Brake deceleration rate (when moving forward)
    public float reverseAcceleration = 1f;  // Backward acceleration rate
    public float turnSpeed = 180f;          // Rotation speed (degrees per second)

    private float speed = 0f;               // Current speed of the character
    private float angle = 0f;               // Current angle of the character (in degrees)

    private void Update()
    {
        HandleMovement();
        MoveCharacter();
    }

    private void HandleMovement()
    {
        // Get input from the player (WASD or Arrow keys)
        float moveInput = 0f;

        // Forward movement (W or Up Arrow)
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            moveInput = 1f;
        }
        // Backward movement (S or Down Arrow)
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            moveInput = -1f;
        }

        // Turn speed based on current speed
        float currentTurnSpeed = turnSpeed * (speed / maxSpeed);
        if (speed < 0f)
        {
            currentTurnSpeed = turnSpeed * (Mathf.Abs(speed) / Mathf.Abs(maxReverseSpeed));
        }

        // Turning logic: Left and Right turns
        if (moveInput >= 0f) // Moving forward
        {
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                angle -= currentTurnSpeed * Time.deltaTime; // Turn left
            }
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                angle += currentTurnSpeed * Time.deltaTime; // Turn right
            }
        }
        else // Moving backward (flips direction)
        {
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                angle += currentTurnSpeed * Time.deltaTime; // Turn right
            }
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                angle -= currentTurnSpeed * Time.deltaTime; // Turn left
            }
        }

        // Movement logic
        if (moveInput > 0f) // Moving forward
        {
            if (speed < maxSpeed)
            {
                speed += acceleration * Time.deltaTime;
            }
        }
        else if (moveInput < 0f) // Moving backward
        {
            // If not already moving forward, move backward immediately
            if (speed <= 0f) // Stop or already reversed
            {
                speed -= reverseAcceleration * Time.deltaTime; // Accelerate backward
            }
            else // If moving forward, brake first
            {
                speed -= brakeDeceleration * Time.deltaTime; // Braking
            }
        }

        // Frictional deceleration on no input
        if (moveInput == 0f)
        {
            if (speed > 0f)
            {
                speed -= brakeDeceleration * Time.deltaTime; // Decelerate forward movement
            }
            else if (speed < 0f)
            {
                speed += brakeDeceleration * Time.deltaTime; // Decelerate reverse movement
            }
        }

        // Clamp speed to max values
        speed = Mathf.Clamp(speed, maxReverseSpeed, maxSpeed);
    }


    private void MoveCharacter()
    {
        Vector3 direction = new Vector3(Mathf.Sin(Mathf.Deg2Rad * angle), 0f, Mathf.Cos(Mathf.Deg2Rad * angle));
        transform.position += direction * speed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0f, angle, 0f);
    }
}
