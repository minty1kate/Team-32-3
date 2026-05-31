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
            dialogueManager.ShowMonologue("Герой\nГолова раскалывается... Почему в доме так темно? (Нажмите C, чтобы встать)", true);
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

        // --- БЛОК ОБУЧЕНИЯ ---
        if (dialogueManager != null)
        {
            // Сначала закрываем старый монолог "Голова раскалывается..."
            dialogueManager.CloseDialogue();

            // Подготавливаем фразы для обучения
            string[] tutorialLines = {
                "Используй AWSD для перемещения по дому.",
                "Чтобы взаимодействовать с предметом или подобрать его, нажми E.",
                "Взаимодействовать можно только с теми предметами, которые подсвечены темным цветом.",
                "Нажми I, чтобы открыть инвентарь.",
                "Нажми Q, чтобы посмотреть текущие задачи."
            };

            // Запускаем последовательное обучение
            dialogueManager.StartTutorial(tutorialLines);
        }
        // ----------------------
        TaskManager tm = FindObjectOfType<TaskManager>();
        if (tm != null) tm.CompleteCurrentTask();
        Debug.Log("Персонаж проснулся, обучение запущено!");
        this.enabled = false;

    }
}