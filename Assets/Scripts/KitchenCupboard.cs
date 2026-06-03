using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class KitchenInteractiveObject : MonoBehaviour
{
    public enum ObjectType { Cupboard, FridgeNote, BoilingPot }

    [Header("Общие настройки объекта")]
    public ObjectType objectType;
    public SpriteRenderer visualRenderer;

    [Header("Настройки для ШКАФА")]
    public Sprite itemIcon;
    public string itemDescription;

    [Header("Настройки для ЗАПИСКИ")]
    public Sprite noteIcon;
    public GameObject noteUIPanel;

    [Header("Настройки для ПЛИТЫ")]
    public GameObject potUIPanel;       // Панель плиты на весь экран
    public Transform potSlotsContainer; // Контейнер (Grid/Horizontal Layout) куда переедут предметы
    public GameObject uiSlotPrefab;     // Префаб пустой ячейки UI (с дочерней картинкой Icon)

    private static int _globalQuestStage = 0;
    private static readonly List<string> RuinedSequence = new List<string> { "Meat", "Soda", "Potato", "Salt", "Soap" };
    private static List<string> _currentInput = new List<string>();

    private bool _isPlayerInside = false;
    private bool _isEmptyCupboard = false;
    private bool _isNoteUIOpen = false;
    private bool _isNoteReadAndClosed = false;
    private bool _isPotUIOpen = false;

    private InventoryManager _inventory;
    private DialogueManager _dialogue;
    private NotificationManager _notification;

    void Start()
    {
        _inventory = FindFirstObjectByType<InventoryManager>();
        _dialogue = FindFirstObjectByType<DialogueManager>();
        _notification = FindFirstObjectByType<NotificationManager>();

        if (visualRenderer == null) visualRenderer = GetComponent<SpriteRenderer>();

        if (objectType == ObjectType.FridgeNote && noteUIPanel != null) noteUIPanel.SetActive(false);
        if (objectType == ObjectType.BoilingPot && potUIPanel != null) potUIPanel.SetActive(false);
    }

    void Update()
    {
        if (!_isPlayerInside || !Input.GetKeyDown(KeyCode.E)) return;

        switch (objectType)
        {
            case ObjectType.Cupboard:
                HandleCupboardInteraction();
                break;

            case ObjectType.FridgeNote:
                HandleNoteInteraction();
                break;

            case ObjectType.BoilingPot:
                HandlePotInteraction();
                break;
        }
    }

    // ==========================================
    // 1. ШКАФЫ
    // ==========================================
    void HandleCupboardInteraction()
    {
        if (_globalQuestStage > 0 || _isEmptyCupboard) return;

        if (_inventory != null && itemIcon != null)
        {
            _inventory.AddItem(itemIcon);
            if (_notification != null) _notification.ShowNotification(itemDescription);

            _isEmptyCupboard = true;
            RemoveHighlight();

            if (_inventory.ItemsCount >= 5)
            {
                _globalQuestStage = 1;

                if (_dialogue != null)
                {
                    _dialogue.StartTutorial(new string[] { "«Шкафы пусты. Что там за записка на холодильнике?»" });
                }
            }
        }
    }

    // ==========================================
    // 2. ЛИСТОК
    // ==========================================
    void HandleNoteInteraction()
    {
        if (_globalQuestStage == 0 || _isNoteReadAndClosed) return;

        if (!_isNoteUIOpen)
        {
            _isNoteUIOpen = true;
            if (noteUIPanel != null) noteUIPanel.SetActive(true);
            RemoveHighlight();
        }
        else
        {
            _isNoteUIOpen = false;
            _isNoteReadAndClosed = true;
            _globalQuestStage = 2;

            if (noteUIPanel != null) noteUIPanel.SetActive(false);

            if (_inventory != null && noteIcon != null) _inventory.AddItem(noteIcon);

            if (_dialogue != null)
            {
                string ggThought = "«Значит, это её идеальный рецепт... Чтобы изгнать эту тварь, я должен нарушить правила и осквернить её варево. Сделаю всё строго наоборот».";
                _dialogue.StartTutorial(new string[] { ggThought });
            }
        }
    }

    // ==========================================
    // 3. ПЛИТА
    // ==========================================
    void HandlePotInteraction()
    {
        if (_globalQuestStage < 2)
        {
            if (_notification != null) _notification.ShowNotification("Сначала нужно понять, как устроен этот яд. Посмотри рецепт.");
            return;
        }

        if (_globalQuestStage == 3) return;

        if (!_isPotUIOpen)
        {
            _isPotUIOpen = true;
            if (potUIPanel != null) potUIPanel.SetActive(true);
            RemoveHighlight();

            PopulatePotItems();
        }
        else
        {
            _isPotUIOpen = false;
            if (potUIPanel != null) potUIPanel.SetActive(false);
        }
    }

    void PopulatePotItems()
    {
        if (_inventory == null || potSlotsContainer == null || uiSlotPrefab == null) return;

        foreach (Transform child in potSlotsContainer)
        {
            Destroy(child.gameObject);
        }

        List<Sprite> collectedItems = _inventory.GetAllItems();

        foreach (Sprite item in collectedItems)
        {
            if (noteIcon != null && item == noteIcon) continue;

            GameObject newSlot = Instantiate(uiSlotPrefab, potSlotsContainer);
            newSlot.name = item.name;

            // МЕНЯЕМ СПРАЙТ ПРЯМО НА САМОМ ПРЕФАБЕ (БЕЗ GETCHILD)
            Image slotImage = newSlot.GetComponent<Image>();
            if (slotImage != null)
            {
                slotImage.sprite = item;
                slotImage.color = Color.white;
            }

            Button slotButton = newSlot.GetComponent<Button>();
            if (slotButton != null)
            {
                slotButton.onClick.AddListener(() => DropItemIntoPot(item));
            }
        }

        _inventory.ClearInventory();
    }

    public void DropItemIntoPot(Sprite selectedItemIcon)
    {
        if (objectType != ObjectType.BoilingPot || _globalQuestStage != 2) return;

        string itemName = selectedItemIcon.name;
        _currentInput.Add(itemName);
        int currentIndex = _currentInput.Count - 1;

        if (itemName == RuinedSequence[currentIndex])
        {
            if (_notification != null) _notification.ShowNotification("Жижа чернеет и пенится... Отлично.");

            Transform thrownSlot = potSlotsContainer.Find(itemName);
            if (thrownSlot != null) Destroy(thrownSlot.gameObject);

            if (_currentInput.Count == RuinedSequence.Count)
            {
                _globalQuestStage = 3;
                if (potUIPanel != null) potUIPanel.SetActive(false);
                if (_notification != null) _notification.ShowNotification("Чернота хлынула через край! Тварь в ужасе бежит!");
                RemoveHighlight();
            }
        }
        else
        {
            _currentInput.Clear();
            if (potUIPanel != null) potUIPanel.SetActive(false);
            _isPotUIOpen = false;

            if (_inventory != null)
            {
                KitchenInteractiveObject[] allObjects = FindObjectsByType<KitchenInteractiveObject>(FindObjectsSortMode.None);
                foreach (var obj in allObjects)
                {
                    if (obj.objectType == ObjectType.Cupboard && obj.itemIcon != null)
                    {
                        _inventory.AddItem(obj.itemIcon);
                    }
                }
                if (noteIcon != null) _inventory.AddItem(noteIcon);
            }

            if (_dialogue != null)
            {
                _dialogue.ShowMonologue("Живая энергия супа подавила меня. Порядок сброшен! Нужно сделать всё НАОБОРОТ...", false, false);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag("Player")) return;
        _isPlayerInside = true;

        bool shouldHighlight = false;

        if (objectType == ObjectType.Cupboard && _globalQuestStage == 0 && !_isEmptyCupboard) shouldHighlight = true;
        if (objectType == ObjectType.FridgeNote && _globalQuestStage == 1 && !_isNoteReadAndClosed) shouldHighlight = true;
        if (objectType == ObjectType.BoilingPot && _globalQuestStage == 2 && !_isPotUIOpen) shouldHighlight = true;

        if (shouldHighlight && visualRenderer != null)
        {
            visualRenderer.color = new Color(0.3f, 0.3f, 0.3f, 1f);
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (!col.CompareTag("Player")) return;
        _isPlayerInside = false;
        RemoveHighlight();
    }

    void RemoveHighlight()
    {
        if (visualRenderer != null) visualRenderer.color = Color.white;
    }
}