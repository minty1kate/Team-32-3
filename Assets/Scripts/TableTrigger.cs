using UnityEngine;

public class TableTrigger : MonoBehaviour
{
    [Header("Настройки UI")]
    [SerializeField] private GameObject interactPrompt; // Спрайт/текст "[E] Осмотреть" над столом
    [SerializeField] private GameObject miniGameCanvas;  // Canvas самой мини-игры

    [Header("Ссылки на игрока")]
    [SerializeField] private MonoBehaviour playerMovementScript; // Скрипт перемещения вашего героя
    [Header("Сюжетные настройки")]
    [SerializeField] private bool isMirrorTrigger = false;

    private bool canInteract = false;
    private bool isMiniGameActive = false;

    private void Start()
    {
        // В начале игры всё выключено
        if (interactPrompt != null) interactPrompt.SetActive(false);
        if (miniGameCanvas != null) miniGameCanvas.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Проверяем, что подошел именно игрок (можно настроить тег "Player")
        if (collision.CompareTag("Player") && !isMiniGameActive)
        {
            canInteract = true;
            if (interactPrompt != null) interactPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canInteract = false;
            if (interactPrompt != null) interactPrompt.SetActive(false);
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
        
        // Отключаем управление героем, чтобы он не ходил во время игры
        if (playerMovementScript != null) playerMovementScript.enabled = false;

        // Включаем интерфейс мини-игры
        if (miniGameCanvas != null) miniGameCanvas.SetActive(true);
    }

    // Этот метод мы вызовем из второго скрипта, когда игра закончится
    public void EndMiniGameAndOpenNewspaper()
    {
        if (miniGameCanvas != null) miniGameCanvas.SetActive(false);
        
        if (playerMovementScript != null) 
        {
            playerMovementScript.enabled = true;
        }

        Debug.Log("Мини-игра пройдена! Управление возвращено игроку.");
    }
}
