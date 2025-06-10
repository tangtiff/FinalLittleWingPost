using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DebugUIManager : MonoBehaviour
{
    [Header("Debug UI Settings")]
    [SerializeField] private GameObject debugPanel;        // Panel containing debug messages
    [SerializeField] private Text debugText;              // Text component for debug messages
    [SerializeField] private float messageDuration = 3f;   // How long messages stay visible
    [SerializeField] private float fadeSpeed = 1f;         // How fast messages fade out
    [SerializeField] private Color panelBackgroundColor = new Color(0, 0, 0, 0.8f); // Semi-transparent black background

    private static DebugUIManager instance;
    private Coroutine currentMessageCoroutine;
    private Color originalColor;
    private Image panelImage;

    private void Awake()
    {
        // Singleton pattern
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Get and set up the panel image
        if (debugPanel != null)
        {
            panelImage = debugPanel.GetComponent<Image>();
            if (panelImage != null)
            {
                panelImage.color = panelBackgroundColor;
            }
            else
            {
                Debug.LogWarning("Debug panel is missing an Image component!");
            }
        }

        // Store original text color
        if (debugText != null)
        {
            originalColor = debugText.color;
        }

        // Ensure debug panel is initially hidden
        if (debugPanel != null)
        {
            debugPanel.SetActive(false);
        }
    }

    public static void ShowDebugMessage(string message)
    {
        if (instance != null)
        {
            instance.DisplayMessage(message);
        }
    }

    private void DisplayMessage(string message)
    {
        if (debugPanel == null || debugText == null) return;

        // Stop any existing message coroutine
        if (currentMessageCoroutine != null)
        {
            StopCoroutine(currentMessageCoroutine);
        }

        // Show the panel and set the message
        debugPanel.SetActive(true);
        debugText.text = message;
        debugText.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);

        // Start fade out coroutine
        currentMessageCoroutine = StartCoroutine(FadeOutMessage());
    }

    private IEnumerator FadeOutMessage()
    {
        // Wait for the message duration
        yield return new WaitForSeconds(messageDuration);

        // Fade out the text
        Color currentColor = debugText.color;
        while (currentColor.a > 0)
        {
            currentColor.a -= fadeSpeed * Time.deltaTime;
            debugText.color = currentColor;
            yield return null;
        }

        // Hide the panel
        debugPanel.SetActive(false);
        currentMessageCoroutine = null;
    }
} 