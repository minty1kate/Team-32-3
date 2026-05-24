using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class Candle : MonoBehaviour
{
    public Sprite litSprite;
    public Sprite unlitSprite;
    public HallManager hallManager;

    [SerializeField] private Color highlightColor = Color.gray;
    private SpriteRenderer _sr;
    private Color _originalColor;
    private bool _isPlayerInside = false;
    private bool _isLit = true;

    void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _originalColor = _sr.color;

        BoxCollider2D col = GetComponent<BoxCollider2D>();
        col.isTrigger = true;

        _sr.sprite = litSprite;
    }

    void Update()
    {
        if (_isPlayerInside && _isLit && Input.GetKeyDown(KeyCode.E))
        {
            Extinguish();
        }
    }

    public void Extinguish()
    {
        _isLit = false;
        _sr.sprite = unlitSprite;
        _sr.color = _originalColor; // Убираем подсветку после тушения

        if (hallManager != null)
            hallManager.CandleExtinguished();
    }

    // Этот метод вызывается из HallManager, когда экзорцист ловит игрока
    public void ResetCandle()
    {
        _isLit = true;
        _sr.sprite = litSprite;

        // Если игрок умер прямо внутри триггера свечи, возвращаем подсветку, 
        // иначе — оставляем оригинальный цвет
        if (_isPlayerInside)
        {
            _sr.color = highlightColor;
        }
        else
        {
            _sr.color = _originalColor;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _isPlayerInside = true;

            // Подсвечиваем только если свеча ещё горит
            if (_isLit)
            {
                _sr.color = highlightColor;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _isPlayerInside = false;
            _sr.color = _originalColor; // Снимаем подсветку в любом случае
        }
    }
}