using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI; // Обязательно добавьте для работы с кнопками

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI textDisplay;
    public GameObject dialoguePanel;
    public float typingSpeed = 0.05f;

    [Header("Кнопки обучения")]
    public Button nextButton;
    public Button closeButton;

    private static bool _wasInitialMonologueShown = false;
    private Coroutine displayCoroutine;

    // Переменные для работы очереди сообщений (обучения)
    private string[] _dialogueLines;
    private int _currentLineIndex;

    void Start()
    {
        // Скрываем кнопки при старте
        if (nextButton != null) nextButton.gameObject.SetActive(false);
        if (closeButton != null) closeButton.gameObject.SetActive(false);

        // Привязываем функции к кнопкам
        if (nextButton != null) nextButton.onClick.AddListener(ShowNextLine);
        if (closeButton != null) closeButton.onClick.AddListener(CloseDialogue);
    }

    // СТАРЫЙ МЕТОД (сохраняем, чтобы ничего не сломалось)
    public void ShowMonologue(string text, bool isOneTime = false)
    {
        if (isOneTime && _wasInitialMonologueShown) return;
        if (isOneTime) _wasInitialMonologueShown = true;

        dialoguePanel.SetActive(true);

        // Скрываем кнопки управления для обычных монологов
        nextButton.gameObject.SetActive(false);
        closeButton.gameObject.SetActive(false);

        if (displayCoroutine != null) StopCoroutine(displayCoroutine);
        displayCoroutine = StartCoroutine(TypeText(text));
    }

    // НОВЫЙ МЕТОД: для запуска серии сообщений (обучения)
    public void StartTutorial(string[] lines)
    {
        _dialogueLines = lines;
        _currentLineIndex = 0;
        dialoguePanel.SetActive(true);
        DisplayTutorialLine();
    }

    private void DisplayTutorialLine()
    {
        if (displayCoroutine != null) StopCoroutine(displayCoroutine);

        nextButton.gameObject.SetActive(false);
        closeButton.gameObject.SetActive(false);

        displayCoroutine = StartCoroutine(TypeTutorialText(_dialogueLines[_currentLineIndex]));
    }

    IEnumerator TypeTutorialText(string line)
    {
        textDisplay.text = "";
        foreach (char letter in line.ToCharArray())
        {
            textDisplay.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        // Показываем кнопки ПОСЛЕ того, как текст напечатался
        if (_currentLineIndex < _dialogueLines.Length - 1)
        {
            nextButton.gameObject.SetActive(true); // Если есть еще текст - кнопка Далее
        }
        else
        {
            closeButton.gameObject.SetActive(true); // Если текст закончился - кнопка Закрыть
        }
    }

    // Вспомогательный TypeText для обычных монологов
    IEnumerator TypeText(string line)
    {
        textDisplay.text = "";
        foreach (char letter in line.ToCharArray())
        {
            textDisplay.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    public void ShowNextLine()
    {
        _currentLineIndex++;
        DisplayTutorialLine();
    }

    public void CloseDialogue()
    {
        if (displayCoroutine != null) StopCoroutine(displayCoroutine);
        if (textDisplay != null) textDisplay.text = "";

        if (nextButton != null) nextButton.gameObject.SetActive(false);
        if (closeButton != null) closeButton.gameObject.SetActive(false);

        if (dialoguePanel != null) dialoguePanel.SetActive(false);
    }

    public static void ResetDialogueStatus()
    {
        _wasInitialMonologueShown = false;
    }
}