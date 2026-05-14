using UnityEngine;
using UnityEngine.UI; // Добавь это для работы с UI Image

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance;

    [Header("Настройки панелей")]
    public GameObject puzzlePanel;
    public Button closeButton; // Ссылка на кнопку закрытия в UI

    [Header("Связи")]
    public DragAndDrop[] allPieces;
    public DialogueManager dialogueManager;
    public TaskManager taskManager; // Ссылка на менеджер задач

    [Header("Визуал целого зеркала")]
    public Image puzzleMirrorImage; // Картинка рамы ВНУТРИ пазла
    public Sprite fixedMirrorSprite; // Спрайт целого зеркала
    public SpriteRenderer wallMirrorRenderer; // Спрайт рамы НА СТЕНЕ (в комнате)

    private bool _isFinished = false;

    void Awake() { Instance = this; }

    void Start()
    {
        // Скрываем кнопку закрытия в начале, чтобы нельзя было выйти раньше времени
        if (closeButton != null) closeButton.gameObject.SetActive(false);
    }

    public void OpenPuzzle()
    {
        puzzlePanel.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        _isFinished = false;
    }

    public void CheckWin()
    {
        if (_isFinished) return;

        int count = 0;
        foreach (var p in allPieces)
        {
            if (p.isLocked) count++;
        }

        if (count == 3)
        {
            FinishPuzzle();
        }
    }

    void FinishPuzzle()
    {
        // 1. Сначала меняем спрайт зеркала внутри пазла на целое
        if (puzzleMirrorImage != null) puzzleMirrorImage.sprite = fixedMirrorSprite;

        // 2. Убираем осколки, чтобы они не мешались (ведь зеркало уже целое)
        foreach (var p in allPieces) p.gameObject.SetActive(false);

        // 3. Вычеркиваем задачу в списке (Q)
        if (taskManager != null) taskManager.CompleteCurrentTask();

        // 4. Показываем диалоговое окно с красивым текстом
        // Текст теперь будет ПОВЕРХ зеркала, если ты поменял порядок в Hierarchy
        string text = "Оно целое... ни единой трещины. Но почему я не вижу своего отражения?";
        dialogueManager.ShowMonologue(text, false);

        // 5. Показываем кнопку "Закрыть" прямо в меню пазла
        // Игрок прочитает текст, полюбуется собранным зеркалом и нажмет её сам
        if (closeButton != null) closeButton.gameObject.SetActive(true);
    }

    // Метод для кнопки "Закрыть"
    // Метод для кнопки "Закрыть"
    public void ExitPuzzle()
    {
        // 1. Закрываем панель пазла
        puzzlePanel.SetActive(false);

        // 2. Закрываем диалоговое окно
        if (dialogueManager != null)
        {
            dialogueManager.CloseDialogue();
        }

        // 3. Меняем спрайт зеркала на стене
        if (wallMirrorRenderer != null && fixedMirrorSprite != null)
        {
            wallMirrorRenderer.sprite = fixedMirrorSprite;
        }

        // ИЗМЕНЕНО: Оставляем мышку видимой для игрока
        Cursor.visible = true;                       // Мышка видна
        Cursor.lockState = CursorLockMode.None;      // Мышка свободно двигается

        _isFinished = true;
    }
}