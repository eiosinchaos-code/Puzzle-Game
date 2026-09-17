using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector3 initialPosition;
    private CanvasGroup canvasGroup;
    private bool isSnapped = false;
    private PuzzleID pieceID;
    private RectTransform rectTransform;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
        pieceID = GetComponent<PuzzleID>();
        rectTransform = GetComponent<RectTransform>();
    }

    public void InitCardPosition()
    {
        initialPosition = transform.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isSnapped) return;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isSnapped) return;
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isSnapped) return;
        canvasGroup.blocksRaycasts = true;

        GameObject targetPH = GameObject.Find("PH" + pieceID.id);

        if (targetPH != null)
        {
            float distance = Vector3.Distance(transform.position, targetPH.transform.position);

            // Distance tolerance scales with piece width
            float tolerance = (rectTransform != null) ? (rectTransform.rect.width * 0.6f) : 50f;

            if (distance < tolerance)
            {
                transform.position = targetPH.transform.position;
                isSnapped = true;
                this.enabled = false;

                Object.FindAnyObjectByType<ManagePuzzleGame>()?.OnPieceSnapped();
                return;
            }
        }

        transform.position = initialPosition;
    }
}