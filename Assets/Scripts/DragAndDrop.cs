using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragAndDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public RectTransform slotTarget; // Сюда в инспекторе перетащи соответствующий Slot
    public float snapDistance = 50f; // Расстояние «магнита»
    public Sprite pieceIcon;
    private RectTransform _rectTransform;
    private CanvasGroup _canvasGroup;
    private Vector3 _startPosition;
    public bool isLocked = false; // Публичная переменная, чтобы менеджер видел успех

    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = gameObject.GetComponent<CanvasGroup>();
        if (_canvasGroup == null) _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        _startPosition = _rectTransform.localPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isLocked) return;
        _canvasGroup.alpha = 0.7f;
        _canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isLocked) return;
        _rectTransform.anchoredPosition += eventData.delta; // Двигаем за мышкой
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isLocked) return;
        _canvasGroup.alpha = 1f;
        _canvasGroup.blocksRaycasts = true;

        // Проверяем, близко ли мы к слоту
        float distance = Vector2.Distance(_rectTransform.anchoredPosition, slotTarget.anchoredPosition);

        if (distance < snapDistance)
        {
            _rectTransform.anchoredPosition = slotTarget.anchoredPosition;
            isLocked = true;

            // ВСТАВЛЯЙ СЮДА:
            GetComponent<Image>().raycastTarget = false;

            PuzzleManager.Instance.CheckWin();
        }
        else
        {
            _rectTransform.localPosition = _startPosition;
        }
    }
}