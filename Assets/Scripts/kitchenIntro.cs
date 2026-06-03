using UnityEngine;

public class KitchenIntroTrigger : MonoBehaviour
{
    [TextArea(3, 5)] public string[] introLines;

    void Start()
    {
        DialogueManager dialogueManager = FindFirstObjectByType<DialogueManager>();

        if (dialogueManager != null && introLines != null && introLines.Length > 0)
        {
            // Блокируем движение игрока перед стартом (если нужно)
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                MonoBehaviour move = player.GetComponent("Movement") as MonoBehaviour;
                if (move != null) move.enabled = false;
            }

            // Запускаем монолог через твой менеджер
            dialogueManager.StartHallIntro(introLines);
        }
    }
}