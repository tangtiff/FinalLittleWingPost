using UnityEngine;

public class Package : MonoBehaviour
{
    [SerializeField] private string packageType;
    private bool isCarried = false;

    public string PackageType => packageType;

    public void SetPackageType(string type)
    {
        if (string.IsNullOrEmpty(type))
        {
            Debug.LogError($"Cannot set empty package type on {gameObject.name}!");
            return;
        }
        if (type.Length != 1 || type[0] < 'a' || type[0] > 'e')
        {
            Debug.LogError($"Cannot set invalid package type '{type}' on {gameObject.name}. Must be a single letter from 'a' to 'e'.");
            return;
        }
        packageType = type;
    }

    private void Start()
    {
        // Validate package type
        if (string.IsNullOrEmpty(packageType))
        {
            Debug.LogError($"Package on {gameObject.name} has no type assigned!");
        }
        else if (packageType.Length != 1 || packageType[0] < 'a' || packageType[0] > 'e')
        {
            Debug.LogError($"Package on {gameObject.name} has invalid type '{packageType}'. Must be a single letter from 'a' to 'e'.");
        }

        // Ensure package has a collider for pickup
        if (GetComponent<Collider>() == null)
        {
            Debug.LogError($"Package on {gameObject.name} is missing a Collider component!");
        }
        else if (!GetComponent<Collider>().isTrigger)
        {
            Debug.LogWarning($"Package on {gameObject.name} should have a trigger collider for pickup.");
        }
    }

    public void SetCarried(bool carried)
    {
        isCarried = carried;
        // Print package type when picked up
        if (carried)
        {
            Debug.Log($"Picked up package type: {packageType}");
        }
        // Optionally disable collider while carried to prevent re-pickup
        if (GetComponent<Collider>() != null)
        {
            GetComponent<Collider>().enabled = !carried;
        }
    }

    public bool IsCarried() => isCarried;
}