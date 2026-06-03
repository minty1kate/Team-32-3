using UnityEngine;

public class TableTrigger : MonoBehaviour
{
    [Header("Настройки UI")]
    [SerializeField] private GameObject interactPrompt; // Спрайт/текст "[E] Осмотреть" над столом
    [SerializeField] private GameObject miniGameCanvas;  // Canvas самой мини-игры

    [Header("Настройки подсветки (Включение/Выключение)")]
    [SerializeField] private SpriteRenderer targetRenderer; // Ссылка на SpriteRenderer подсветки
    private bool _hasRenderer;

    [Header("Ссылки на игрока")]
    [SerializeField] private MonoBehaviour playerMovementScript; // Скрипт перемещения вашего героя

    [Header("Сюжетные настройки")]
    [SerializeField] private bool isMirrorTrigger = false;

    [Header("Настройки звука мини-игры")]
    [SerializeField] private AudioSource audioSource; // Ссылка на компонент AudioSource
    [SerializeField] private AudioClip miniGameLoopSound;
    [SerializeField] public AudioSource backgroundAudioSource;
    [SerializeField] private AudioClip mainSceneTheme;

    private bool canInteract = false;
    private bool isMiniGameActive = false;

    private void Start()
    {
        _hasRenderer = targetRenderer != null;
        if (_hasRenderer)
        {
            // Скрываем спрайт (выключаем галочку) в самом начале игры
            targetRenderer.enabled = false;
        }

        if (backgroundAudioSource != null && mainSceneTheme != null)
        {
            backgroundAudioSource.clip = mainSceneTheme; // Вставляем "кассету" в "магнитофон"
            backgroundAudioSource.loop = true;          // Делаем звук бесконечным
            backgroundAudioSource.Play();               // Включаем
        }

        // В начале игры всё выключено
        if (interactPrompt != null) interactPrompt.SetActive(false);
        if (miniGameCanvas != null) miniGameCanvas.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Проверяем, что подошел именно игрок
        if (collision.CompareTag("Player") && !isMiniGameActive)
        {
            // Если это зеркало и газета еще не пройдена — ничего не делаем
            if (isMirrorTrigger && !GhostHandGame.isNewspaperPassed) return;

            canInteract = true;
            if (interactPrompt != null) interactPrompt.SetActive(true);

            // Показываем спрайт (включаем галочку) при приближении игрока
            if (_hasRenderer)
            {
                targetRenderer.enabled = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canInteract = false;
            if (interactPrompt != null) interactPrompt.SetActive(false);

            // Прячем спрайт назад (выключаем галочку), когда игрок отошел
            if (_hasRenderer && !isMiniGameActive)
            {
                targetRenderer.enabled = false;
            }
        }
    }

    private void Update()
    {
        // Если игрок в зоне и нажимает E
        if (canInteract && Input.GetKeyDown(KeyCode.E) && !isMiniGameActive)
        {
            if (isMirrorTrigger && !GhostHandGame.isNewspaperPassed)
            {
                Debug.Log("Герой еще не знает, что он призрак. Зеркало заблокировано.");
                return; // Игнорируем нажатие кнопки E
            }

            StartMiniGame();
        }
    }

    private void StartMiniGame()
    {
        isMiniGameActive = true;
        canInteract = false;

        if (interactPrompt != null) interactPrompt.SetActive(false);

        // Отключаем видимость спрайта на время открытого окна мини-игры
        if (_hasRenderer)
        {
            targetRenderer.enabled = false;
        }

        // Отключаем управление героем, чтобы он не ходил во время игры
        if (playerMovementScript != null) playerMovementScript.enabled = false;

        // Включаем интерфейс мини-игры
        if (miniGameCanvas != null) miniGameCanvas.SetActive(true);

        if (audioSource != null && miniGameLoopSound != null)
        {
            audioSource.clip = miniGameLoopSound;
            audioSource.loop = true; // Включаем зацикливание
            audioSource.Play();      // Включаем воспроизведение
        }
    }

    // Этот метод вызывается из внешнего скрипта, когда игра закончится
    public void EndMiniGameAndOpenNewspaper()
    {
        isMiniGameActive = false; // Сбрасываем флаг активности мини-игры
        if (miniGameCanvas != null) miniGameCanvas.SetActive(false);

        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = true;
        }

        if (audioSource != null)
        {
            audioSource.Stop(); // Полностью выключаем звук
        }

        Debug.Log("Мини-игра пройдена! Управление возвращено игроку.");
    }

    public GameObject GetPlayerObject()
    {
        if (playerMovementScript != null)
        {
            // Берем игровой объект, на котором висит скрипт ходьбы (это и есть наш Player 1)
            return playerMovementScript.gameObject;
        }
        return null;
    }
}