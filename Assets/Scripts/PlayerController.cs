using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private float moveInput;
    private float turnInput;
    public float acceleration = 15f;
    public float maxSpeed = 10f;
    public float turnSpeed = 100f;
    public float turnSmoothness = 5f; // How quickly turning responds
    public bool velocityBasedTurning = true; // Turn faster when moving faster
    public float minTurnSpeedMultiplier = 0.3f; // Minimum turn speed when stationary
    public float linearDrag = 1f;
    public float angularDrag = 5f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -0.5f, 0);
        
        // Apply drag settings
        rb.linearDamping = linearDrag;
        rb.angularDamping = angularDrag;
    }

    void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();
        turnInput = movementVector.x;
        moveInput = movementVector.y;
    }

    void FixedUpdate()
    {
        HandleMovement();
        HandleTurning();
    }

    void HandleMovement()
    {
        // Forward/backward acceleration
        if (rb.linearVelocity.magnitude < maxSpeed)
        {
            Vector3 forward = transform.forward * moveInput * acceleration;
            rb.AddForce(forward, ForceMode.Acceleration);
        }
    }

    void HandleTurning()
    {
        if (Mathf.Abs(turnInput) > 0.1f)
        {
            // Calculate turn speed modifier based on current velocity
            float speedModifier = 1f;
            if (velocityBasedTurning)
            {
                float currentSpeed = rb.linearVelocity.magnitude;
                speedModifier = Mathf.Lerp(minTurnSpeedMultiplier, 1f, currentSpeed / maxSpeed);
            }
            
            // Calculate desired angular velocity
            float desiredAngularVelocityY = turnInput * turnSpeed * speedModifier * Mathf.Deg2Rad;
            
            // Smoothly interpolate to the desired angular velocity
            Vector3 currentAngularVelocity = rb.angularVelocity;
            float smoothedAngularVelocityY = Mathf.Lerp(
                currentAngularVelocity.y, 
                desiredAngularVelocityY, 
                turnSmoothness * Time.fixedDeltaTime
            );
            
            rb.angularVelocity = new Vector3(
                currentAngularVelocity.x, 
                smoothedAngularVelocityY, 
                currentAngularVelocity.z
            );
        }
        else
        {
            // Gradually stop turning when no input
            Vector3 currentAngularVelocity = rb.angularVelocity;
            rb.angularVelocity = new Vector3(
                currentAngularVelocity.x,
                Mathf.Lerp(currentAngularVelocity.y, 0f, turnSmoothness * Time.fixedDeltaTime),
                currentAngularVelocity.z
            );
        }
    }
}
