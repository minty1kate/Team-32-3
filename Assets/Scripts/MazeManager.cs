using UnityEngine;
using System.Collections;

public class MazeManager : MonoBehaviour
{
    [Header("Ссылки на UI")]
    public GameObject mazePanel;
    public MazeBall mazeBallScript;

    [Space(10)]
    public GameObject openSafePanel; // Панель открытого сейфа с ключом внутри и кнопкой "Взять"

    [Header("Настройки Ключа для Инвентаря")]
    public InventoryManager inventoryManager; // Ссылка на менеджер инвентаря
    public TaskManager taskManager;           // Ссылка на менеджер задач
    public Sprite keyIcon;                    // Картинка ключа, которая отобразится в инвентаре

    [Header("Система Диалогов")]
    public DialogueManager dialogueManager;   // Ссылка на менеджер диалогов

    [Header("Ссылки на сцене")]
    public GameObject keyItem;
    public SafePicture safePicture;
    public SpriteRenderer pictureSpriteRenderer; // Рендерер картины на стене, чтобы сменить ей спрайт
    public Sprite openSafeWallSprite;            // Спрайт открытого сейфа для стены холла

    // Перетащи сюда скрипт управления своего ГГ
    public MonoBehaviour playerMovementScript;

    void Start()
    {
        if (mazePanel != null) mazePanel.SetActive(false);
        if (openSafePanel != null) openSafePanel.SetActive(false);
        if (keyItem != null) keyItem.SetActive(false);
    }

    public void ExorcistIsDead()
    {
        if (safePicture != null) safePicture.UnlockInteraction();
    }

    public void OpenMaze()
    {
        if (mazePanel != null)
        {
            mazePanel.SetActive(true);

            if (playerMovementScript != null) playerMovementScript.enabled = false;

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            SafeScreenController screenCtrl = mazePanel.GetComponent<SafeScreenController>();
            if (screenCtrl != null) screenCtrl.ResetToSafeView();

            if (mazeBallScript != null) mazeBallScript.ResetToStart();
        }
    }

    public void WinMaze()
    {
        StartCoroutine(WinSequenceCoroutine());
    }

    private IEnumerator WinSequenceCoroutine()
    {
        if (mazePanel != null) mazePanel.SetActive(false);

        yield return new WaitForSeconds(0.0f);

        if (openSafePanel != null)
        {
            openSafePanel.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    // Этот метод вешаем на кнопку "Взять" в UI
    // Замени этот метод в MazeManager.cs
    public void TakeKeyAndCloseEverything()
    {
        if (inventoryManager != null && keyIcon != null) inventoryManager.AddItem(keyIcon);
        if (taskManager != null) taskManager.CompleteCurrentTask();
        if (openSafePanel != null) openSafePanel.SetActive(false);
        if (keyItem != null) keyItem.SetActive(true);
        if (pictureSpriteRenderer != null && openSafeWallSprite != null) pictureSpriteRenderer.sprite = openSafeWallSprite;

        // Запускаем монолог
        TriggerFinalHallDialogue();

        // Запускаем фикс мыши
        StartCoroutine(ForceShowCursorRoutine());
    }

    private IEnumerator ForceShowCursorRoutine()
    {
        // Ждем один кадр, пока все скрипты отработают свои Update()
        yield return null;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void TriggerFinalHallDialogue()
    {
        if (dialogueManager != null)
        {
            string[] finalLines = new string[]
            {
            "Отлично, ключ у меня. Он такой холодный и тяжелый...",
            "Барьер экзорциста исчез вместе с ним, теперь проход свободен.",
            "Пора спускаться в подвал и закончить со всем этим. Надеюсь, там я найду ответы."
            };

            // Блокируем движение ГГ
            if (playerMovementScript != null) playerMovementScript.enabled = false;

            // Принудительно освобождаем курсор перед стартом текста
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            // Запускаем диалог
            dialogueManager.StartTutorial(finalLines);

            // ХАК ДЛЯ КУРСОРA: Очищаем старые функции кнопки и вешаем новую чистую логику
            if (dialogueManager.closeButton != null)
            {
                dialogueManager.closeButton.onClick.RemoveAllListeners();
                // Возвращаем стандартное закрытие панели диалога
                dialogueManager.closeButton.onClick.AddListener(dialogueManager.CloseDialogue);
                // Добавляем разблокировку игрока и возврат мыши
                dialogueManager.closeButton.onClick.AddListener(EnablePlayerMovement);
            }
        }
        else
        {
            EnablePlayerMovement();
        }
    }

    // НОВЫЙ МЕТОД: Возвращает игроку подвижность и прячет курсор.
    // Его мы вызовем вручную при закрытии диалога.
    public void EnablePlayerMovement()
    {
        if (playerMovementScript != null) playerMovementScript.enabled = true;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        Debug.Log("Движение игрока восстановлено, курсор скрыт.");
    }

    public void CloseMaze()
    {
        if (mazePanel != null) mazePanel.SetActive(false);
        if (playerMovementScript != null) playerMovementScript.enabled = true;
    }
}