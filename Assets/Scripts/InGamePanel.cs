using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InGamePanel : BasePanelManager
{
    [Header("Timer Settings")]
    [SerializeField] private float gameDuration = 180f;
    [SerializeField] private Text timerText;
    private float timeRemaining;

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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pausePanel.TogglePause();
        }

        if (isGameActive && !isPaused && timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            UpdateTimerDisplay();

            if (timeRemaining <= 0)
            {
                timeRemaining = 0;
                isGameActive = false;
                if (inGamePanelRoot != null)
                    inGamePanelRoot.SetActive(false);
                endPanel.ShowTimeUp(packagesDelivered, totalPackages, timeRemaining);            }
        }
    }

    private void OnPauseButtonClicked()
    {
        pausePanel.TogglePause();
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
            scoreText.text = $"Packages: {packagesDelivered}/{totalPackages}";
        }
    }

    public void PackageDelivered()
    {
        packagesDelivered++;
        UpdateScoreDisplay();

        if (packagesDelivered >= totalPackages)
        {
            isGameActive = false;
            if (inGamePanelRoot != null)
                inGamePanelRoot.SetActive(false);
            endPanel.ShowSuccess(packagesDelivered, totalPackages, timeRemaining, packagesDelivered);
        }
    }

    public float GetTimeRemaining() => timeRemaining;
    public int GetPackagesDelivered() => packagesDelivered;
    public int GetTotalPackages() => totalPackages;

    void OnDestroy()
    {
        if (pauseButton != null)
            pauseButton.onClick.RemoveListener(OnPauseButtonClicked);
    }
} 