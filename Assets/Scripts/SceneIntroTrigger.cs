using UnityEngine;

public class SceneIntroTrigger : MonoBehaviour
{
    [Header("Система Диалогов")]
    [SerializeField] private DialogueManager dialogueManager;

    [Header("Объект Игрока")]
    [SerializeField] private GameObject playerObject;

    void Start()
    {
        // Проверяем, привязаны ли ссылки
        if (dialogueManager != null)
        {
            // Массив строк для монолога (разделен, чтобы перелистывался кнопкой "Далее")
            string[] introLines = new string[]
            {
                "Сюда они не доберутся...",
                "Здесь тихо...",
                "Здесь... безопасно?"
            };

            // Блокируем движение игрока на время стартового монолога
            if (playerObject != null)
            {
                Player_Movement movement = playerObject.GetComponent<Player_Movement>();
                if (movement != null) movement.enabled = false;
            }

            // Настраиваем кнопку закрытия диалога, чтобы она возвращала управление игроку
            if (dialogueManager.closeButton != null)
            {
                dialogueManager.closeButton.onClick.RemoveAllListeners();
                dialogueManager.closeButton.onClick.AddListener(dialogueManager.CloseDialogue);
                dialogueManager.closeButton.onClick.AddListener(EnablePlayerMovement);
            }

            // Запускаем показ через существующий метод StartTutorial
            dialogueManager.StartTutorial(introLines);
        }
        else
        {
            Debug.LogError("Dialogue Manager не привязан в скрипте SceneIntroTrigger!");
        }
    }

    private void EnablePlayerMovement()
    {
        if (playerObject != null)
        {
            Player_Movement movement = playerObject.GetComponent<Player_Movement>();
            if (movement != null)
            {
                movement.enabled = true;
            }
        }
    }
}