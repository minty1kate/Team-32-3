using UnityEngine;
using System.Collections;

public class MazeManager : MonoBehaviour
{
    [Header("Ссылки на UI")]
    public GameObject mazePanel;
    public MazeBall mazeBallScript;

    [Space(10)]
    public GameObject openSafePanel;

    [Header("Звуковые эффекты")]
    public AudioSource safeAudioSource;
    public AudioClip openSafeSound;

    [Header("Настройки Ключа для Инвентаря")]
    public InventoryManager inventoryManager;
    public TaskManager taskManager;
    public Sprite keyIcon;

    [Header("Система Диалогов")]
    public DialogueManager dialogueManager;

    [Header("Ссылки на сцене")]
    public GameObject keyItem;
    public SafePicture safePicture;
    public SpriteRenderer pictureSpriteRenderer;
    public Sprite openSafeWallSprite;
    public DoorHighlight doorScript;

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
        if (safeAudioSource != null && openSafeSound != null)
        {
            safeAudioSource.PlayOneShot(openSafeSound);
        }

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

    public void TakeKeyAndCloseEverything()
    {
        if (inventoryManager != null && keyIcon != null) inventoryManager.AddItem(keyIcon);
        if (taskManager != null) taskManager.CompleteCurrentTask();
        if (openSafePanel != null) openSafePanel.SetActive(false);
        if (keyItem != null) keyItem.SetActive(true);
        if (pictureSpriteRenderer != null && openSafeWallSprite != null) pictureSpriteRenderer.sprite = openSafeWallSprite;

        if (doorScript != null) doorScript.UnlockDoor();

        TriggerFinalHallDialogue();
        StartCoroutine(ForceShowCursorRoutine());
    }

    private IEnumerator ForceShowCursorRoutine()
    {
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

            if (playerMovementScript != null) playerMovementScript.enabled = false;

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            dialogueManager.StartTutorial(finalLines);

            if (dialogueManager.closeButton != null)
            {
                dialogueManager.closeButton.onClick.RemoveAllListeners();
                dialogueManager.closeButton.onClick.AddListener(dialogueManager.CloseDialogue);
                dialogueManager.closeButton.onClick.AddListener(EnablePlayerMovement);
            }
        }
        else
        {
            EnablePlayerMovement();
        }
    }

    public void EnablePlayerMovement()
    {
        if (playerMovementScript != null) playerMovementScript.enabled = true;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        Debug.Log("Движение игрока восстановлено, курсор оставлен видимым.");
    }

    public void CloseMaze()
    {
        if (mazePanel != null) mazePanel.SetActive(false);
        if (playerMovementScript != null) playerMovementScript.enabled = true;
    }
}