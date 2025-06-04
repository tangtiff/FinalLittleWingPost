using UnityEngine;
using UnityEngine.SceneManagement;

public class BasePanelManager : MonoBehaviour
{
    protected bool isGameActive = true;
    protected bool isPaused = false;
    protected bool hasEnded = false;

    protected void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    protected void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    protected void SetPauseState(bool paused)
    {
        if (hasEnded) return;
        isPaused = paused;
        Time.timeScale = isPaused ? 0f : 1f;
    }
} 