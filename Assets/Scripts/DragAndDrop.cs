using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector2 initialAnchoredPosition;
    private Transform initialParent;
    private CanvasGroup canvasGroup;
    private bool isSnapped = false;
    private PuzzleID pieceID;
    private RectTransform rectTransform;
    private Canvas canvas;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();

        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        // Fetch PuzzleID
        pieceID = GetComponent<PuzzleID>();
    }

    public void InitCardPosition()
    {
        initialAnchoredPosition = rectTransform.anchoredPosition;
        initialParent = transform.parent;
    }

    private void Start()
    {
        if (initialParent == null)
        {
            InitCardPosition();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isSnapped) return;

        canvasGroup.blocksRaycasts = false;
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isSnapped) return;

        Camera cam = (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay) ? eventData.pressEventCamera : null;

        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, eventData.position, cam, out Vector3 worldPoint))
        {
            transform.position = worldPoint;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isSnapped) return;

        canvasGroup.blocksRaycasts = true;

        // Fallback check to prevent NullReferenceException if Awake didn't catch pieceID
        if (pieceID == null)
        {
            pieceID = GetComponent<PuzzleID>();
        }

        if (pieceID == null)
        {
            Debug.LogError($"[DragAndDrop] Missing PuzzleID component on {gameObject.name}!");
            ResetToStartPosition();
            return;
        }

        GameObject targetPH = GameObject.Find("PH" + pieceID.id);

        if (targetPH != null)
        {
            Camera cam = (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay) ? eventData.pressEventCamera : null;

            Vector2 pieceScreenPos = RectTransformUtility.WorldToScreenPoint(cam, rectTransform.position);
            Vector2 phScreenPos = RectTransformUtility.WorldToScreenPoint(cam, targetPH.transform.position);

            float distance = Vector2.Distance(pieceScreenPos, phScreenPos);
            float snapThreshold = rectTransform.rect.width * (canvas != null ? canvas.scaleFactor : 1f) * 1.2f;

            if (distance <= snapThreshold || Vector3.Distance(transform.position, targetPH.transform.position) < 100f)
            {
                // Snap piece into placeholder container
                transform.SetParent(targetPH.transform, false);
                rectTransform.anchoredPosition = Vector2.zero;

                isSnapped = true;
                this.enabled = false;

                Object.FindAnyObjectByType<PuzzleGameplayManager>()?.OnPieceSnapped();
                return;
            }
        }

        ResetToStartPosition();
    }

    private void ResetToStartPosition()
    {
        transform.SetParent(initialParent, false);
        rectTransform.anchoredPosition = initialAnchoredPosition;
    }
}