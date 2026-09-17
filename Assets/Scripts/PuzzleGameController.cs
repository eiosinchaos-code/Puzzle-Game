using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PuzzleGameController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameDisplay;
    [SerializeField] private TextMeshProUGUI timerDisplay;

    private ManagePuzzleGame puzzleManager;
    private float timeRemaining;
    private bool isGameActive = true;

    private void Start()
    {
        puzzleManager = Object.FindAnyObjectByType<ManagePuzzleGame>();

        if (GameSettings.Instance != null)
        {
            nameDisplay.text = $"Player: {GameSettings.Instance.playerName}";
            timeRemaining = GameSettings.Instance.timeLimit;
            GameSettings.Instance.playerQuitEarly = false;

            if (GameSettings.Instance.isPracticeMode)
            {
                timerDisplay.text = "Timer: OFF (Practice)";
            }
            else
            {
                timerDisplay.text = "Get Ready...";
            }
        }
    }

    private void Update()
    {
        if (!isGameActive || GameSettings.Instance.isPracticeMode) return;

        // Wait until pieces spawn and shuffle before counting down
        if (puzzleManager != null && !puzzleManager.isSetupComplete)
        {
            return;
        }

        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            timerDisplay.text = $"Time: {Mathf.CeilToInt(timeRemaining)}s";
        }
        else
        {
            timeRemaining = 0;
            isGameActive = false;
            GameSettings.Instance.finalScore = 0;
            EndGame();
        }
    }

    public void QuitEarly()
    {
        if (!isGameActive) return;
        isGameActive = false;

        GameSettings.Instance.playerQuitEarly = true;

        if (GameSettings.Instance.isPracticeMode)
        {
            GameSettings.Instance.finalScore = 0;
        }
        else
        {
            float baseScore = timeRemaining * 10f;
            GameSettings.Instance.finalScore = Mathf.CeilToInt(baseScore * 0.25f);
        }

        EndGame();
    }

    public void CompletePuzzle()
    {
        if (!isGameActive) return;
        isGameActive = false;

        if (GameSettings.Instance.isPracticeMode)
        {
            GameSettings.Instance.finalScore = 0;
        }
        else
        {
            GameSettings.Instance.finalScore = Mathf.CeilToInt(timeRemaining * 10f);
        }

        EndGame();
    }

    private void EndGame()
    {
        SceneManager.LoadScene("exit");
    }
}