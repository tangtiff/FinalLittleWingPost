using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class CountdownTimer : MonoBehaviour
{
    [SerializeField] private float gameDuration = 180f; // 3 minutes in seconds
    [SerializeField] private Text timerText; // Reference to the UI Text component
    [SerializeField] private GameObject endingScreenPanel; // Reference to the ending screen UI panel
    [SerializeField] private Text finalTimeText; // Text to display final time
    [SerializeField] private Button mainMenuButton; // Button to return to main menu
    [SerializeField] private Button restartButton; // Button to restart the game
    
    [Header("Ending Messages")]
    [SerializeField] private string timeUpMessage = "Time's Up!";
    [SerializeField] private string successMessage = "All Packages Delivered!";
    
    [Header("Score Display")]
    [SerializeField] private Text scoreText; // Reference to the score text component
    [SerializeField] private int currentScore = 0; // Current score value
    
    [Header("Final Timer Colors")]
    [SerializeField] private Color successColor = Color.green; // Color when packages are delivered
    [SerializeField] private Color timeUpColor = Color.red; // Color when time is up
    
    private float timeRemaining;
    private bool isGameActive = true;
    private bool hasEnded = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timeRemaining = gameDuration;
        UpdateTimerDisplay();
        UpdateScoreDisplay();
        
        // Hide ending screen at start
        if (endingScreenPanel != null)
            endingScreenPanel.SetActive(false);
            
        // Setup button listeners
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(ReturnToMainMenu);
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);
            
        // Show timer
        if (timerText != null)
        {
            timerText.gameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isGameActive && timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            UpdateTimerDisplay();

            if (timeRemaining <= 0)
            {
                timeRemaining = 0;
                isGameActive = false;
                OnTimerComplete();
            }
        }
    }

    private void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void OnTimerComplete()
    {
        if (!hasEnded)
        {
            ShowEndingScreen(timeUpMessage, timeUpColor);
        }
    }

    public void OnAllPackagesDelivered()
    {
        if (!hasEnded)
        {
            ShowEndingScreen(successMessage, successColor);
        }
    }

    // Call this method to update the score
    public void AddScore(int points)
    {
        currentScore += points;
        UpdateScoreDisplay();
    }

    private void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {currentScore}";
        }
    }

    private void ShowEndingScreen(string message, Color messageColor)
    {
        hasEnded = true;
        isGameActive = false;
        
        // Hide the in-game timer
        if (timerText != null)
        {
            timerText.gameObject.SetActive(false);
        }
        
        if (endingScreenPanel != null)
        {
            endingScreenPanel.SetActive(true);
            
            // Update the final time and score display with appropriate color
            if (finalTimeText != null)
            {
                int minutes = Mathf.FloorToInt(timeRemaining / 60);
                int seconds = Mathf.FloorToInt(timeRemaining % 60);
                finalTimeText.text = $"{message}\nTime Remaining: {minutes:00}:{seconds:00}\nFinal Score: {currentScore}";
                finalTimeText.color = messageColor;
            }
        }
    }

    private void RestartGame()
    {
        // Show the timer again before restarting
        if (timerText != null)
        {
            timerText.gameObject.SetActive(true);
        }
        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void ReturnToMainMenu()
    {
        // Load the main menu scene (assuming it's at build index 0)
        SceneManager.LoadScene(0);
    }

    void OnDestroy()
    {
        // Clean up button listeners
        if (mainMenuButton != null)
            mainMenuButton.onClick.RemoveListener(ReturnToMainMenu);
        if (restartButton != null)
            restartButton.onClick.RemoveListener(RestartGame);
    }
}
