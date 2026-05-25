using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class SafePicture : MonoBehaviour
{
    [Header("Настройки подсветки")]
    [SerializeField] private Color highlightColor = Color.gray;

    [Header("Ссылки")]
    public MazeManager mazeManager; // Ссылка на менеджер лабиринта

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
            if (mazeManager != null)
            {
                mazeManager.OpenMaze(); // Мгновенно открываем сейф и лабиринт
            }
            return; // Выходим из метода, чтобы обычный код ниже не мешал
        }
        // --------------------------------

        // Обычная логика игры (по кнопке E и после смерти экзорциста)
        if (_isPlayerInside && _canInteract && Input.GetKeyDown(KeyCode.E))
        {
            if (mazeManager != null)
            {
                mazeManager.OpenMaze();
            }
        }
    }
    //void Update()
    //{
    //    // Если игрок рядом, экзорцист мертв и нажата кнопка E
    //    if (_isPlayerInside && _canInteract && Input.GetKeyDown(KeyCode.E))
    //    {
    //        if (mazeManager != null)
    //        {
    //            mazeManager.OpenMaze(); // Открываем лабиринт через менеджер
    //        }
    //    }
    //}

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