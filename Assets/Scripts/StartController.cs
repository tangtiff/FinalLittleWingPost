using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class StartScreenController : MonoBehaviour
{
    public Button startButton;
    public Button helpButton;
    public GameObject instructionPanel;
    public TMP_Text helpButtonText;

    private bool isPanelVisible = false;

    void Start()
    {
        startButton.onClick.AddListener(OnStartButtonClicked);
        helpButton.onClick.AddListener(OnHelpButtonToggled);
        instructionPanel.SetActive(false);
        UpdateHelpButtonText();
    }

    void OnStartButtonClicked()
    {
        SceneManager.LoadScene("SampleScene");
    }

    void OnHelpButtonToggled()
    {
        isPanelVisible = !isPanelVisible;
        instructionPanel.SetActive(isPanelVisible);
        UpdateHelpButtonText();
    }

    void UpdateHelpButtonText()
    {
        if (helpButtonText != null)
        {
            helpButtonText.text = isPanelVisible ? "X" : "?";
        }
    }
}
