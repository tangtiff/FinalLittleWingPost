using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InGamePanel : MonoBehaviour
{
    [Header("Timer Settings")]
    [SerializeField] private float gameDuration = 180f;
    [SerializeField] private Text timerText;
    private float timeRemaining;
    private bool isGameActive = true;
    private bool isPaused = false;

    [Header("Score Display")]
    [SerializeField] private Text scoreText;
    [SerializeField] private int packagesDelivered = 0;
    [SerializeField] private int totalPackages = 0;

    [Header("Pause Button")]
    [SerializeField] private Button pauseButton;

    [Header("References")]
    [SerializeField] private PausePanel pausePanel;
    [SerializeField] private EndPanel endPanel;
    [SerializeField] private GameObject inGamePanelRoot;

    void Start()
    {
        timeRemaining = gameDuration;
        UpdateTimerDisplay();
        UpdateScoreDisplay();

        // Setup pause button
        if (pauseButton != null)
            pauseButton.onClick.AddListener(OnPauseButtonClicked);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && isGameActive)
        {
            if (pausePanel == null)
            {
                Debug.LogError("PausePanel reference is missing in InGamePanel!");
                return;
            }
            pausePanel.TogglePause();
            isPaused = !isPaused;
        }

        if (isGameActive && !isPaused && timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            UpdateTimerDisplay();

            if (timeRemaining <= 0)
            {
                timeRemaining = 0;
                EndGame(false);
            }
        }
    }

    private void OnPauseButtonClicked()
    {
        if (pausePanel == null)
        {
            Debug.LogError("PausePanel reference is missing in InGamePanel!");
            return;
        }
        pausePanel.TogglePause();
        isPaused = !isPaused; // Update our own pause state
    }

    private void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    private void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = $"{packagesDelivered}/{totalPackages}";
        }
    }

    public void PackageDelivered()
    {
        packagesDelivered++;
        UpdateScoreDisplay();

        if (packagesDelivered >= totalPackages)
        {
            EndGame(true);
        }
    }

    private void EndGame(bool isSuccess)
    {
        isGameActive = false;
        Time.timeScale = 0f; // Stop the game
        if (inGamePanelRoot != null)
            inGamePanelRoot.SetActive(false);
        
        if (isSuccess)
            endPanel.ShowSuccess(packagesDelivered, totalPackages, timeRemaining, packagesDelivered);
        else
            endPanel.ShowTimeUp(packagesDelivered, totalPackages, timeRemaining);
    }

    public float GetTimeRemaining() => timeRemaining;
    public int GetPackagesDelivered() => packagesDelivered;
    public int GetTotalPackages() => totalPackages;

    public void ApplyTimePenalty(float penaltySeconds)
    {
        if (isGameActive && !isPaused)
        {
            timeRemaining = Mathf.Max(0f, timeRemaining - penaltySeconds);
            UpdateTimerDisplay();
            
            // If time runs out after penalty, end the game
            if (timeRemaining <= 0)
            {
                EndGame(false);
            }
        }
    }

    void OnDestroy()
    {
        if (pauseButton != null)
            pauseButton.onClick.RemoveListener(OnPauseButtonClicked);
    }
} 