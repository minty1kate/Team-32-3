using UnityEngine;

public class SafeScreenController : MonoBehaviour
{
    [Header("Элементы Сейфа")]
    public GameObject safeBackground;     // Картинка самого сейфа
    public GameObject safeTriggerButton;  // Прозрачная кнопка на сейфе

    [Header("Элементы Лабиринта")]
    public GameObject mazeGameplayGroup;  // Объект, в котором лежат стены, точка и финиш

    // Этот метод мы привяжем к прозрачной кнопке на сейфе
    public void ClickOnSafeInterface()
    {
        // ЭТА СТРОКА ДЛЯ ПРОВЕРКИ КЛИКА
        Debug.LogWarning("!!! МЕТОД КЛИКА ВЫЗВАН УСПЕШНО !!!");

        if (safeBackground != null) safeBackground.SetActive(false);
        if (safeTriggerButton != null) safeTriggerButton.SetActive(false);

        if (mazeGameplayGroup != null)
        {
            mazeGameplayGroup.SetActive(true);
            Debug.Log("Лабиринт включен!");
        }
        else
        {
            Debug.LogError("ОШИБКА: Поле mazeGameplayGroup ПУСТОЕ в инспекторе!");
        }
    }

    // Этот метод нужно вызывать, когда игрок ТОЛЬКО ЧТО подошел к картине и нажал E
    public void ResetToSafeView()
    {
        // Возвращаем вид закрытого сейфа
        if (safeBackground != null) safeBackground.SetActive(true);
        if (safeTriggerButton != null) safeTriggerButton.SetActive(true);

        // Прячем лабиринт, пока игрок не кликнет на сейф
        if (mazeGameplayGroup != null) mazeGameplayGroup.SetActive(false);
    }
}