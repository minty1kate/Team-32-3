using UnityEngine;

public class KitchenLevelController : MonoBehaviour
{
    [Header("Объекты для удаления после прохождения")]
    public GameObject grandma;      // mother_stand_back_0
    public GameObject potTrigger;   // Pot_Trigger (плита)
    public GameObject noteTrigger;  // Note_Trigger (холодильник)
    
    public GameObject doorBlocker;  // Невидимая стена (если есть)

    void Start()
    {
        // Проверяем: суп готов?
        if (MiniGameManager.isSoupDone)
        {
            DisableInteraction();
        }
    }

    void DisableInteraction()
    {
        // 1. Убираем бабушку
        if (grandma != null) grandma.SetActive(false);

        // 2. Выключаем триггер плиты (мини-игра больше не откроется и "E" не появится)
        if (potTrigger != null) potTrigger.SetActive(false);

        // 3. Выключаем триггер записки (если хочешь, чтобы её тоже нельзя было читать в конце)
        if (noteTrigger != null) noteTrigger.SetActive(false);

        // 4. Открываем дверь
        if (doorBlocker != null) doorBlocker.SetActive(false);

        Debug.Log("Уровень завершен: триггеры и бабушка удалены.");
    }
}