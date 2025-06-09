using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    public float maxSpeed = 5f;             // Maximum forward speed
    public float maxReverseSpeed = -2.5f;   // Maximum backward speed
    public float acceleration = 2f;         // Forward acceleration rate
    public float brakeDeceleration = 2f;    // Brake deceleration rate (when moving forward)
    public float reverseAcceleration = 1f;  // Backward acceleration rate
    public float turnSpeed = 180f;          // Rotation speed (degrees per second)
    public float tiltAmount = 4f;           // Maximum tilt angle when turning

    private float speed = 0f;               // Current speed of character
    private float angle = 0f;               // Current angle of character (in degrees)
    private float tilt = 0f;                // Current tilt of character (in degrees)
    private Rigidbody rb;                   // Reference to the player's Rigidbody

    private void Start()
    {
        rb = GetComponent<Rigidbody>(); // Initialize Rigidbody
        materialMap = new Dictionary<string, Material>
        {
            {"a", material1 },
            {"b", material2 },
            {"c", material3 },
            {"d", material4 },
            {"e", material5 }
        };
    }

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
                tilt = Mathf.Lerp(tilt, tiltAmount, Time.deltaTime * 5f);
            }
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                angle += currentTurnSpeed * Time.deltaTime; // Turn right
                tilt = Mathf.Lerp(tilt, -tiltAmount, Time.deltaTime * 5f);
            }
            else
            {
                tilt = Mathf.Lerp(tilt, 0f, Time.deltaTime * 5f);
            }
        }
        else // Moving backward (flips direction)
        {
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                angle += currentTurnSpeed * Time.deltaTime; // Turn right
                tilt = Mathf.Lerp(tilt, tiltAmount, Time.deltaTime * 5f);
            }
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                angle -= currentTurnSpeed * Time.deltaTime; // Turn left
                tilt = Mathf.Lerp(tilt, -tiltAmount, Time.deltaTime * 5f);
            }
            else
            {
                tilt = Mathf.Lerp(tilt, 0f, Time.deltaTime * 5f);
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
        transform.localRotation = Quaternion.Euler(0f, angle, tilt);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Package"))
        {
            PickupPackage(other.gameObject);
        }
    }

    public GameObject carriedPrefab;
    public Transform carryBasePoint;
    public float verticalOffset;
    private List<GameObject> carriedPackages = new List<GameObject>();
    public Material material1;
    public Material material2;
    public Material material3;
    public Material material4;
    public Material material5;
    private Dictionary<string, Material> materialMap;

    private void PickupPackage(GameObject worldPackage)
    {
        worldPackage.SetActive(false); // Set world package to invisible

        string type = worldPackage.GetComponent<Package>().packageType; // Get package type

        // Instantiate carried package and set position on vespa
        GameObject carried = Instantiate(carriedPrefab);
        carried.transform.SetParent(carryBasePoint);
        carried.transform.localPosition = new Vector3(0, verticalOffset * carriedPackages.Count, 0);
        carried.transform.localRotation = Quaternion.identity;

        // Assign packageType and material
        Package carriedScript = carried.GetComponent<Package>();
        carriedScript.packageType = type; // Set packageType field on carried package to match the one from world package
        if (materialMap.TryGetValue(type, out Material mat))
        {
            Renderer rend = carried.GetComponentInChildren<Renderer>();
            rend.material = mat;
        }

        carriedPackages.Add(carried); // Track carried package
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
