using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public static GameSettings Instance;

    public string playerName = "Player";
    public string selectedPictureName = "lion";

    public int gridRows = 5;
    public int gridCols = 5;

    public float timeLimit = 60f;
    public bool isPracticeMode = false; // Timer OFF mode flag

    public float finalScore = 0f;
    public bool playerQuitEarly = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}