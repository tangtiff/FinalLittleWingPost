using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    [SerializeField] private float gameDuration = 180f; // 3 minutes in seconds
    [SerializeField] private Text timerText; // Reference to the UI Text component
    private float timeRemaining;
    private bool isGameActive = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timeRemaining = gameDuration;
        UpdateTimerDisplay();
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
        // Add your game over logic here
        Debug.Log("Time's up!");
        // You can trigger game over events, show a game over screen, etc.
    }
}
