using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PuzzleGameplayManager : MonoBehaviour
{
    [Header("Prefabs & Containers")]
    [SerializeField] private Image piecePrefab;
    [SerializeField] private Image placeholderPrefab;
    [SerializeField] private RectTransform leftSideContainer;
    [SerializeField] private RectTransform rightSideContainer;

    [Header("UI Displays")]
    [SerializeField] private TextMeshProUGUI nameDisplay;
    [SerializeField] private TextMeshProUGUI timerDisplay;

    [Header("Hint System")]
    [SerializeField] private GameObject hintOverlay;
    [SerializeField] private Image hintOverlayImage;
    [SerializeField] private Button hintButton;

    [Header("Canvas Scaling Settings")]
    [SerializeField] private float areaWidth = 700f;
    [SerializeField] private float areaHeight = 700f;

    private float cellWidth;
    private float cellHeight;
    private int totalPieces;
    private int correctlyPlacedPieces = 0;

    private float timeRemaining;
    private bool isGameActive = true;
    private bool isSetupComplete = false;

    private void Start()
    {
        EnsureGameManager();

        totalPieces = GameManager.Instance.gridRows * GameManager.Instance.gridCols;
        timeRemaining = GameManager.Instance.timeLimit;
        GameManager.Instance.playerQuitEarly = false;

        if (nameDisplay != null)
            nameDisplay.text = $"Player: {GameManager.Instance.playerName}";

        if (timerDisplay != null)
            timerDisplay.text = GameManager.Instance.isPracticeMode ? "Timer: OFF (Practice)" : "Get Ready...";

        if (rightSideContainer != null && rightSideContainer.rect.width > 0)
        {
            areaWidth = rightSideContainer.rect.width;
            areaHeight = rightSideContainer.rect.height;
        }

        if (leftSideContainer != null) leftSideContainer.sizeDelta = new Vector2(areaWidth, areaHeight);
        if (rightSideContainer != null) rightSideContainer.sizeDelta = new Vector2(areaWidth, areaHeight);

        string picture = GameManager.Instance.selectedPictureName;

        // Loads sliced sprites inside the specific picture folder (e.g., Resources/lion/)
        Sprite[] sprites = Resources.LoadAll<Sprite>(picture);

        CalculateCellSizes(sprites);
        SetupHintOverlay(picture);

        CreatePlaceholders();
        CreatePieces(sprites);

        StartCoroutine(ShuffleRoutine());
    }

    private void EnsureGameManager()
    {
        if (GameManager.Instance == null)
        {
            GameObject managerObj = new GameObject("GameManager");
            managerObj.AddComponent<GameManager>();
        }
    }

    private void CalculateCellSizes(Sprite[] sprites)
    {
        float targetWidth = areaWidth / GameManager.Instance.gridCols;
        float targetHeight = areaHeight / GameManager.Instance.gridRows;

        if (sprites != null && sprites.Length > 0 && sprites[0] != null)
        {
            float aspect = sprites[0].rect.width / sprites[0].rect.height;
            if (targetWidth / targetHeight > aspect)
            {
                cellHeight = targetHeight;
                cellWidth = targetHeight * aspect;
            }
            else
            {
                cellWidth = targetWidth;
                cellHeight = targetWidth / aspect;
            }
        }
        else
        {
            cellWidth = targetWidth;
            cellHeight = targetHeight;
        }
    }

    private void Update()
    {
        if (!isGameActive || !isSetupComplete || GameManager.Instance.isPracticeMode) return;

        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            if (timerDisplay != null)
                timerDisplay.text = $"Time: {Mathf.CeilToInt(timeRemaining)}s";
        }
        else
        {
            timeRemaining = 0;
            GameManager.Instance.finalScore = 0;
            EndGame();
        }
    }

    private Vector3 GetGridPosition(int index)
    {
        int col = index % GameManager.Instance.gridCols;
        int row = index / GameManager.Instance.gridCols;

        float x = cellWidth * (col - (GameManager.Instance.gridCols - 1) / 2f);
        float y = -cellHeight * (row - (GameManager.Instance.gridRows - 1) / 2f);

        return new Vector3(x, y, 0f);
    }

    private void CreatePlaceholders()
    {
        Transform parent = rightSideContainer != null ? rightSideContainer : transform;
        for (int i = 0; i < totalPieces; i++)
        {
            Image ph = Instantiate(placeholderPrefab, parent);
            ph.name = $"PH{i + 1}";
            ph.rectTransform.sizeDelta = new Vector2(cellWidth, cellHeight);
            ph.rectTransform.anchoredPosition = GetGridPosition(i);

            PuzzleID id = ph.gameObject.GetComponent<PuzzleID>() ?? ph.gameObject.AddComponent<PuzzleID>();
            id.id = i + 1;
        }
    }

    private void CreatePieces(Sprite[] sprites)
    {
        Transform parent = leftSideContainer != null ? leftSideContainer : transform;
        for (int i = 0; i < totalPieces; i++)
        {
            Image p = Instantiate(piecePrefab, parent);
            p.name = $"Piece{i + 1}";
            p.rectTransform.sizeDelta = new Vector2(cellWidth, cellHeight);
            p.rectTransform.anchoredPosition = GetGridPosition(i);

            PuzzleID id = p.gameObject.GetComponent<PuzzleID>() ?? p.gameObject.AddComponent<PuzzleID>();
            id.id = i + 1;

            if (sprites != null && i < sprites.Length) p.sprite = sprites[i];
        }
    }

    private IEnumerator ShuffleRoutine()
    {
        yield return new WaitForSeconds(2f);

        int[] indices = new int[totalPieces];
        for (int i = 0; i < totalPieces; i++) indices[i] = i;

        for (int i = 0; i < totalPieces; i++)
        {
            int rnd = Random.Range(i, totalPieces);
            int temp = indices[i];
            indices[i] = indices[rnd];
            indices[rnd] = temp;
        }

        for (int i = 0; i < totalPieces; i++)
        {
            GameObject pieceObj = GameObject.Find($"Piece{i + 1}");
            if (pieceObj != null)
            {
                pieceObj.GetComponent<RectTransform>().anchoredPosition = GetGridPosition(indices[i]);
                pieceObj.GetComponent<DragAndDrop>()?.InitCardPosition();
            }
        }

        isSetupComplete = true;
    }

    public void OnPieceSnapped()
    {
        correctlyPlacedPieces++;
        if (correctlyPlacedPieces >= totalPieces)
        {
            GameManager.Instance.finalScore = GameManager.Instance.isPracticeMode ? 0 : Mathf.CeilToInt(timeRemaining * 10f);
            EndGame();
        }
    }

    public void QuitEarly()
    {
        if (!isGameActive) return;
        GameManager.Instance.playerQuitEarly = true;
        GameManager.Instance.finalScore = GameManager.Instance.isPracticeMode ? 0 : Mathf.CeilToInt(timeRemaining * 2.5f);
        EndGame();
    }

    private void EndGame()
    {
        isGameActive = false;
        GameManager.Instance.LoadScene("exit");
    }

    private void SetupHintOverlay(string picture)
    {
        if (hintOverlayImage != null)
        {
            // Specifically loads your standalone hint images ending in '- copy'
            Texture2D tex = Resources.Load<Texture2D>($"{picture} - copy");
            if (tex != null)
            {
                hintOverlayImage.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            }
        }
        if (hintOverlay != null) hintOverlay.SetActive(false);
    }

    public void ShowHint() => StartCoroutine(TriggerHintRoutine());

    private IEnumerator TriggerHintRoutine()
    {
        if (hintOverlay == null) yield break;
        hintOverlay.transform.SetAsLastSibling();
        hintOverlay.SetActive(true);
        if (hintButton != null) hintButton.interactable = false;

        yield return new WaitForSeconds(5f);

        hintOverlay.SetActive(false);
        if (hintButton != null) hintButton.interactable = true;
    }
}