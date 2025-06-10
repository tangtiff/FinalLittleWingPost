using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InGamePanel : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Text timerText;
    [SerializeField] private Text scoreText;
    [SerializeField] private Button pauseButton;
    [SerializeField] private PausePanel pausePanel;
    [SerializeField] private EndPanel endPanel;
    [SerializeField] private GameObject inGamePanelRoot;

    private SceneController gameController;

    void Start()
    {
        // Find the SceneController
        gameController = FindObjectOfType<SceneController>();
        if (gameController == null)
        {
            Debug.LogError("SceneController not found in the scene!");
            return;
        }

        // Setup pause button
        if (pauseButton != null)
            pauseButton.onClick.AddListener(OnPauseButtonClicked);

        // Initial UI update
        UpdateTimerDisplay();
        UpdateScoreDisplay();
    }

    void Update()
    {
        if (gameController == null) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pausePanel == null)
            {
                Debug.LogError("PausePanel reference is missing in InGamePanel!");
                return;
            }
            pausePanel.TogglePause();
        }

        // Update UI every frame
        UpdateTimerDisplay();
        UpdateScoreDisplay();
    }

    private void OnPauseButtonClicked()
    {
        if (pausePanel == null)
        {
            Debug.LogError("PausePanel reference is missing in InGamePanel!");
            return;
        }
        pausePanel.TogglePause();
    }

    private void UpdateTimerDisplay()
    {
        if (timerText != null && gameController != null)
        {
            float timeRemaining = gameController.GetTimeRemaining();
            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    private void UpdateScoreDisplay()
    {
        if (scoreText != null && gameController != null)
        {
            int delivered = gameController.GetPackagesDelivered();
            int total = gameController.GetTotalPackages();
            scoreText.text = $"{delivered}/{total}";
        }
    }

    void OnDestroy()
    {
        if (pauseButton != null)
            pauseButton.onClick.RemoveListener(OnPauseButtonClicked);
    }
} 