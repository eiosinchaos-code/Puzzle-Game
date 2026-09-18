using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExitUIController : MonoBehaviour
{
    [Header("UI Text Displays")]
    [SerializeField] private TextMeshProUGUI resultMessage;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;

    [Header("Action Buttons")]
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button saveHighScoreButton;

    private void Start()
    {
        if (GameManager.Instance == null) return;

        // Display Player Name and Score inside Scoretext
        if (scoreText != null)
        {
            scoreText.text = $"{GameManager.Instance.playerName} - Score: {Mathf.RoundToInt(GameManager.Instance.finalScore)}";
        }

        // Display message inside Resultmessage based on game outcome
        if (resultMessage != null)
        {
            if (GameManager.Instance.playerQuitEarly)
            {
                resultMessage.text = "Game Over - Quit Early!";
            }
            else if (GameManager.Instance.isPracticeMode)
            {
                resultMessage.text = "Practice Complete!";
            }
            else if (GameManager.Instance.finalScore > 0)
            {
                resultMessage.text = "Puzzle Completed!";
            }
            else
            {
                resultMessage.text = "Time's Up!";
            }
        }

        // Update High Score Display
        UpdateHighScoreDisplay();

        // Bind Button Listeners
        if (saveHighScoreButton != null)
        {
            saveHighScoreButton.onClick.RemoveAllListeners();
            saveHighScoreButton.onClick.AddListener(OnSaveHighScoreClicked);
        }

        if (settingsButton != null)
        {
            settingsButton.onClick.RemoveAllListeners();
            settingsButton.onClick.AddListener(OnSettingsClicked);
        }

        if (quitButton != null)
        {
            quitButton.onClick.RemoveAllListeners();
            quitButton.onClick.AddListener(OnQuitClicked);
        }
    }

    private void UpdateHighScoreDisplay()
    {
        if (highScoreText != null)
        {
            float topScore = PlayerPrefs.GetFloat("HighScore", 0f);
            string topPlayer = PlayerPrefs.GetString("HighScorePlayer", "None");
            highScoreText.text = $"High Score: {Mathf.RoundToInt(topScore)} ({topPlayer})";
        }
    }

    private void OnSaveHighScoreClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SaveHighScore(GameManager.Instance.finalScore, saveHighScoreButton);
            UpdateHighScoreDisplay();
        }
    }

    private void OnSettingsClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GoToPreferences();
        }
    }

    private void OnQuitClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.QuitApplication();
        }
    }
}