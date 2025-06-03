using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartScreenController : MonoBehaviour
{
    public Button startButton;

    void Start()
    {
        startButton.onClick.AddListener(OnStartButtonClicked);
    }

    // Loads game scene when start button clicked
    void OnStartButtonClicked()
    {
        SceneManager.LoadScene("SampleScene");
    }
}