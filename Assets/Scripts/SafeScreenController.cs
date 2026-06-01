using UnityEngine;

public class SafeScreenController : MonoBehaviour
{
    [Header("Элементы Сейфа")]
    public GameObject safeBackground;     // Картинка самого сейфа
    public GameObject safeTriggerButton;  // Прозрачная кнопка на сейфе

    [Header("Элементы Лабиринта")]
    public GameObject mazeGameplayGroup;  // Объект, в котором лежат стены, точка и финиш

    [Header("Настройки Звука")]
    [SerializeField] private AudioClip safeClickSound; // Сюда перетащим аудиофайл (например, щелчок или скрежет)

    // Этот метод мы привяжем к прозрачной кнопке на сейфе
    public void ClickOnSafeInterface()
    {
        // ЭТА СТРОКА ДЛЯ ПРОВЕРКИ КЛИКА
        Debug.LogWarning("!!! МЕТОД КЛИКА ВЫЗВАН УСПЕШНО !!!");

        // ВОСПРОИЗВЕДЕНИЕ ЗВУКА КЛИКА
        PlayClickSound();

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

    // Вспомогательный метод для проигрывания аудиоклипа
    private void PlayClickSound()
    {
        if (safeClickSound != null)
        {
            // Воспроизводит звук в точке нахождения камеры (чтобы игрок его четко слышал)
            if (Camera.main != null)
            {
                AudioSource.PlayClipAtPoint(safeClickSound, Camera.main.transform.position);
            }
            else
            {
                // Если главной камеры нет, воспроизводим в точке самого объекта
                AudioSource.PlayClipAtPoint(safeClickSound, transform.position);
            }
        }
        else
        {
            Debug.LogWarning("Предупреждение: В SafeScreenController не назначен safeClickSound!");
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