using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ExitSceneController : MonoBehaviour
{
    [Header("UI Text References")]
    [SerializeField] private TextMeshProUGUI resultMessageText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;

    [Header("UI Button References")]
    [SerializeField] private Button saveScoreButton;
    [SerializeField] private Button playAgainButton;
    [SerializeField] private Button quitGameButton;

    private float currentScore;
    private float highScore;

    private void Start()
    {
        if (GameSettings.Instance != null)
        {
            currentScore = GameSettings.Instance.finalScore;
            highScore = PlayerPrefs.GetFloat("HighScore", 0f);

            if (GameSettings.Instance.isPracticeMode)
            {
                resultMessageText.text = $"Great practice, {GameSettings.Instance.playerName}!";
                scoreText.text = "Your Score: N/A (Practice Mode - No Score Recorded)";
            }
            else
            {
                string quitStatus = GameSettings.Instance.playerQuitEarly ? " (Quit Early)" : "";
                resultMessageText.text = $"Thanks for playing, {GameSettings.Instance.playerName}!{quitStatus}";
                scoreText.text = $"Your Score: {currentScore}";
            }

            highScoreText.text = $"High Score: {highScore}";

            // Only allow saving if not in practice mode and new score beats high score
            if (saveScoreButton != null)
            {
                saveScoreButton.interactable = (!GameSettings.Instance.isPracticeMode && currentScore > highScore);
            }
        }
    }

    public void SaveHighScore()
    {
        PlayerPrefs.SetFloat("HighScore", currentScore);
        PlayerPrefs.SetString("HighScorePlayer", GameSettings.Instance.playerName);
        PlayerPrefs.Save();

        highScore = currentScore;
        highScoreText.text = $"High Score: {highScore}";

        if (saveScoreButton != null)
        {
            saveScoreButton.interactable = false;
        }
    }

    public void GoToPreferences()
    {
        SceneManager.LoadScene("preferences");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}