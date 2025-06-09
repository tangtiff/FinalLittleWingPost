using UnityEngine;

public class Mailbox : MonoBehaviour
{
    [SerializeField] private string mailboxType;
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material highlightMaterial;
    private Renderer mailboxRenderer;
    private bool isHighlighted = false;

    public string MailboxType => mailboxType;

    private void Start()
    {
        // Validate mailbox type
        if (string.IsNullOrEmpty(mailboxType))
        {
            Debug.LogError($"Mailbox on {gameObject.name} has no type assigned!");
        }
        else if (mailboxType.Length != 1 || mailboxType[0] < 'a' || mailboxType[0] > 'e')
        {
            Debug.LogError($"Mailbox on {gameObject.name} has invalid type '{mailboxType}'. Must be a single letter from 'a' to 'e'.");
        }
        else
        {
            Debug.Log($"Mailbox at {gameObject.name} accepts package type: {mailboxType}");
        }

        // Get renderer for visual feedback
        mailboxRenderer = GetComponentInChildren<Renderer>();
        if (mailboxRenderer == null)
        {
            Debug.LogWarning($"Mailbox on {gameObject.name} has no Renderer component for visual feedback!");
        }
        else if (defaultMaterial == null)
        {
            defaultMaterial = mailboxRenderer.material;
        }

        // Ensure mailbox has a collider for delivery
        if (GetComponent<Collider>() == null)
        {
            Debug.LogError($"Mailbox on {gameObject.name} is missing a Collider component!");
        }
        else if (!GetComponent<Collider>().isTrigger)
        {
            Debug.LogWarning($"Mailbox on {gameObject.name} should have a trigger collider for delivery.");
        }
    }

    public void Highlight(bool highlight)
    {
        if (mailboxRenderer != null && highlightMaterial != null && isHighlighted != highlight)
        {
            isHighlighted = highlight;
            mailboxRenderer.material = highlight ? highlightMaterial : defaultMaterial;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Optional: Highlight mailbox when player with matching package is nearby
        if (other.CompareTag("Player"))
        {
            Debug.Log($"Player entered mailbox zone - This mailbox requires package type: {mailboxType}");
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null && player.HasMatchingPackage(mailboxType))
            {
                Highlight(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Remove highlight when player leaves
        if (other.CompareTag("Player"))
        {
            Highlight(false);
        }
    }
}