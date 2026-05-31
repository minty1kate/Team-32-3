using UnityEngine;

public class MirrorHighlight : MonoBehaviour
{

    [Header("Настройки подсветки")]
    public SpriteRenderer mirrorRenderer;
    public Color highlightColor = Color.gray;
    private Color _originalColor;

    [Header("Интерактив")]
    public GameObject puzzlePanel;

    // ИСПРАВЛЕНО: Изменили Inventory на InventoryManager
    public InventoryManager inventory;
    public int requiredPieces = 3;

    private bool _canInteract = false;

    void Start()
    {
        if (mirrorRenderer == null) mirrorRenderer = GetComponent<SpriteRenderer>();
        _originalColor = mirrorRenderer.color;

        if (puzzlePanel != null) puzzlePanel.SetActive(false);
    }

    void Update()
    {
        if (_canInteract && Input.GetKeyDown(KeyCode.E))
        {
            OpenPuzzle();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // ИСПРАВЛЕНО: Проверяем ItemsCount вместо pieces
            if (inventory != null && inventory.ItemsCount >= requiredPieces)
            {
                _canInteract = true;
                mirrorRenderer.color = highlightColor;
                Debug.Log("Зеркало подсвечено, можно чинить!");
            }
            else
            {
                Debug.Log("Осколков мало. Нужно: " + requiredPieces + ", а есть: " + (inventory != null ? inventory.ItemsCount : 0));
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _canInteract = false;
            mirrorRenderer.color = _originalColor;
        }
    }

    void OpenPuzzle()
    {
        if (puzzlePanel != null)
        {
            puzzlePanel.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}