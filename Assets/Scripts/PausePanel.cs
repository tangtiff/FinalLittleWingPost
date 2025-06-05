using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PausePanel : BasePanelManager
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button menuButton;

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
        SetPauseState(isPaused);
        
        if (panel != null)
            panel.SetActive(isPaused);
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