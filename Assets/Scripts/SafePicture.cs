using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class SafePicture : MonoBehaviour
{
    [Header("Настройки подсветки")]
    [SerializeField] private Color highlightColor = Color.gray;

    [Header("Ссылки")]
    public MazeManager mazeManager; // Ссылка на менеджер лабиринта
    public DialogueManager dialogueManager; // Ссылка на менеджер диалогов

    [Header("Текст монолога")]
    [SerializeField]
    private string[] safeLines = new string[]
    {
        "Ого... Не знал, что за этой картиной скрывается потайной сейф. Что же такого секретного можно здесь прятать?",
        "И как мне его открыть?.. Похоже, механизм заклинило, придется пробиваться через эту защиту."
    };

    private SpriteRenderer _sr;
    private Color _originalColor;
    private bool _isPlayerInside = false;
    private bool _canInteract = false; // Можно ли взаимодействовать (умрет ли экзорцист)

    void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _originalColor = _sr.color;

        BoxCollider2D col = GetComponent<BoxCollider2D>();
        col.isTrigger = true; // Делаем коллайдер триггером
    }

    // Этот метод вызовет экзорцист перед тем, как исчезнуть
    public void UnlockInteraction()
    {
        _canInteract = true;

        // Если игрок УЖЕ стоит у картины в момент смерти экзорциста — сразу подсвечиваем её
        if (_isPlayerInside)
        {
            _sr.color = highlightColor;
        }
    }

    void Update()
    {
        // --- ВРЕМЕННЫЙ ЧИТ ДЛЯ ТЕСТОВ ---
        // Если игрок просто подошел к картине и нажал кнопку T (английскую)
        if (_isPlayerInside && Input.GetKeyDown(KeyCode.T))
        {
            _canInteract = true; // Принудительно разрешаем взаимодействие
            InteractWithPicture();
            return; // Выходим из метода, чтобы обычный код ниже не мешал
        }
        // --------------------------------

        // Обычная логика игры (по кнопке E и после смерти экзорциста)
        if (_isPlayerInside && _canInteract && Input.GetKeyDown(KeyCode.E))
        {
            InteractWithPicture();
        }
    }

    private void InteractWithPicture()
    {
        // 1. Открываем лабиринт/сейф через менеджер
        if (mazeManager != null)
        {
            mazeManager.OpenMaze();
        }

        // 2. Одновременно запускаем диалоговое окно поверх сейфа
        if (dialogueManager != null && safeLines.Length > 0)
        {
            // Используем метод StartTutorial, который умеет работать с очередью строк и кнопками Далее/Закрыть
            dialogueManager.StartTutorial(safeLines);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _isPlayerInside = true;

            // Подсвечиваем картину при приближении, только если экзорцист уже побежден
            if (_canInteract)
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
            _sr.color = _originalColor; // Возвращаем обычный цвет картины
        }
    }
}