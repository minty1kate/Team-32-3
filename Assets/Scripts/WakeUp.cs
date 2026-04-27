using UnityEngine;

public class WakeUp : MonoBehaviour
{
    [Header("Настройки спрайтов")]
    public Sprite lyingSprite;
    public Sprite standingSprite;

    [Header("Система диалогов")]
    public DialogueManager dialogueManager;

    private SpriteRenderer _sr;
    private MonoBehaviour _moveScript;

    // СТАТИЧЕСКАЯ ПЕРЕМЕННАЯ: запоминает, проснулся ли герой за всё время игры
    private static bool _alreadyWokenUp = false;

    private bool _isLying = true;

    void Start()
    {
        _sr = GetComponent<SpriteRenderer>();
        _moveScript = GetComponent("Movement") as MonoBehaviour;

        // ПРОВЕРКА: Если мы уже вставали раньше
        if (_alreadyWokenUp)
        {
            SetAlreadyStanding();
        }
        else
        {
            // Если это самый первый запуск игры (герой еще спит)
            SetInitialLyingState();
        }
    }

    // Состояние, когда герой уже проснулся (для повторных заходов)
    void SetAlreadyStanding()
    {
        _isLying = false;
        if (standingSprite != null) _sr.sprite = standingSprite;
        if (_moveScript != null) _moveScript.enabled = true;

        // --- ДОБАВЬТЕ ЭТУ СТРОКУ НИЖЕ ---
        if (dialogueManager != null)
        {
            dialogueManager.CloseDialogue(); // Закрываем окно, если уже проснулись
        }
        // -------------------------------

        // Скрипт WakeUp больше не нужен, отключаем его сразу
        this.enabled = false;
    }

    // Состояние в самом начале игры
    void SetInitialLyingState()
    {
        if (lyingSprite != null) _sr.sprite = lyingSprite;
        if (_moveScript != null) _moveScript.enabled = false;

        if (dialogueManager != null)
        {
            dialogueManager.ShowMonologue("Голова раскалывается... Почему в доме так темно? (Нажмите C, чтобы встать)", true);
        }
    }

    void Update()
    {
        if (_isLying && Input.GetKeyDown(KeyCode.C))
        {
            StandUp();
        }
    }

    void StandUp()
    {
        _isLying = false;
        _alreadyWokenUp = true; // ЗАПОМИНАЕМ, что встали

        if (standingSprite != null) _sr.sprite = standingSprite;
        if (_moveScript != null) _moveScript.enabled = true;

        if (dialogueManager != null)
        {
            dialogueManager.CloseDialogue();
        }

        Debug.Log("Персонаж проснулся впервые!");
        this.enabled = false;
    }
}