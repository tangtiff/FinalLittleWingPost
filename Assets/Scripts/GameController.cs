using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public float skySpeed;

    [Header("Timer Settings")]
    [SerializeField] private float gameDuration = 180f; // Game duration in seconds
    [SerializeField] private Text timerText; // UI Text element for timer
    private float timeRemaining;
    private bool isGameActive = true;
    private bool isPaused = false;

    [Header("Score and Progress Settings")]
    [SerializeField] private Text progressText; // UI Text element for progress
    [SerializeField] private int packagesDelivered = 0;
    [SerializeField] private int totalPackages = 0;

    [Header("UI Elements")]
    [SerializeField] private Button pauseButton;
    [SerializeField] private GameObject pausePanel; // Pause panel UI
    [SerializeField] private GameObject endPanel;   // End panel UI
    [SerializeField] private Button resumeButton;   // Resume button on pause panel

    [Header("End Panel UI")]
    [SerializeField] private Text titleText;
    [SerializeField] private Text timerEndText;
    [SerializeField] private Button pausedRestartButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button menuButton;

    [Header("Stars")]
    [SerializeField] private Image[] starImages = new Image[5];
    [SerializeField] private Color filledStarColor = Color.yellow;
    [SerializeField] private Color unfilledStarColor = Color.gray;

    [Header("Messages")]
    [SerializeField] private Color successColor = Color.green;
    [SerializeField] private Color timeUpColor = Color.red;

    void Start()
    {
        timeRemaining = gameDuration;
        UpdateTimerDisplay();
        UpdateProgressDisplay();

        // Setup the pause and resume buttons
        if (resumeButton != null)
        {
            resumeButton.onClick.AddListener(ResumeGame);
        }
        if (pauseButton != null)
        {
            pauseButton.onClick.AddListener(PauseGame);
        }

        // Set up the restart and menu buttons on the end panel
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }
        if (pausedRestartButton != null)
        {
            pausedRestartButton.onClick.AddListener(RestartGame);
        }
        if (menuButton != null)
        {
            menuButton.onClick.AddListener(ReturnToMainMenu);
        }

        // Initially, hide the pause and end panels
        pausePanel.SetActive(false);
        endPanel.SetActive(false);
    }

    void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * skySpeed);

        // Toggle pause when Escape key is pressed
        if (Input.GetKeyDown(KeyCode.Escape) && isGameActive)
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }

        // If the game is active, update the timer and check for game end conditions
        if (isGameActive && !isPaused)
        {
            // Update timer if game is active and not paused
            timeRemaining -= Time.deltaTime;
            UpdateTimerDisplay();

            if (timeRemaining <= 0)
            {
                timeRemaining = 0;
                EndGame(false); // Game over due to time running out
            }

            // Check if all packages have been delivered
            if (packagesDelivered >= totalPackages)
            {
                EndGame(true); // End game if all packages are delivered
            }
        }
    }

    // Pause the game and show the pause panel
    private void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f; // Pause game time
        pausePanel.SetActive(true); // Show the pause panel
    }

    // Resume the game and hide the pause panel
    private void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f; // Resume game time
        pausePanel.SetActive(false); // Hide the pause panel
    }

    // Ends the game and shows the end panel
    private void EndGame(bool isSuccess)
    {
        isGameActive = false;
        Time.timeScale = 0f; // Pause game time

        // Show the end panel and update its UI
        endPanel.SetActive(true);

        if (titleText != null)
        {
            titleText.text = isSuccess ? "SUCCESS!" : "TIME'S UP!";
        }

        if (timerEndText != null)
        {
            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);
            timerEndText.text = $"{minutes:00}:{seconds:00}";
            timerEndText.color = timeRemaining > 0 ? successColor : timeUpColor;
        }

        // Update star colors based on packages delivered
        if (starImages != null)
        {
            int filledStars = Mathf.RoundToInt((float)packagesDelivered / totalPackages * starImages.Length);
            for (int i = 0; i < starImages.Length; i++)
            {
                if (starImages[i] != null)
                {
                    starImages[i].color = (i < filledStars) ? filledStarColor : unfilledStarColor;
                }
            }
        }
    }

    // Restart the current level
    private void RestartGame()
    {
        Time.timeScale = 1f; // Ensure time is resumed
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Reload current scene
    }

    // Return to the main menu (assuming it's at index 0)
    private void ReturnToMainMenu()
    {
        Time.timeScale = 1f; // Ensure time is resumed
        SceneManager.LoadScene(0); // Load the main menu scene
    }

    // Updates the timer display in the UI
    private void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            int minutes = Mathf.Max(Mathf.FloorToInt(timeRemaining / 60), 0);
            int seconds = Mathf.Max(Mathf.FloorToInt(timeRemaining % 60), 0);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    // Updates the progress display in the UI
    private void UpdateProgressDisplay()
    {
        if (progressText != null)
        {
            progressText.text = $"{packagesDelivered}/{totalPackages}";
        }
    }

    // Call this function to mark a package as delivered
    public void PackageDelivered()
    {
        if (isGameActive && !isPaused)
        {
            packagesDelivered++;
            UpdateProgressDisplay();

            // Check if all packages are delivered
            if (packagesDelivered >= totalPackages)
            {
                EndGame(true); // End game when all packages are delivered
            }
        }
    }

    // Helper function to get the time remaining
    public float GetTimeRemaining() => timeRemaining;

    // Helper function to get the number of packages delivered
    public int GetPackagesDelivered() => packagesDelivered;

    // Helper function to get the total number of packages
    public int GetTotalPackages() => totalPackages;

    // Helper function to set the total packages for the game
    public void SetTotalPackages(int total)
    {
        totalPackages = total;
        UpdateProgressDisplay();
    }

    // Ensure we remove listeners when the game ends
    void OnDestroy()
    {
        if (resumeButton != null)
        {
            resumeButton.onClick.RemoveListener(ResumeGame);
        }
        if (pauseButton != null)
        {
            pauseButton.onClick.RemoveListener(PauseGame);
        }
        if (restartButton != null)
        {
            restartButton.onClick.RemoveListener(RestartGame);
        }
        if (pausedRestartButton != null)
        {
            pausedRestartButton.onClick.RemoveListener(RestartGame);
        }
        if (menuButton != null)
        {
            menuButton.onClick.RemoveListener(ReturnToMainMenu);
        }
    }
}
