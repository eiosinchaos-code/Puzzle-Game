using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PreferencesController : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TMP_Dropdown pictureDropdown;
    [SerializeField] private Slider timerSlider;
    [SerializeField] private TextMeshProUGUI timerTextLabel;
    [SerializeField] private TextMeshProUGUI practiceWarningText; // Dynamic warning text UI

    private void Start()
    {
        timerSlider.wholeNumbers = true;
        timerSlider.minValue = 30;
        timerSlider.maxValue = 200; // Max value used for special timer modes
        timerSlider.onValueChanged.AddListener(UpdateTimerLabel);
        UpdateTimerLabel(timerSlider.value);

        if (GameSettings.Instance != null && !string.IsNullOrEmpty(GameSettings.Instance.playerName))
        {
            nameInput.text = GameSettings.Instance.playerName;
        }
    }

    private void UpdateTimerLabel(float value)
    {
        if (value >= 200)
        {
            timerTextLabel.text = "Timer: OFF (Practice)";
            if (practiceWarningText != null)
            {
                practiceWarningText.text = "⚠️ Practice Mode: No score will be awarded upon completion!";
                practiceWarningText.gameObject.SetActive(true);
            }
        }
        else if (value >= 190)
        {
            timerTextLabel.text = "Timer: 999 Seconds";
            if (practiceWarningText != null) practiceWarningText.gameObject.SetActive(false);
        }
        else
        {
            timerTextLabel.text = $"{value} Seconds";
            if (practiceWarningText != null) practiceWarningText.gameObject.SetActive(false);
        }
    }

    public void SaveAndPlay()
    {
        GameSettings.Instance.playerName = string.IsNullOrEmpty(nameInput.text) ? "Player" : nameInput.text;

        string chosenPic = pictureDropdown.options[pictureDropdown.value].text.ToLower();
        GameSettings.Instance.selectedPictureName = chosenPic;

        if (chosenPic == "seaside")
        {
            GameSettings.Instance.gridRows = 5;
            GameSettings.Instance.gridCols = 10;
        }
        else if (chosenPic == "space")
        {
            GameSettings.Instance.gridRows = 10;
            GameSettings.Instance.gridCols = 10;
        }
        else
        {
            GameSettings.Instance.gridRows = 5;
            GameSettings.Instance.gridCols = 5;
        }

        // Timer mode selection logic
        if (timerSlider.value >= 200)
        {
            GameSettings.Instance.isPracticeMode = true;
            GameSettings.Instance.timeLimit = 0f;
        }
        else if (timerSlider.value >= 190)
        {
            GameSettings.Instance.isPracticeMode = false;
            GameSettings.Instance.timeLimit = 999f;
        }
        else
        {
            GameSettings.Instance.isPracticeMode = false;
            GameSettings.Instance.timeLimit = timerSlider.value;
        }

        SceneManager.LoadScene("chapterPuzzle");
    }

    public void GoBackToStart()
    {
        SceneManager.LoadScene("intro");
    }
}