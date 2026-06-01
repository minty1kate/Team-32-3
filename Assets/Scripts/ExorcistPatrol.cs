using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ExorcistPatrol : MonoBehaviour
{
    public Transform[] waypoints;
    public float baseSpeed = 2f; // Начальная скорость
    public float speedIncreasePerCandle = 0.5f; // На сколько увеличивается скорость за свечу

    [Header("Настройки Спрайтов")]
    public Sprite normalSprite;
    public Sprite painSprite;
    public Sprite deathSprite;

    [Header("Эффекты боли (UI)")]
    public GameObject painFlashPanel; // Красная панель на весь экран внутри Canvas

    [Header("Финальный монолог после смерти")]
    [TextArea(2, 5)] // Делает поля в инспекторе удобными для ввода текста
    public string[] deathDialogueLines; // Сюда в инспекторе пишем реплики ГГ после победы

    private float _currentSpeed;
    private int currentPointIndex = 0;
    private SpriteRenderer spriteRenderer;
    private bool _isDistracted = false; // Блокирует движение во время боли/смерти
    private Color _originalColor;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        _originalColor = spriteRenderer.color;
        _currentSpeed = baseSpeed;
        if (normalSprite != null) spriteRenderer.sprite = normalSprite;
    }

    void Update()
    {
        // Если экзорцист горит, умирает или испытывает боль — он не идет по точкам
        if (_isDistracted || waypoints.Length == 0) return;

        Transform targetPoint = waypoints[currentPointIndex];
        transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, _currentSpeed * Time.deltaTime);

        if (targetPoint.position.x > transform.position.x)
            spriteRenderer.flipX = false;
        else if (targetPoint.position.x < transform.position.x)
            spriteRenderer.flipX = true;

        if (Vector2.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            currentPointIndex = (currentPointIndex + 1) % waypoints.Length;
        }
    }

    // Метод вызывается из HallManager, когда тушится свеча (кроме последней)
    public void TakeDamage(int extinguishedCount)
    {
        // Увеличиваем скорость на основе количества потушенных свечей
        _currentSpeed = baseSpeed + (extinguishedCount * speedIncreasePerCandle);

        // Запускаем корутину боли
        StartCoroutine(PainReactionRoutine());
    }

    // Метод вызывается из HallManager, когда тушится 5-я свеча
    public void Die()
    {
        _isDistracted = true;
        StartCoroutine(DeathRoutine());
    }

    // Сбросить скорость экзорциста к начальной (вызывается при поимке игрока)
    public void ResetExorcist()
    {
        _isDistracted = false;
        _currentSpeed = baseSpeed;
        currentPointIndex = 0;
        spriteRenderer.color = _originalColor;
        if (normalSprite != null) spriteRenderer.sprite = normalSprite;
    }

    private IEnumerator PainReactionRoutine()
    {
        _isDistracted = true;

        // Меняем спрайт на боль
        if (painSprite != null) spriteRenderer.sprite = painSprite;

        // Включаем красную рамку на UI
        if (painFlashPanel != null) painFlashPanel.SetActive(true);

        // Ждем, например, 0.6 секунды
        yield return new WaitForSeconds(0.6f);

        // Выключаем красную рамку
        if (painFlashPanel != null) painFlashPanel.SetActive(false);

        // Возвращаем обычный спрайт и разрешаем ходить
        if (normalSprite != null) spriteRenderer.sprite = normalSprite;

        _isDistracted = false;
    }

    private IEnumerator DeathRoutine()
    {
        _isDistracted = true;

        // Меняем спрайт на смерть
        if (deathSprite != null) spriteRenderer.sprite = deathSprite;
        if (painFlashPanel != null) painFlashPanel.SetActive(true); // Сильная вспышка при смерти

        float duration = 2.0f; // Время горения (2 секунды)
        float elapsed = 0f;

        // Эффект горения: экзорцист быстро мигает красным/оранжевым цветом
        while (elapsed < duration)
        {
            spriteRenderer.color = (spriteRenderer.color == _originalColor) ? Color.red : Color.orange;

            // Выключим вспышку боли чуть раньше, через 0.3 сек
            if (elapsed > 0.3f && painFlashPanel != null && painFlashPanel.activeSelf)
            {
                painFlashPanel.SetActive(false);
            }

            yield return new WaitForSeconds(0.1f);
            elapsed += 0.1f;
        }

        // 1. ПЕРЕКЛЮЧАЕМ ЗАДАЧУ (Победить экзорциста -> Найти ключ от подвала)
        TaskManager taskManager = FindFirstObjectByType<TaskManager>();
        if (taskManager != null)
        {
            taskManager.CompleteCurrentTask();
        }

        // 2. РАЗБЛОКИРУЕМ СЕЙФ / КАРТИНУ (Возвращаем старую логику)
        MazeManager maze = FindFirstObjectByType<MazeManager>();
        if (maze != null)
        {
            maze.ExorcistIsDead(); // Передаем сигнал в мини-игру сейфа
        }

        // 3. ЗАПУСКАЕМ ДИАЛОГ ПОСЛЕ СМЕРТИ ЭКЗОРЦИСТА
        DialogueManager dialogueManager = FindFirstObjectByType<DialogueManager>();
        if (dialogueManager != null && deathDialogueLines != null && deathDialogueLines.Length > 0)
        {
            dialogueManager.StartTutorial(deathDialogueLines);
        }

        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Если экзорцист уже умирает, он не может поймать игрока
        if (collision.CompareTag("Player") && spriteRenderer.sprite != deathSprite)
        {
            Player_Movement move = collision.GetComponent<Player_Movement>();
            if (move != null && move.isHidden)
            {
                return;
            }

            // Находим HallManager на сцене и заставляем его сбросить все свечи
            HallManager hallManager = FindObjectOfType<HallManager>();
            if (hallManager != null)
            {
                hallManager.ResetAllCandles();
            }

            // Возвращаем экзорциста в нормальное состояние
            ResetExorcist();

            // Логика перемещения игрока
            if (DoorSpawnPoint.spawnPosition != Vector2.zero)
            {
                collision.transform.position = DoorSpawnPoint.spawnPosition;
            }
            else
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }
}