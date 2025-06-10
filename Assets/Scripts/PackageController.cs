using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PackageController : MonoBehaviour
{
    public List<Transform> spawnLocations = new List<Transform>(); // List of possible spawn locations for packages
    public GameObject packagePrefab;                           // Reference to the package prefab to spawn
    public Transform carryBasePoint;                                   // Stack starting point on vespa
    public float verticalOffset = 1f;                           // Space between stacked packages (if needed)
    private Dictionary<string, Material> materialMap;           // { packageType, material }
    public Material material1, material2, material3, material4, material5; // Materials for each package type

    private List<GameObject> spawnedPackages = new List<GameObject>(); // Track all spawned packages
    private List<GameObject> carriedPackages = new List<GameObject>(); // Track currently carried packages
    private int totalPackages = 5;  // Number of packages to spawn (5 packages in this example)

    public PlayerController playerController;  // Reference to the PlayerController

    void Start()
    {
        materialMap = new Dictionary<string, Material>
        {
            {"a", material1 },
            {"b", material2 },
            {"c", material3 },
            {"d", material4 },
            {"e", material5 }
        };

        // Randomly spawn the packages at the spawn locations
        SpawnPackages();
    }

    // Spawn packages at random locations
    void SpawnPackages()
    {
        if (spawnLocations.Count == 0)
        {
            Debug.LogError("No spawn locations specified!");
            return;
        }

        // Select random spawn locations
        List<Transform> selectedLocations = SelectRandomLocations(totalPackages);

        // Assign tags and materials to the spawned packages
        char tag = 'a';
        for (int i = 0; i < selectedLocations.Count; i++)
        {
            Transform spawnLocation = selectedLocations[i];
            GameObject package = Instantiate(packagePrefab, spawnLocation.position, Quaternion.identity);
            package.transform.SetParent(spawnLocation); // Optional: make it a child of the spawn location

            // Assign package tag
            package.tag = tag.ToString();  // Tag 'a' to 'e'
            tag++;

            // Initialize package with its package type and material
            Package packageScript = package.GetComponent<Package>();
            if (packageScript != null)
            {
                string packageType = package.tag.ToLower();  // Use tag to set package type
                packageScript.packageType = packageType;

                // Assign material based on package type
                if (materialMap.TryGetValue(packageType, out Material material))
                {
                    Renderer rend = package.GetComponentInChildren<Renderer>();
                    rend.material = material;
                }
            }

            // Track the spawned package
            spawnedPackages.Add(package);
        }
    }

    // Randomly select a number of spawn locations from the available pool
    List<Transform> SelectRandomLocations(int count)
    {
        List<Transform> selectedLocations = new List<Transform>();
        HashSet<int> selectedIndices = new HashSet<int>();

        while (selectedLocations.Count < count)
        {
            int randomIndex = Random.Range(0, spawnLocations.Count);
            if (!selectedIndices.Contains(randomIndex))
            {
                selectedLocations.Add(spawnLocations[randomIndex]);
                selectedIndices.Add(randomIndex);
            }
        }

        return selectedLocations;
    }

    public void PickupPackage(GameObject worldPackage)
    {
        worldPackage.SetActive(false); // Set world package to invisible

        string type = worldPackage.GetComponent<Package>().packageType; // Get package type from world package

        // Instantiate carried package and set position on vespa
        GameObject carried = Instantiate(packagePrefab);
        carried.transform.SetParent(carryBasePoint);
        carried.transform.localPosition = new Vector3(0, verticalOffset * carriedPackages.Count, 0);
        carried.transform.localRotation = Quaternion.identity;

        // Assign packageType and material to carried package
        Package carriedScript = carried.GetComponent<Package>();
        carriedScript.packageType = type; // Set packageType field on carried package to match the one from world package
        if (materialMap.TryGetValue(type, out Material mat))
        {
            Renderer rend = carried.GetComponentInChildren<Renderer>();
            rend.material = mat;
        }

        carriedPackages.Add(carried); // Track carried package

        // Debug.Log($"Picked up a package, type: {type}. Stack size: {carriedPackages.Count}");
    }

    // Deliver a package to a mailbox
    public void DeliverPackage(Mailbox mailbox)
    {
        if (carriedPackages.Count == 0) return;

        for (int i = 0; i < carriedPackages.Count; i++)
        {
            GameObject package = carriedPackages[i];
            string type = package.GetComponent<Package>().packageType;

            if (type == mailbox.mailboxType) // Valid delivery
            {
                Debug.Log($"Delivered package (type: {type}) to mailbox (type: {mailbox.mailboxType})!");
                Destroy(package); // Destroy the package
                carriedPackages.RemoveAt(i); // Remove from carried list

                // Shift down remaining packages
                for (int j = i; j < carriedPackages.Count; j++)
                {
                    Vector3 pos = carriedPackages[j].transform.localPosition;
                    carriedPackages[j].transform.localPosition = new Vector3(pos.x, pos.y - verticalOffset, pos.z);
                }
                return;
            }
        }

        Debug.Log("No matching package to deliver.");
    }

    // Call this when the game ends or the player respawns packages
    public void RespawnPackages()
    {
        foreach (var package in spawnedPackages)
        {
            Destroy(package); // Destroy all current packages
        }

        spawnedPackages.Clear(); // Clear the list

        // Re-spawn packages
        SpawnPackages();
    }

    // You may want to add additional logic for when the player needs to respawn, restart the game, etc.
}




//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;

//public class PlayerController : MonoBehaviour
//{
//    public float maxSpeed = 5f;             // Maximum forward speed
//    public float maxReverseSpeed = -2.5f;   // Maximum backward speed
//    public float acceleration = 2f;         // Forward acceleration rate
//    public float brakeDeceleration = 2f;    // Brake deceleration rate (when moving forward)
//    public float reverseAcceleration = 1f;  // Backward acceleration rate
//    public float turnSpeed = 180f;          // Rotation speed (degrees per second)
//    public float tiltAmount = 4f;           // Maximum tilt angle when turning

//    private float speed = 0f;               // Current speed of character
//    private float angle = 0f;               // Current angle of character (in degrees)
//    private float tilt = 0f;                // Current tilt of character (in degrees)
//    private Rigidbody rb;                   // Reference to the player's Rigidbody

//    private void Start()
//    {
//        rb = GetComponent<Rigidbody>();
//        materialMap = new Dictionary<string, Material>
//        {
//            {"a", material1 },
//            {"b", material2 },
//            {"c", material3 },
//            {"d", material4 },
//            {"e", material5 }
//        };
//    }

//    private void Update()
//    {
//        HandleMovement();
//        MoveCharacter();
//    }

//    private void HandleMovement()
//    {
//        // Get input from the player (WASD or Arrow keys)
//        float moveInput = 0f;

//        // Forward movement (W or Up Arrow)
//        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
//        {
//            moveInput = 1f;
//        }
//        // Backward movement (S or Down Arrow)
//        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
//        {
//            moveInput = -1f;
//        }

//        // Turn speed based on current speed
//        float currentTurnSpeed = turnSpeed * (speed / maxSpeed);
//        if (speed < 0f)
//        {
//            currentTurnSpeed = turnSpeed * (Mathf.Abs(speed) / Mathf.Abs(maxReverseSpeed));
//        }

//        // Turning logic: Left and Right turns
//        if (moveInput >= 0f) // Moving forward
//        {
//            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
//            {
//                angle -= currentTurnSpeed * Time.deltaTime; // Turn left
//                tilt = Mathf.Lerp(tilt, tiltAmount, Time.deltaTime * 5f);
//            }
//            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
//            {
//                angle += currentTurnSpeed * Time.deltaTime; // Turn right
//                tilt = Mathf.Lerp(tilt, -tiltAmount, Time.deltaTime * 5f);
//            }
//            else
//            {
//                tilt = Mathf.Lerp(tilt, 0f, Time.deltaTime * 5f);
//            }
//        }
//        else // Moving backward (flips direction)
//        {
//            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
//            {
//                angle += currentTurnSpeed * Time.deltaTime; // Turn right
//                tilt = Mathf.Lerp(tilt, tiltAmount, Time.deltaTime * 5f);
//            }
//            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
//            {
//                angle -= currentTurnSpeed * Time.deltaTime; // Turn left
//                tilt = Mathf.Lerp(tilt, -tiltAmount, Time.deltaTime * 5f);
//            }
//            else
//            {
//                tilt = Mathf.Lerp(tilt, 0f, Time.deltaTime * 5f);
//            }
//        }

//        // Movement logic
//        if (moveInput > 0f) // Moving forward
//        {
//            if (speed < maxSpeed)
//            {
//                speed += acceleration * Time.deltaTime;
//            }
//        }
//        else if (moveInput < 0f) // Moving backward
//        {
//            // If not already moving forward, move backward immediately
//            if (speed <= 0f) // Stop or already reversed
//            {
//                speed -= reverseAcceleration * Time.deltaTime; // Accelerate backward
//            }
//            else // If moving forward, brake first
//            {
//                speed -= brakeDeceleration * Time.deltaTime; // Braking
//            }
//        }

//        // Frictional deceleration on no input
//        if (moveInput == 0f)
//        {
//            if (speed > 0f)
//            {
//                speed -= brakeDeceleration * Time.deltaTime; // Decelerate forward movement
//            }
//            else if (speed < 0f)
//            {
//                speed += brakeDeceleration * Time.deltaTime; // Decelerate reverse movement
//            }
//        }

//        // Clamp speed to max values
//        speed = Mathf.Clamp(speed, maxReverseSpeed, maxSpeed);
//    }

//    private void MoveCharacter()
//    {
//        Vector3 direction = new Vector3(Mathf.Sin(Mathf.Deg2Rad * angle), 0f, Mathf.Cos(Mathf.Deg2Rad * angle));
//        transform.position += direction * speed * Time.deltaTime;
//        transform.rotation = Quaternion.Euler(0f, angle, 0f);
//        transform.localRotation = Quaternion.Euler(0f, angle, tilt);
//    }

//    private void OnTriggerEnter(Collider other)
//    {
//        if (other.gameObject.CompareTag("Package")) // Player-Package collision
//        {
//            PickupPackage(other.gameObject);
//        }
//        if (other.gameObject.CompareTag("Delivery")) // Player-DeliveryMailbox collision
//        {
//            DeliverPackage(other.GetComponent<Mailbox>());
//        }
//    }

//    public GameObject carriedPrefab;
//    public Transform carryBasePoint;                                   // Stack starting point on vespa
//    public float verticalOffset;                                       // Space between stacked packages
//    private List<GameObject> carriedPackages = new List<GameObject>(); // Current carried packages
//    private Dictionary<string, Material> materialMap;                  // { packageType, material }
//    public Material material1;                                         // Assigned to packageType "a"
//    public Material material2;                                         // Assigned to packageType "b"
//    public Material material3;                                         // Assigned to packageType "c"
//    public Material material4;                                         // Assigned to packageType "d"
//    public Material material5;                                         // Assigned to packageType "e"

//    private void PickupPackage(GameObject worldPackage)
//    {
//        worldPackage.SetActive(false); // Set world package to invisible

//        string type = worldPackage.GetComponent<Package>().packageType; // Get package type from world package

//        // Instantiate carried package and set position on vespa
//        GameObject carried = Instantiate(carriedPrefab);
//        carried.transform.SetParent(carryBasePoint);
//        carried.transform.localPosition = new Vector3(0, verticalOffset * carriedPackages.Count, 0);
//        carried.transform.localRotation = Quaternion.identity;

//        // Assign packageType and material to carried package
//        Package carriedScript = carried.GetComponent<Package>();
//        carriedScript.packageType = type; // Set packageType field on carried package to match the one from world package
//        if (materialMap.TryGetValue(type, out Material mat))
//        {
//            Renderer rend = carried.GetComponentInChildren<Renderer>();
//            rend.material = mat;
//        }

//        carriedPackages.Add(carried); // Track carried package

//        // Debug.Log($"Picked up a package, type: {type}. Stack size: {carriedPackages.Count}");
//    }

//    private void DeliverPackage(Mailbox mailbox)
//    {
//        if (carriedPackages.Count == 0) return;
//        // Try each package
//        for (int i = 0; i < carriedPackages.Count; i++)
//        {
//            GameObject package = carriedPackages[i];
//            string type = package.GetComponent<Package>().packageType;

//            if (type == mailbox.mailboxType) // Valid delivery
//            {
//                Debug.Log($"Delivered package (type: {type}) to mailbox (type: {mailbox.mailboxType})!");
//                Destroy(package); // Destroy package
//                carriedPackages.RemoveAt(i); // Remove from list
//                // Shift down remaining packages
//                for (int j = i; j < carriedPackages.Count; j++)
//                {
//                    Vector3 pos = carriedPackages[j].transform.localPosition;
//                    carriedPackages[j].transform.localPosition = new Vector3(pos.x, pos.y - verticalOffset, pos.z);
//                }
//                return; // Exit after first valid delivery
//            }
//        }

//        // No valid delivery
//        Debug.Log("No matching package to deliver.");
//    }

//    private void OnCollisionEnter(Collision collision)
//    {
//        // Check if the player collided
//        if (collision.gameObject.CompareTag("enemy"))
//        {
//            ApplyKnockback(collision.transform.position, 2.5f, 0.25f);
//        }
//        else if (collision.gameObject.CompareTag("Solid"))
//        {
//            speed = 0f;
//        }
//    }

//    private bool isKnockedBack = false;

//    public void ApplyKnockback(Vector3 sourcePosition, float distance, float duration)
//    {
//        if (!isKnockedBack)
//        {
//            Vector3 direction = (transform.position - sourcePosition).normalized;
//            direction.y = 0f;
//            StartCoroutine(DoKnockback(direction, distance, duration));
//        }
//    }

//    private IEnumerator DoKnockback(Vector3 direction, float distance, float duration)
//    {
//        isKnockedBack = true;
//        speed = 0f;

//        Vector3 startPos = transform.position;
//        Vector3 targetPos = startPos + direction * distance;
//        float elapsed = 0f;

//        while (elapsed < duration)
//        {
//            transform.position = Vector3.Lerp(startPos, targetPos, elapsed / duration);
//            elapsed += Time.deltaTime;
//            yield return null;
//        }

//        transform.position = targetPos;
//        isKnockedBack = false;
//    }
//}