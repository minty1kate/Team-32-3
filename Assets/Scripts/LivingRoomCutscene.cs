using UnityEngine;

public class LivingRoomCutscene : MonoBehaviour
{
    [Header("Связи")]
    public DialogueManager dialogueManager;

    [Header("Реплики монолога при входе")]
    [TextArea(3, 5)]
    public string[] livingRoomLines = new string[]
    {
        "Я так и знал... Но что с его лицом? Это же мой младший брат... Или то чудовище, что приняло его облик?",

        "Oн действительно сидит там и безостановочно бормочет свои проклятые заклинания. Ему плевать на меня, он хочет окончательно осквернить мой дом!",

        "Я буквально чувствую, как его слова душат меня, воздух становится тяжелым. Хватит это терпеть.",

        "Нужно подойти ближе. Если я вырву страницы из его проклятой книги и начну швырять его игрушки, я смогу напугать его и прервать этот кошмар..."
    };

    // Статическая переменная, чтобы монолог срабатывал только один раз за всю игру
    private static bool _wasLivingRoomDialogueShown = false;

    void Start()
    {
        // Если DialogueManager не перетащили руками, пытаемся найти его на сцене автоматически
        if (dialogueManager == null)
        {
            dialogueManager = FindObjectOfType<DialogueManager>();
        }

        // Проверяем, заходили ли мы уже сюда
        if (!_wasLivingRoomDialogueShown)
        {
            if (dialogueManager != null && livingRoomLines.Length > 0)
            {
                // Запускаем твой метод цепочки реплик
                dialogueManager.StartTutorial(livingRoomLines);

                // Отмечаем, что монолог воспроизведен
                _wasLivingRoomDialogueShown = true;
            }
            else if (dialogueManager == null)
            {
                Debug.LogError("LivingRoomCutscene: Не найден DialogueManager в сцене Гостиной!");
            }
        }
    }
}