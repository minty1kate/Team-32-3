using UnityEngine;

public class HallStartDialogue : MonoBehaviour
{
    private DialogueManager _dialogueManager;
    private TaskManager _taskManager;
    private MonoBehaviour _playerMovement;

    void Start()
    {
        _dialogueManager = FindFirstObjectByType<DialogueManager>();
        _taskManager = FindFirstObjectByType<TaskManager>();

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            _playerMovement = player.GetComponent("Movement") as MonoBehaviour;
        }

        if (_taskManager != null)
        {
            _taskManager.SetSceneTasks(new string[] {
                "Победить экзорциста",
                "Найти ключ от подвала"
            });
        }

        TriggerHallDialogue();
    }

    void TriggerHallDialogue()
    {
        if (_dialogueManager != null)
        {
            string[] hallLines = new string[]
            {
                "Он добрался до прихожей... Везде расставил свои свечи, от этого запаха уже кружится голова. Он точно не уйдет, пока не избавится от меня.",
                "Я видела его лицо сквозь полумрак. В его глазах столько холодной, пугающей злости... Он ведь даже не пытается понять, кто я. Для него я просто враг.",
                "Мне нужно как-то потушить эти свечи, иначе его барьер заблокирует меня здесь навсегда.",
                "Единственное место, куда он еще не успел дотянуться — это подвал. Нужно прорваться мимо него и запереться там, пока не стало слишком поздно."
            };

            if (_playerMovement != null) _playerMovement.enabled = false;

            // Вызываем специальный метод для старта холла
            _dialogueManager.StartHallIntro(hallLines);
        }
    }
}