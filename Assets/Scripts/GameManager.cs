using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Player & Game Settings")]
    public string playerName = "Player";
    public string selectedPictureName = "lion";
    public int gridRows = 5;
    public int gridCols = 5;
    public float timeLimit = 60f;
    public bool isPracticeMode = false;

    [Header("End Game Results")]
    public float finalScore = 0f;
    public bool playerQuitEarly = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            // Destroy duplicate instances created when loading scenes
            Destroy(gameObject);
        }
    }

    // --- Dynamic Slider Listener for Preferences UI ---

    // --- Dynamic Slider Listener for Preferences UI ---

    public void UpdateTimerUI()
    {
        Slider slider = Object.FindAnyObjectByType<Slider>();
        TextMeshProUGUI timerText = null;
        GameObject warningObj = null;

        // Search active and inactive objects safely
        TextMeshProUGUI[] allTexts = Resources.FindObjectsOfTypeAll<TextMeshProUGUI>();
        foreach (var t in allTexts)
        {
            if (t.gameObject.scene.isLoaded)
            {
                if (t.name == "timerText") timerText = t;
                if (t.name == "Practicemodewarning") warningObj = t.gameObject;
            }
        }

        if (slider == null) return;

        if (slider.value >= 1000f)
        {
            if (timerText != null) timerText.text = "Timer: OFF (Practice)";
            if (warningObj != null) warningObj.SetActive(true);
        }
        else
        {
            if (timerText != null) timerText.text = $"Timer: {Mathf.RoundToInt(slider.value)}s";
            if (warningObj != null) warningObj.SetActive(false);
        }
    }

    // --- Preferences Methods ---

    public void SavePreferencesAndPlayFromScene()
    {
        TMP_InputField nameInput = Object.FindAnyObjectByType<TMP_InputField>(FindObjectsInactive.Include);
        TMP_Dropdown pictureDropdown = Object.FindAnyObjectByType<TMP_Dropdown>(FindObjectsInactive.Include);
        Slider timerSlider = Object.FindAnyObjectByType<Slider>(FindObjectsInactive.Include);

        SavePreferencesAndPlay(nameInput, pictureDropdown, timerSlider);
    }

    public void SavePreferencesAndPlay(TMP_InputField nameInput, TMP_Dropdown pictureDropdown, Slider timerSlider)
    {
        if (nameInput != null && !string.IsNullOrEmpty(nameInput.text))
        {
            playerName = nameInput.text;
        }

        if (pictureDropdown != null && pictureDropdown.options.Count > pictureDropdown.value)
        {
            selectedPictureName = pictureDropdown.options[pictureDropdown.value].text.Trim().ToLower();
        }

        if (selectedPictureName.Contains("seaside"))
        {
            gridRows = 5;
            gridCols = 10;
        }
        else if (selectedPictureName.Contains("space"))
        {
            gridRows = 10;
            gridCols = 10;
        }
        else
        {
            gridRows = 5;
            gridCols = 5;
        }

        if (timerSlider != null)
        {
            if (timerSlider.value >= 1000f)
            {
                isPracticeMode = true;
                timeLimit = 0f;
            }
            else
            {
                isPracticeMode = false;
                timeLimit = timerSlider.value;
            }
        }

        SceneManager.LoadScene("chapterPuzzle");
    }

    // --- Navigation Shortcuts ---

    public void GoToPreferences()
    {
        SceneManager.LoadScene("preferences");
    }

    public void GoToIntro()
    {
        SceneManager.LoadScene("intro");
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void QuitApplication()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // --- High Score Operations ---
    public void SaveHighScore(float scoreToSave, Button saveButton)
    {
        PlayerPrefs.SetFloat("HighScore", scoreToSave);
        PlayerPrefs.SetString("HighScorePlayer", playerName);
        PlayerPrefs.Save();

        if (saveButton != null) saveButton.interactable = false;
    }
}