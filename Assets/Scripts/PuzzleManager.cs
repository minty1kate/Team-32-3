using UnityEngine;
using UnityEngine.UI;
using System.Collections; // Обязательно для задержки (IEnumerator)

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance;

    [Header("Настройки панелей")]
    public GameObject puzzlePanel;
    public Button closeButton;
    public float finishDelay = 1.5f; // Настройка времени задержки в секундах

    [Header("Связи")]
    public DragAndDrop[] allPieces;
    public DialogueManager dialogueManager;
    public TaskManager taskManager;
    public InventoryManager inventory;

    [Header("Визуал целого зеркала")]
    public Image puzzleMirrorImage;
    public Sprite fixedMirrorSprite;
    public SpriteRenderer wallMirrorRenderer;

    private bool _isFinished = false;

    void Awake() { Instance = this; }

    void Start()
    {
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
            // ЗАМЕНЕНО: Теперь вызываем задержку через корутину
            StartCoroutine(FinishWithDelay());
        }
    }

    // НОВЫЙ МЕТОД: Корутина для ожидания
    private IEnumerator FinishWithDelay()
    {
        _isFinished = true; // Сразу ставим флаг, чтобы не вызывать победу дважды

        // 1. Сначала меняем спрайт зеркала внутри пазла на целое
        if (puzzleMirrorImage != null) puzzleMirrorImage.sprite = fixedMirrorSprite;

        // 2. Убираем осколки
        foreach (var p in allPieces) p.gameObject.SetActive(false);
        if (inventory != null)
        {
            inventory.ClearInventory();
        }
        // ЖДЕМ указанное время
        yield return new WaitForSeconds(finishDelay);

        // Теперь выполняем остальное
        FinishPuzzle();
    }

    void FinishPuzzle()
    {
        // 3. Вычеркиваем задачу
        if (taskManager != null) taskManager.CompleteCurrentTask();

        // 4. Показываем диалоговое окно
        string text = "Это же наше семейное фото.. Но почему меня здесь нет? Я помню, я стоял прямо между ними... Меня будто выжгли из этой картины... Но зачем?";
        dialogueManager.ShowMonologue(text, false);

        // 5. Показываем кнопку "Закрыть"
        if (closeButton != null) closeButton.gameObject.SetActive(true);
    }

    public void ExitPuzzle()
    {
        //if (taskManager != null) taskManager.CompleteCurrentTask();
        // 1. Закрываем само окно пазла
        puzzlePanel.SetActive(false);

        // 2. Обновляем зеркало на стене (делаем целым)
        if (wallMirrorRenderer != null && fixedMirrorSprite != null)
        {
            wallMirrorRenderer.sprite = fixedMirrorSprite;
        }

        // 3. Готовим сюжетные реплики про брата-чтеца
        string[] storyLines = {
        "Этот шепот из гостиной... Кажется, это голос брата.",
        "Он снова сидит с той старой книгой. Чтец... Мне всегда казалось это странным.",
        "Звучит не как чтение сказки, а как ритуал призыва. Мне кажется, он призывает демонов.",
        "Нужно немедленно пойти в гостиную. Я должен остановить его, пока не стало поздно."
    };

        // 4. Запускаем диалог через твой метод StartTutorial
        if (dialogueManager != null)
        {
            dialogueManager.StartTutorial(storyLines);
        }

        // 5. Включаем курсор, чтобы игрок мог нажимать на кнопки "Далее"
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}