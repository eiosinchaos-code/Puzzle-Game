using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ManagePuzzleGame : MonoBehaviour
{
    public Image piece;
    public Image placeHolder;

    [Header("Hint System")]
    [SerializeField] private GameObject hintOverlay;
    [SerializeField] private Image hintOverlayImage;
    [SerializeField] private Button hintButton;

    private float timer;
    private bool cardsShuffled = false;
    public bool isSetupComplete { get; private set; } = false;

    [Header("Grid Layout Settings")]
    [SerializeField] private float totalAreaWidth = 400f;  // Total width available for grid
    [SerializeField] private float totalAreaHeight = 400f; // Total height available for grid

    private float phWidth;
    private float phHeight;

    private int nbRows = 5;
    private int nbColumns = 5;
    private int totalPieces;
    private int correctlyPlacedPieces = 0;
    private string selectedPictureName = "lion";

    void Start()
    {
        if (GameSettings.Instance != null)
        {
            nbRows = GameSettings.Instance.gridRows;
            nbColumns = GameSettings.Instance.gridCols;
            selectedPictureName = GameSettings.Instance.selectedPictureName;
        }

        totalPieces = nbRows * nbColumns;

        // Dynamically calculate piece size based on grid density
        phWidth = totalAreaWidth / nbColumns;
        phHeight = totalAreaHeight / nbRows;

        // Load Hint Image (Loads Texture2D first to ensure full picture renders even if sliced)
        if (hintOverlayImage != null)
        {
            Texture2D fullTexture = Resources.Load<Texture2D>(selectedPictureName);
            if (fullTexture == null)
            {
                fullTexture = Resources.Load<Texture2D>(selectedPictureName + "/" + selectedPictureName);
            }
            if (fullTexture == null)
            {
                fullTexture = Resources.Load<Texture2D>(selectedPictureName + " - copy");
            }

            if (fullTexture != null)
            {
                // Reconstruct full sprite from entire texture dimensions
                Sprite fullSprite = Sprite.Create(
                    fullTexture,
                    new Rect(0, 0, fullTexture.width, fullTexture.height),
                    new Vector2(0.5f, 0.5f)
                );
                hintOverlayImage.sprite = fullSprite;
            }
        }

        if (hintOverlay != null)
            hintOverlay.SetActive(false);

        CreatePlaceHolders();
        CreatePieces();
    }

    void Update()
    {
        if (!cardsShuffled)
        {
            timer += Time.deltaTime;
            if (timer >= 2f)
            {
                ShufflePieces();
                cardsShuffled = true;
                isSetupComplete = true;
            }
        }
    }

    public void CreatePlaceHolders()
    {
        Vector3 centerPosition = GameObject.Find("rightSide").transform.position;
        Transform canvasTransform = GameObject.Find("Canvas").transform;

        for (int i = 0; i < totalPieces; i++)
        {
            float row = i % nbColumns;
            float column = i / nbColumns;

            Vector3 phPosition = new Vector3(
                centerPosition.x + phWidth * (row - (nbColumns - 1) / 2f),
                centerPosition.y - phHeight * (column - (nbRows - 1) / 2f),
                centerPosition.z
            );

            Image ph = Instantiate(placeHolder, phPosition, Quaternion.identity);
            ph.name = "PH" + (i + 1);
            ph.transform.SetParent(canvasTransform);

            RectTransform rect = ph.GetComponent<RectTransform>();
            if (rect != null) rect.sizeDelta = new Vector2(phWidth, phHeight);

            PuzzleID puzzleID = ph.gameObject.GetComponent<PuzzleID>();
            if (puzzleID == null) puzzleID = ph.gameObject.AddComponent<PuzzleID>();
            puzzleID.id = i + 1;
        }
    }

    public void CreatePieces()
    {
        Vector3 centerPosition = GameObject.Find("leftSide").transform.position;
        Transform canvasTransform = GameObject.Find("Canvas").transform;

        string resourcePath = selectedPictureName + "/" + selectedPictureName;
        Sprite[] allSprites = Resources.LoadAll<Sprite>(resourcePath);

        if (allSprites == null || allSprites.Length == 0)
        {
            allSprites = Resources.LoadAll<Sprite>(selectedPictureName);
        }

        for (int i = 0; i < totalPieces; i++)
        {
            float row = i % nbColumns;
            float column = i / nbColumns;

            Vector3 phPosition = new Vector3(
                centerPosition.x + phWidth * (row - (nbColumns - 1) / 2f),
                centerPosition.y - phHeight * (column - (nbRows - 1) / 2f),
                centerPosition.z
            );

            Image ph = Instantiate(piece, phPosition, Quaternion.identity);
            ph.name = "Piece" + (i + 1);
            ph.transform.SetParent(canvasTransform);

            RectTransform rect = ph.GetComponent<RectTransform>();
            if (rect != null) rect.sizeDelta = new Vector2(phWidth, phHeight);

            PuzzleID puzzleID = ph.gameObject.GetComponent<PuzzleID>();
            if (puzzleID == null) puzzleID = ph.gameObject.AddComponent<PuzzleID>();
            puzzleID.id = i + 1;

            if (allSprites != null && i < allSprites.Length)
            {
                ph.GetComponent<Image>().sprite = allSprites[i];
            }
        }
    }

    void ShufflePieces()
    {
        int[] newArray = new int[totalPieces];
        for (int i = 0; i < totalPieces; i++) newArray[i] = i;

        for (int t = 0; t < totalPieces; t++)
        {
            int tmp = newArray[t];
            int r = Random.Range(t, totalPieces);
            newArray[t] = newArray[r];
            newArray[r] = tmp;
        }

        Vector3 centerPosition = GameObject.Find("leftSide").transform.position;

        for (int i = 0; i < totalPieces; i++)
        {
            float row = newArray[i] % nbColumns;
            float column = newArray[i] / nbColumns;

            GameObject g = GameObject.Find("Piece" + (i + 1));
            if (g != null)
            {
                Vector3 newPosition = new Vector3(
                    centerPosition.x + phWidth * (row - (nbColumns - 1) / 2f),
                    centerPosition.y - phHeight * (column - (nbRows - 1) / 2f),
                    centerPosition.z
                );

                g.transform.position = newPosition;
                g.GetComponent<DragAndDrop>()?.InitCardPosition();
            }
        }
    }

    public void ShowHint()
    {
        if (hintOverlay != null)
        {
            StartCoroutine(TriggerHintRoutine());
        }
    }

    private IEnumerator TriggerHintRoutine()
    {
        hintOverlay.transform.SetAsLastSibling();
        hintOverlay.SetActive(true);
        if (hintButton != null) hintButton.interactable = false;

        yield return new WaitForSeconds(5f);

        hintOverlay.SetActive(false);
        if (hintButton != null) hintButton.interactable = true;
    }

    public void OnPieceSnapped()
    {
        correctlyPlacedPieces++;
        if (correctlyPlacedPieces >= totalPieces)
        {
            Object.FindAnyObjectByType<PuzzleGameController>()?.CompletePuzzle();
        }
    }
}