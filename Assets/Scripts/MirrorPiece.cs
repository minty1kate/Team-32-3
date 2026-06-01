using UnityEngine;

public class MirrorPiece : MonoBehaviour
{
    [Header("Ссылки")]
    public Sprite pieceIcon;        // Иконка для инвентаря
    public GameObject hintObject;

    [Header("Настройки подсветки")]
    public Color darkColor = new Color(0.5f, 0.5f, 0.5f, 1f);

    [Header("Звуковые эффекты")]
    [SerializeField] private AudioClip pickupSound; // Сюда перетащи аудиофайл подбора

    private SpriteRenderer _sr;
    private Color _originalColor;
    private bool _canInteract = false;

    void Start()
    {
        _sr = GetComponent<SpriteRenderer>();
        if (_sr != null) _originalColor = _sr.color;
        if (hintObject != null) hintObject.SetActive(false);
    }

    void Update()
    {
        if (_canInteract && Input.GetKeyDown(KeyCode.E))
        {
            PickUp();
        }
    }

    private void PickUp()
    {
        // Воспроизводим звук в точке нахождения детальки (не прервется при Destroy)
        if (pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
        }

        // 1. Добавляем в инвентарь (исправлено на FindAnyObjectByType)
        InventoryManager inv = FindAnyObjectByType<InventoryManager>();
        if (inv != null) inv.AddItem(pieceIcon);

        // 2. Увеличиваем счетчик в менеджере задач
        TaskManager tm = FindAnyObjectByType<TaskManager>();
        if (tm != null)
        {
            tm.IncrementMirrorPieces();
        }

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _canInteract = true;
            if (hintObject != null) hintObject.SetActive(true);
            if (_sr != null) _sr.color = darkColor;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _canInteract = false;
            if (_sr != null) _sr.color = _originalColor;
            if (hintObject != null) hintObject.SetActive(false);
        }
    }
}