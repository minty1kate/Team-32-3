using UnityEngine;
using UnityEngine.Events; // Позволяет назначать действия прямо в Инспекторе Unity

public class InteractableKitchenObject : MonoBehaviour
{
    [Header("UI Подсказка")]
    public GameObject interactHint; // Сюда перетащим наш Interact_Hint из Canvas

    [Header("Событие при нажатии E")]
    public UnityEvent onInteract; // Что произойдет при нажатии

    private bool isPlayerInRange = false;

    void Start()
    {
        // Прячем подсказку при старте игры
        if (interactHint != null)
        {
            interactHint.SetActive(false);
        }
    }

    // Когда игрок входит в зону триггера
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            if (interactHint != null) interactHint.SetActive(true); // Показываем подсказку
        }
    }

    // Когда игрок выходит из зоны
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (interactHint != null) interactHint.SetActive(false); // Прячем подсказку
        }
    }

    void Update()
    {
        // Если игрок рядом и нажал английскую "E"
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            onInteract.Invoke(); // Выполняем то, что настроим в Инспекторе
        }
    }
}
