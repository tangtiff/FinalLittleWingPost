using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndPanel
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Text titleText;
    [SerializeField] private Text timerText;
    // [SerializeField] private Text scoreText;
    // [SerializeField] private Text deliveredText;
    [SerializeField] private Image timerBackgroundImage;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button menuButton;
    
    [Header("Messages")]
    [SerializeField] private Color successColor = Color.green;
    [SerializeField] private Color timeUpColor = Color.red;

    [Header("Stars")]
    [SerializeField] private Image[] starImages = new Image[5];
    [SerializeField] private Color filledStarColor = Color.yellow;
    [SerializeField] private Color unfilledStarColor = Color.gray;

    void Start()
    {
        if (panel != null)
            panel.SetActive(false);

        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);
        if (menuButton != null)
            menuButton.onClick.AddListener(ReturnToMainMenu);
    }

    public void ShowTimeUp(int packagesDelivered, int totalPackages, float timeRemaining)
    {
        if (hasEnded) return;
        ShowEndScreen(false, packagesDelivered, totalPackages, timeRemaining);
    }

    public void ShowSuccess(int packagesDelivered, int totalPackages, float timeRemaining, int score)
    {
        if (hasEnded) return;
        ShowEndScreen(true, packagesDelivered, totalPackages, timeRemaining, score);
    }

    private void ShowEndScreen(bool isSuccess, int packagesDelivered, int totalPackages, float timeRemaining, int score = 0)
    {
        hasEnded = true;
        isGameActive = false;
        if (panel != null)
        {
            panel.SetActive(true);
            if (titleText != null)
                titleText.text = isSuccess ? "SUCCESS!" : "TIME'S UP!";
            if (timerBackgroundImage != null)
                timerBackgroundImage.color = isSuccess ? successColor : timeUpColor;
            if (timerText != null)
            {
                int minutes = Mathf.FloorToInt(timeRemaining / 60);
                int seconds = Mathf.FloorToInt(timeRemaining % 60);
                timerText.text = $"{minutes:00}:{seconds:00}";
                timerText.color = timeRemaining > 0 ? successColor : timeUpColor;
            }
            // if (scoreText != null) scoreText.gameObject.SetActive(isSuccess);
            // if (deliveredText != null) deliveredText.gameObject.SetActive(!isSuccess);
            // if (isSuccess && scoreText != null)
            // {
            //     scoreText.text = $"Score: {score}";
            // }
            // if (!isSuccess && deliveredText != null)
            // {
            //     deliveredText.text = $"{packagesDelivered}/{totalPackages} delivered";
            // }
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
    }

    void OnDestroy()
    {
        if (restartButton != null)
            restartButton.onClick.RemoveListener(RestartGame);
        if (menuButton != null)
            menuButton.onClick.RemoveListener(ReturnToMainMenu);
    }
}