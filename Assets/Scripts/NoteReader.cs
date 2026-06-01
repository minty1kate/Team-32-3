using UnityEngine;
using System.Collections;

public class NoteReader : MonoBehaviour
{
    [Header("UI Записки (на весь экран)")]
    public GameObject bigNoteUI; // Сюда перетащим Note_Big

    private bool isShowing = false;
    private bool isCooldown = false; // Защита от случайного двойного срабатывания

    // Вызывается из InteractableKitchenObject
    public void ShowNote()
    {
        // Открываем только если она закрыта и нет кулдауна
        if (!isShowing && !isCooldown)
        {
            bigNoteUI.SetActive(true); // Показываем
            isShowing = true;
            StartCoroutine(ActionCooldown());
        }
    }

    void Update()
    {
        // Если записка открыта, кулдаун прошел, и игрок жмет E, Esc или Пробел
        if (isShowing && !isCooldown && (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Space)))
        {
            bigNoteUI.SetActive(false); // Прячем
            isShowing = false;
            StartCoroutine(ActionCooldown()); // Снова ждем долю секунды, чтобы она тут же не открылась
        }
    }

    // Та самая микро-задержка
    private IEnumerator ActionCooldown()
    {
        isCooldown = true;
        yield return new WaitForSeconds(0.2f); // Ждем 0.2 секунды
        isCooldown = false;
    }
}