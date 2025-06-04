using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndPanel : BasePanelManager
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Text messageText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button menuButton;
    
    [Header("Messages")]
    [SerializeField] private string timeUpMessage = "Time's Up!";
    [SerializeField] private string successMessage = "All Packages Delivered!";
    [SerializeField] private Color successColor = Color.green;
    [SerializeField] private Color timeUpColor = Color.red;

    void Start()
    {
        if (panel != null)
            panel.SetActive(false);

        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);
        if (menuButton != null)
            menuButton.onClick.AddListener(ReturnToMainMenu);
    }

    public void ShowTimeUp()
    {
        if (hasEnded) return;
        ShowEndScreen(timeUpMessage, timeUpColor);
    }

    public void ShowSuccess(int packagesDelivered, int totalPackages, float timeRemaining)
    {
        if (hasEnded) return;
        ShowEndScreen(successMessage, successColor, packagesDelivered, totalPackages, timeRemaining);
    }

    private void ShowEndScreen(string message, Color messageColor, int packagesDelivered = 0, int totalPackages = 0, float timeRemaining = 0)
    {
        hasEnded = true;
        isGameActive = false;
        
        if (panel != null)
        {
            panel.SetActive(true);
            if (messageText != null)
            {
                int minutes = Mathf.FloorToInt(timeRemaining / 60);
                int seconds = Mathf.FloorToInt(timeRemaining % 60);
                messageText.text = $"{message}\nTime Remaining: {minutes:00}:{seconds:00}\nPackages Delivered: {packagesDelivered}/{totalPackages}";
                messageText.color = messageColor;
            }
        }
    }

    void OnDestroy()
    {
        if (restartButton != null)
            restartButton.onClick.RemoveListener(RestartGame);
        if (menuButton != null)
            menuButton.onClick.RemoveListener(ReturnToMainMenu);
    }
} 