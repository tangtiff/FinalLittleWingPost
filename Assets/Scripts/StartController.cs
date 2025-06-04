using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartScreenController : MonoBehaviour
{
    public Button startButton;
    public Button helpButton;
    public GameObject instructionPanel;
    public Button closeButton;

    void Start()
    {
        startButton.onClick.AddListener(OnStartButtonClicked);
        helpButton.onClick.AddListener(OnHelpButtonClicked);
        closeButton.onClick.AddListener(OnCloseButtonClicked);
        instructionPanel.SetActive(false);
    }
    void OnStartButtonClicked()
    {
        SceneManager.LoadScene("SampleScene");
    }

    // Displays how-to-play panel when help button clicked
    void OnHelpButtonClicked()
    {
        instructionPanel.SetActive(true);
    }

    // Closes how-to-play panel when close button clicked
    void OnCloseButtonClicked()
    {
        instructionPanel.SetActive(false);
    }
}
