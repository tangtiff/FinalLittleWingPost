using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PausePanel : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button menuButton;

    private bool isPaused = false;
    private bool hasEnded = false;

    void Start()
    {
        if (panel != null)
            panel.SetActive(false);

        if (resumeButton != null)
            resumeButton.onClick.AddListener(ResumeGame);
        if (menuButton != null)
            menuButton.onClick.AddListener(ReturnToMainMenu);
    }

    void Update()
    {
        // Check for escape key press to toggle pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (hasEnded) return;

        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
        
        if (panel != null)
        {
            panel.SetActive(isPaused);
            Debug.Log($"Pause panel {(isPaused ? "activated" : "deactivated")}");
        }
        else
        {
            Debug.LogError("Pause panel GameObject reference is missing!");
        }
    }

    private void SetPauseState(bool paused)
    {
        Time.timeScale = paused ? 0f : 1f;
    }

    private void ResumeGame()
    {
        TogglePause();
    }

    private void ReturnToMainMenu()
    {
        // Reset time scale before loading new scene
        Time.timeScale = 1f;
        // Load the main menu scene (assuming it's at build index 0)
        SceneManager.LoadScene(0);
    }

    void OnDestroy()
    {
        if (resumeButton != null)
            resumeButton.onClick.RemoveListener(ResumeGame);
        if (menuButton != null)
            menuButton.onClick.RemoveListener(ReturnToMainMenu);
    }
} 