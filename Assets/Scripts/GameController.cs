using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public float skySpeed;  // Speed of skybox rotation

    [Header("Timer Settings")]
    [SerializeField] private float gameDuration;     // Game duration in seconds
    [SerializeField] private Text timerText;         // Timer
    private float timeRemaining;
    private bool isGameActive = true;
    private bool isPaused = false;

    [Header("Score and Progress Settings")]
    [SerializeField] private GameObject inGamePanel; // In-game panel UI
    [SerializeField] private Text progressText;      // Delivery progress text (i.e. 0/5)
    [SerializeField] private int packagesDelivered = 0;     // Number of packages delivered
    [SerializeField] private int totalPackages = 0;  // Total packages to deliver

    [Header("UI Elements")]
    [SerializeField] private GameObject pausePanel;  // Pause panel UI
    [SerializeField] private Button pauseButton;     // Button to pause game
    [SerializeField] private Button resumeButton;    // Button to resume game
    [SerializeField] private Button pausedRestartButton;    // Button to restart game (from pause)

    [Header("End Panel UI")]
    [SerializeField] private GameObject endPanel;    // End panel UI
    [SerializeField] private Text titleText;         // End message text
    [SerializeField] private Text timerEndText;      // Remaining time text
    [SerializeField] private Button restartButton;   // Button to restart game
    [SerializeField] private Button menuButton;      // Button to return to menu

    [Header("End Message Colours")]
    [SerializeField] private Color successColour;    // Colour for win
    [SerializeField] private Color timeUpColour;     // Colour for lose

    [Header("Stars")]
    [SerializeField] private Image[] starImages = new Image[5];
    [SerializeField] private Color filledStarColor;
    [SerializeField] private Color unfilledStarColor;

    void Start()
    {
        timeRemaining = gameDuration;
        UpdateTimerDisplay();
        UpdateProgressDisplay();

        // Set up UI buttons
        if (resumeButton != null)
            resumeButton.onClick.AddListener(ResumeGame);
        if (pauseButton != null)
            pauseButton.onClick.AddListener(PauseGame);
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);
        if (pausedRestartButton != null)
            pausedRestartButton.onClick.AddListener(RestartGame);
        if (menuButton != null)
            menuButton.onClick.AddListener(ReturnToMainMenu);

        // Setting visibility of panels
        pausePanel.SetActive(false);
        inGamePanel.SetActive(true);
        endPanel.SetActive(false);
    }

    void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * skySpeed);  // Rotate skybox

        // Toggle pause for esc
        if (Input.GetKeyDown(KeyCode.Escape) && isGameActive)
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }

        // Update timer and check game end conditions when unpaused
        if (isGameActive && !isPaused)
        {
            timeRemaining -= Time.deltaTime;
            UpdateTimerDisplay();

            if (timeRemaining <= 0)
            {
                timeRemaining = 0;
                EndGame(false); // Game over
            }

            // Check if all packages have been delivered
            if (packagesDelivered >= totalPackages)
            {
                EndGame(true);
            }
        }
    }

    // Pause game and show pause panel
    private void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f; // Pause time
        pausePanel.SetActive(true);
    }

    // Resume game and hide pause panel
    private void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f; // Resume game time
        pausePanel.SetActive(false); // Hide the pause panel
    }

    // End game and show end panel
    private void EndGame(bool isSuccess)
    {
        isGameActive = false;
        Time.timeScale = 0f; // Pause time

        // Show end panel and update UI
        endPanel.SetActive(true);
        inGamePanel.SetActive(false);

        if (titleText != null)
        {
            titleText.text = isSuccess ? "SUCCESS!" : "TIME'S UP!";
        }

        if (timerEndText != null)
        {
            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);
            timerEndText.text = $"{minutes:00}:{seconds:00}";
            timerEndText.color = timeRemaining > 0 ? successColour : timeUpColour;
        }

        // Update star colours
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

    // Restart game
    private void RestartGame()
    {
        Time.timeScale = 1f; // Ensure time is resumed
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Reload current scene
    }

    // Return to main menu (index 0)
    private void ReturnToMainMenu()
    {
        Time.timeScale = 1f; // Resume time
        SceneManager.LoadScene(0); // Load main menu scene
    }

    // Updates the timer display
    private void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            int minutes = Mathf.Max(Mathf.FloorToInt(timeRemaining / 60), 0);
            int seconds = Mathf.Max(Mathf.FloorToInt(timeRemaining % 60), 0);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    // Updates the delivery progress UI
    private void UpdateProgressDisplay()
    {
        if (progressText != null)
        {
            progressText.text = $"{packagesDelivered}/{totalPackages}";
        }
    }

    // TODO: USE TO MARK PACKAGE AS DELIVERED
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

    // Helper function to get remaining time
    public float GetTimeRemaining() => timeRemaining;

    // Helper function to get number of packages delivered
    public int GetPackagesDelivered() => packagesDelivered;

    // Helper function to get total number of packages
    public int GetTotalPackages() => totalPackages;

    // Helper function to set total packages
    public void SetTotalPackages(int total)
    {
        totalPackages = total;
        UpdateProgressDisplay();
    }

    void OnDestroy()
    {
        if (resumeButton != null)
            resumeButton.onClick.RemoveListener(ResumeGame);
        if (pauseButton != null)
            pauseButton.onClick.RemoveListener(PauseGame);
        if (restartButton != null)
            restartButton.onClick.RemoveListener(RestartGame);
        if (pausedRestartButton != null)
            pausedRestartButton.onClick.RemoveListener(RestartGame);
        if (menuButton != null)
            menuButton.onClick.RemoveListener(ReturnToMainMenu);
    }
}
