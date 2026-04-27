using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI textDisplay;
    public GameObject dialoguePanel;
    public float typingSpeed = 0.05f;

    // Статическая переменная для ОДНОРАЗОВЫХ монологов (например, пробуждение)
    private static bool _wasInitialMonologueShown = false;

    private Coroutine displayCoroutine;

    // ОБНОВЛЕННЫЙ МЕТОД: теперь он принимает решение, блокировать ли повторный показ
    public void ShowMonologue(string text, bool isOneTime = false)
    {
        // Если мы пометили монолог как одноразовый И он уже был показан — ничего не делаем
        if (isOneTime && _wasInitialMonologueShown) return;

        // Если это был одноразовый монолог, запоминаем это
        if (isOneTime) _wasInitialMonologueShown = true;

        // Включаем панель (теперь она точно появится!)
        dialoguePanel.SetActive(true);

        if (displayCoroutine != null) StopCoroutine(displayCoroutine);
        displayCoroutine = StartCoroutine(TypeText(text));
    }

    IEnumerator TypeText(string line)
    {
        textDisplay.text = "";
        foreach (char letter in line.ToCharArray())
        {
            textDisplay.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    public void CloseDialogue()
    {
        if (displayCoroutine != null) StopCoroutine(displayCoroutine);

        if (textDisplay != null) textDisplay.text = "";

        if (dialoguePanel != null) dialoguePanel.SetActive(false);
    }

    // Метод для сброса памяти (если нужно)
    public static void ResetDialogueStatus()
    {
        _wasInitialMonologueShown = false;
    }
}