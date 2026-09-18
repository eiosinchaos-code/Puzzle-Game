using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PreferencesUIController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TMP_Dropdown pictureDropdown;
    [SerializeField] private Slider timerSlider;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private GameObject practiceModeWarning;

    [Header("Buttons")]
    [SerializeField] private Button saveAndPlayButton;
    [SerializeField] private Button backButton;

    private void Start()
    {
        UpdateTimerDisplay();

        if (timerSlider != null)
        {
            timerSlider.onValueChanged.AddListener(delegate { UpdateTimerDisplay(); });
        }

        // Dynamically bind button clicks to persistent GameManager instance
        if (saveAndPlayButton != null)
        {
            saveAndPlayButton.onClick.RemoveAllListeners();
            saveAndPlayButton.onClick.AddListener(OnSaveAndPlayClicked);
        }

        if (backButton != null)
        {
            backButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(OnBackClicked);
        }
    }

    public void UpdateTimerDisplay()
    {
        if (timerSlider == null) return;

        if (timerSlider.value >= 1000f)
        {
            if (timerText != null) timerText.text = "Timer: OFF (Practice)";
            if (practiceModeWarning != null) practiceModeWarning.SetActive(true);
        }
        else
        {
            if (timerText != null) timerText.text = $"Timer: {Mathf.RoundToInt(timerSlider.value)}s";
            if (practiceModeWarning != null) practiceModeWarning.SetActive(false);
        }
    }

    private void OnSaveAndPlayClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SavePreferencesAndPlay(nameInput, pictureDropdown, timerSlider);
        }
    }

    private void OnBackClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GoToIntro();
        }
    }
}