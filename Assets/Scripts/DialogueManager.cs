using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI textDisplay;
    public GameObject dialoguePanel;
    public float typingSpeed = 0.05f;

    [Header("Кнопки управления")]
    public Button nextButton;
    public Button closeButton;

    private static bool _wasInitialMonologueShown = false;
    private static bool _wasHallTutorialTriggered = false; // Предохранитель для плашки
    private Coroutine displayCoroutine;

    private string[] _dialogueLines;
    private int _currentLineIndex;

    // Новые флаги для Холла
    private bool _isHallIntroActive = false;

    void Start()
    {
        if (nextButton != null) nextButton.gameObject.SetActive(false);
        if (closeButton != null) closeButton.gameObject.SetActive(false);

        if (nextButton != null) nextButton.onClick.AddListener(ShowNextLine);
        if (closeButton != null) closeButton.onClick.AddListener(CloseDialogue);
    }

    // Однострочный монолог (мысли у предметов)
    public void ShowMonologue(string text, bool isOneTime = false)
    {
        if (isOneTime && _wasInitialMonologueShown) return;
        if (isOneTime) _wasInitialMonologueShown = true;

        _isHallIntroActive = false;
        _dialogueLines = null;

        dialoguePanel.SetActive(true);
        nextButton.gameObject.SetActive(false);
        closeButton.gameObject.SetActive(false);

        if (displayCoroutine != null) StopCoroutine(displayCoroutine);
        displayCoroutine = StartCoroutine(TypeText(text));
    }

    // СТАРЫЙ МЕТОД (ОСТАЛСЯ ДЛЯ ДРУГИХ СЦЕН БЕЗ ИЗМЕНЕНИЙ)
    public void StartTutorial(string[] lines)
    {
        _isHallIntroActive = false;
        _dialogueLines = lines;
        _currentLineIndex = 0;
        dialoguePanel.SetActive(true);
        DisplayLine();
    }

    // НОВЫЙ МЕТОД (СТРОГО ДЛЯ СТАРТА ХОЛЛА С ПОСЛЕДУЮЩЕЙ ПЛАШКОЙ)
    public void StartHallIntro(string[] lines)
    {
        _isHallIntroActive = true;
        _dialogueLines = lines;
        _currentLineIndex = 0;
        dialoguePanel.SetActive(true);
        DisplayLine();
    }

    private void DisplayLine()
    {
        if (displayCoroutine != null) StopCoroutine(displayCoroutine);

        nextButton.gameObject.SetActive(false);
        closeButton.gameObject.SetActive(false);

        displayCoroutine = StartCoroutine(TypeLineText(_dialogueLines[_currentLineIndex]));
    }

    IEnumerator TypeLineText(string line)
    {
        textDisplay.text = "";
        foreach (char letter in line.ToCharArray())
        {
            textDisplay.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        if (_currentLineIndex < _dialogueLines.Length - 1)
        {
            nextButton.gameObject.SetActive(true);
        }
        else
        {
            closeButton.gameObject.SetActive(true);
        }
    }

    IEnumerator TypeText(string line)
    {
        textDisplay.text = "";
        foreach (char letter in line.ToCharArray())
        {
            textDisplay.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        closeButton.gameObject.SetActive(true);
    }

    public void ShowNextLine()
    {
        _currentLineIndex++;
        DisplayLine();
    }

    public void CloseDialogue()
    {
        if (displayCoroutine != null) StopCoroutine(displayCoroutine);
        if (textDisplay != null) textDisplay.text = "";

        if (nextButton != null) nextButton.gameObject.SetActive(false);
        if (closeButton != null) closeButton.gameObject.SetActive(false);

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            MonoBehaviour move = player.GetComponent("Movement") as MonoBehaviour;
            if (move != null) move.enabled = true;
        }

        // Если закрылся именно стартовый монолог Холла и плашка еще не вызывалась
        if (_isHallIntroActive && !_wasHallTutorialTriggered)
        {
            _isHallIntroActive = false;
            _wasHallTutorialTriggered = true; // Блокируем повторы

            HallManager hall = FindFirstObjectByType<HallManager>();
            if (hall != null)
            {
                hall.StartTutorialSequence();
            }
        }

        if (dialoguePanel != null) dialoguePanel.SetActive(false);
    }

    public static void ResetDialogueStatus()
    {
        _wasInitialMonologueShown = false;
        _wasHallTutorialTriggered = false;
    }
}