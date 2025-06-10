using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PackageController : MonoBehaviour
{
    private int totalPackages = 5;                                          // Number of packages to spawn (default 5)
    public List<Transform> spawnLocations = new List<Transform>();          // List of possible spawn locations for packages
    public List<GameObject> allMailboxes = new List<GameObject>();          // List of all mailbox objects (> totalPackages)
    public List<GameObject> packageAssets = new List<GameObject>();         // List of package assets to spawn (length = totalPackages)

    private List<GameObject> carriedPackages = new List<GameObject>();      // Currently carried packages
    public GameObject packagePrefab;                                        // Reference to the package prefab (used on pickup)
    public Transform carryBasePoint;                                        // Stack starting point on vespa
    public float verticalOffset;                                            // Space between stacked packages

    public float bobbingHeight;                                             // Height of package bobbing animation
    public float bobbingSpeed;                                              // Speed of package bobbing animation
    public float rotationSpeed;                                             // Rotation speed of package animation

    private Dictionary<string, Material> materialMap;                       // Dictionary matching package material to tag
    public Material material1, material2, material3, material4, material5;  // Materials for each package type
    private Dictionary<string, Material> pipeMatMap;                        // Dictionary matching pipe material to tag
    public Material pipeMat1, pipeMat2, pipeMat3, pipeMat4, pipeMat5;       // Materials for each pipe type

    public float stealCooldown = 8f;
    private bool canSteal = true;

    void Start()
    {
        materialMap = new Dictionary<string, Material>
        {
            { "a", material1 },
            { "b", material2 },
            { "c", material3 },
            { "d", material4 },
            { "e", material5 }
        };
        pipeMatMap = new Dictionary<string, Material>
        {
            { "a", pipeMat1 },
            { "b", pipeMat2 },
            { "c", pipeMat3 },
            { "d", pipeMat4 },
            { "e", pipeMat5 }
        };
        AssignMailboxes();  // Assigns mailboxes to receive packages
        SpawnPackages();  // Randomly spawn packages
    }

    // Randomly activates and assigns mailboxes to tags (starting from 'a')
    void AssignMailboxes()
    {
        // Check if there are enough mailboxes
        if (allMailboxes.Count < totalPackages)
        {
            Debug.LogError("There should be at least 1 goal for each package (total " + totalPackages + " packages), found " + allMailboxes.Count + " goals.");
            return;
        }

        // Disable all mailboxes
        foreach (var mailbox in allMailboxes)
        {
            Transform marker = mailbox.transform.Find("Pipe");
            if (marker != null)
            {
                marker.gameObject.SetActive(false);  // Disables light ring around mailbox
            }
            mailbox.tag = "NonDelivery";  // Tag mailbox as not-for-delivery (i.e. deactivated)
        }

        // Select random mailboxes to activate
        List<GameObject> selectedBoxes = SelectRandomMailboxes(totalPackages);

        // Assign tags starting from 'a' to the selected mailboxes
        char tag = 'a';
        foreach (var mailbox in selectedBoxes)
        {
            mailbox.GetComponent<Mailbox>().mailboxType = tag.ToString();
            mailbox.tag = "Delivery";  // Tag mailbox as for-delivery (i.e. activated)

            // Enable the light ring (visual marker) for the selected goal
            Transform marker = mailbox.transform.Find("Pipe");
            if (marker != null)
            {
                marker.gameObject.SetActive(true);  // Enable visual marker for selected goals

                // Color the pipe using the material for this tag
                if (pipeMatMap.TryGetValue(tag.ToString(), out Material pipeMat))
                {
                    Renderer pipeRenderer = marker.GetComponent<Renderer>();
                    if (pipeRenderer != null)
                    {
                        pipeRenderer.material = pipeMat;
                    }
                }
            }

            tag++;  // Increment tag
        }
    }

    // Randomly select a number of mailboxes from the available pool
    List<GameObject> SelectRandomMailboxes(int count)
    {
        List<GameObject> selectedBoxes = new List<GameObject>();
        HashSet<int> selectedIndices = new HashSet<int>();

        while (selectedBoxes.Count < count)
        {
            int randomIndex = Random.Range(0, allMailboxes.Count);
            if (!selectedIndices.Contains(randomIndex))
            {
                selectedBoxes.Add(allMailboxes[randomIndex]);
                selectedIndices.Add(randomIndex);
            }
        }

        return selectedBoxes;
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

        // Ensure correct number of package assets
        if (packageAssets.Count != totalPackages)
        {
            Debug.LogError("There must be exactly " + totalPackages + " package assets.");
            return;
        }

        // Initiate location of packages
        for (int i = 0; i < selectedLocations.Count; i++)
        {
            Transform spawnLocation = selectedLocations[i];

            GameObject package = packageAssets[i];
            package.transform.position = spawnLocation.position;
            package.transform.rotation = Quaternion.identity;

            // Set the package as a child of the spawn location
            package.transform.SetParent(spawnLocation);

            // Start bobbing and rotating animation for package
            StartCoroutine(AnimatePackage(package));
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

    // Make packages bob up and down and rotate
    IEnumerator AnimatePackage(GameObject package)
    {
        Vector3 startPos = package.transform.position;
        float timeElapsed = 0f;

        while (package != null) // While the package is still in the scene
        {
            timeElapsed += Time.deltaTime;

            // Bobbing effect
            float yOffset = Mathf.Sin(timeElapsed * bobbingSpeed) * bobbingHeight;

            // Slow rotation effect
            float rotation = timeElapsed * rotationSpeed;

            // Apply transformations
            package.transform.position = new Vector3(startPos.x, startPos.y + yOffset, startPos.z);
            package.transform.rotation = Quaternion.Euler(0f, rotation, 0f);

            yield return null; // Wait until the next frame
        }
    }

    // Picks up package and places package on player's vehicle
    public void PickupPackage(GameObject package)
    {
        package.SetActive(false); // Set world package to invisible

        string type = package.GetComponent<Package>().packageType; // Get package type

        // Show debug message
        DebugUIManager.ShowDebugMessage($"Picked up package type: {type}");

        // Instantiate a new carried package and set position on vehicle
        GameObject carried = Instantiate(packagePrefab);
        carried.transform.SetParent(carryBasePoint);
        carried.transform.localPosition = new Vector3(0, verticalOffset * carriedPackages.Count, 0);
        carried.transform.localRotation = Quaternion.identity;

        // Assign packageType and material to carried package
        Package carriedScript = carried.GetComponent<Package>();
        carriedScript.packageType = type;
        if (materialMap.TryGetValue(type, out Material mat))
        {
            Renderer rend = carried.GetComponentInChildren<Renderer>();
            rend.material = mat;
        }

        carriedPackages.Add(carried);
    }

    // Deliver a package to a mailbox
    public void DeliverPackage(Mailbox mailbox)
    {
        if (carriedPackages.Count == 0)
        {
            DebugUIManager.ShowDebugMessage("No packages to deliver!");
            return;
        }

        bool deliveryAttempted = false;
        string carriedTypes = "";
        for (int i = 0; i < carriedPackages.Count; i++)
        {
            GameObject package = carriedPackages[i];
            string type = package.GetComponent<Package>().packageType;
            carriedTypes += (i > 0 ? ", " : "") + type;

            if (type == mailbox.mailboxType) // Valid delivery
            {
                deliveryAttempted = true;
                GameObject deliveryGoal = mailbox.gameObject;
                deliveryGoal.tag = "NonDelivery";  // Change tag to 'NonDelivery' (i.e. deactivated)

                // Hide the visual marker for goal
                Transform marker = deliveryGoal.transform.Find("Pipe");
                if (marker != null)
                {
                    marker.gameObject.SetActive(false); // Hide the visual marker for this mailbox
                }

                Destroy(package); // Destroy the package
                carriedPackages.RemoveAt(i); // Remove from carried list

                // Update UI
                GameController gameController = FindFirstObjectByType<GameController>();
                if (gameController != null)
                {
                    gameController.PackageDelivered();
                }
                else
                {
                    Debug.LogError("GameController not found in the scene!");
                }

                // Shift down remaining packages on vehicle
                for (int j = i; j < carriedPackages.Count; j++)
                {
                    Vector3 pos = carriedPackages[j].transform.localPosition;
                    carriedPackages[j].transform.localPosition = new Vector3(pos.x, pos.y - verticalOffset, pos.z);
                }
                DebugUIManager.ShowDebugMessage($"Successfully delivered package of type {type}!");
                return;
            }
        }

        if (!deliveryAttempted)
        {
            DebugUIManager.ShowDebugMessage($"This mailbox needs type {mailbox.mailboxType}! You have: {carriedTypes}");
        }
    }

    public void StealPackage()
    {
        if (!canSteal)
        {
            Debug.Log("Steal is on cooldown");
            return;
        }
        if (carriedPackages.Count == 0)
        {
            Debug.Log("No packages to steal");
            return;
        }

        // cooldown for package stealing
        StartCoroutine(StealCooldownRoutine());

        // steals a random package
        int randomIndex = Random.Range(0, carriedPackages.Count);
        GameObject stolen = carriedPackages[randomIndex];

        // get type and remove from the list
        string type = stolen.GetComponent<Package>().packageType;
        carriedPackages.RemoveAt(randomIndex);
        Destroy(stolen);

        // restack carried packages
        for (int i = 0; i < carriedPackages.Count; i++)
        {
            carriedPackages[i].transform.localPosition = new Vector3(0, verticalOffset * i, 0);
        }

        // respawn the packages
        Transform respawnLocation = spawnLocations[Random.Range(0, spawnLocations.Count)];
        GameObject respawnAsset = packageAssets.Find(p => p.GetComponent<Package>().packageType == type);
        if (respawnAsset != null)
        {
            GameObject newPackage = Instantiate(respawnAsset, respawnLocation.position, Quaternion.identity);
            newPackage.transform.SetParent(respawnLocation);
            StartCoroutine(AnimatePackage(newPackage));
            Debug.Log($"Stolen package of type '{type}' respawned.");
        }
        else
        {
            Debug.LogWarning($"No prefab found to respawn package of type '{type}'!");
        }
    }

    private IEnumerator StealCooldownRoutine()
    {
        canSteal = false;
        yield return new WaitForSeconds(stealCooldown);
        canSteal = true;
    }


}
