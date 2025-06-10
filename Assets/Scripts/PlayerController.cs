using System.Collections;
using UnityEditor.EditorTools;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float maxSpeed = 5f;
    public float maxReverseSpeed = -2.5f;
    public float acceleration = 2f;
    public float brakeDeceleration = 2f;
    public float reverseAcceleration = 1f;
    public float turnSpeed = 180f;
    public float tiltAmount = 4f;

    private float speed = 0f;
    private float angle = 0f;
    private float tilt = 0f;
    private Rigidbody rb;

    private PackageController packageController; // Reference to PackageController

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        packageController = FindFirstObjectByType<PackageController>(); // Find PackageController in the scene
    }

    private void Update()
    {
        HandleMovement();
        MoveCharacter();
    }

    private void HandleMovement()
    {
        float moveInput = 0f;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) moveInput = 1f;
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) moveInput = -1f;

        float currentTurnSpeed = turnSpeed * (speed / maxSpeed);
        if (speed < 0f) currentTurnSpeed = turnSpeed * (Mathf.Abs(speed) / Mathf.Abs(maxReverseSpeed));

        if (moveInput >= 0f)
        {
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                angle -= currentTurnSpeed * Time.deltaTime;
                tilt = Mathf.Lerp(tilt, tiltAmount, Time.deltaTime * 5f);
            }
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                angle += currentTurnSpeed * Time.deltaTime;
                tilt = Mathf.Lerp(tilt, -tiltAmount, Time.deltaTime * 5f);
            }
            else
            {
                tilt = Mathf.Lerp(tilt, 0f, Time.deltaTime * 5f);
            }
        }
        else
        {
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                angle += currentTurnSpeed * Time.deltaTime;
                tilt = Mathf.Lerp(tilt, tiltAmount, Time.deltaTime * 5f);
            }
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                angle -= currentTurnSpeed * Time.deltaTime;
                tilt = Mathf.Lerp(tilt, -tiltAmount, Time.deltaTime * 5f);
            }
            else
            {
                tilt = Mathf.Lerp(tilt, 0f, Time.deltaTime * 5f);
            }
        }

        if (moveInput > 0f && speed < maxSpeed) speed += acceleration * Time.deltaTime;
        else if (moveInput < 0f)
        {
            if (speed <= 0f) speed -= reverseAcceleration * Time.deltaTime;
            else speed -= brakeDeceleration * Time.deltaTime;
        }

        if (moveInput == 0f)
        {
            if (speed > 0f) speed -= brakeDeceleration * Time.deltaTime;
            else if (speed < 0f) speed += brakeDeceleration * Time.deltaTime;
        }

        speed = Mathf.Clamp(speed, maxReverseSpeed, maxSpeed);
    }

    private void MoveCharacter()
    {
        Vector3 direction = new Vector3(Mathf.Sin(Mathf.Deg2Rad * angle), 0f, Mathf.Cos(Mathf.Deg2Rad * angle));
        transform.position += direction * speed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0f, angle, 0f);
        transform.localRotation = Quaternion.Euler(0f, angle, tilt);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Delivery"))
        {
            packageController.DeliverPackage(other.GetComponent<Mailbox>());
        }
        else if (other.gameObject.CompareTag("Package")) // Player-Package collision
        {
            packageController.PickupPackage(other.gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the player collided
        if (collision.gameObject.CompareTag("enemy"))
        {
            ApplyKnockback(collision.transform.position, 2.5f, 0.25f);
        }
        else if (collision.gameObject.CompareTag("Solid"))
        {
            speed = 0f;
        }
    }

    private bool isKnockedBack = false;

    public void ApplyKnockback(Vector3 sourcePosition, float distance, float duration)
    {
        if (!isKnockedBack)
        {
            Vector3 direction = (transform.position - sourcePosition).normalized;
            direction.y = 0f;
            StartCoroutine(DoKnockback(direction, distance, duration));
        }
    }

    private IEnumerator DoKnockback(Vector3 direction, float distance, float duration)
    {
        isKnockedBack = true;
        speed = 0f;

        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + direction * distance;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
        isKnockedBack = false;
    }
}
