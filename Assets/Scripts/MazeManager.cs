using UnityEngine;

public class MazeManager : MonoBehaviour
{
    [Header("Ссылки на UI")]
    public GameObject mazePanel;
    public MazeBall mazeBallScript;

    [Header("Ссылки на сцене")]
    public GameObject keyItem;
    public SafePicture safePicture;

    // Перетащи сюда скрипт управления своего ГГ
    public MonoBehaviour playerMovementScript;

    void Start()
    {
        if (mazePanel != null) mazePanel.SetActive(false);
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

            // НЕ используем Time.timeScale = 0, чтобы физика UI работала стабильно
            if (playerMovementScript != null) playerMovementScript.enabled = false;

            SafeScreenController screenCtrl = mazePanel.GetComponent<SafeScreenController>();
            if (screenCtrl != null) screenCtrl.ResetToSafeView();

            if (mazeBallScript != null) mazeBallScript.ResetToStart();
        }
    }

    public void WinMaze()
    {
        if (mazePanel != null) mazePanel.SetActive(false);
        if (playerMovementScript != null) playerMovementScript.enabled = true;

        if (keyItem != null) keyItem.SetActive(true);
        if (safePicture != null) safePicture.gameObject.SetActive(false);
    }

    public void CloseMaze()
    {
        if (mazePanel != null) mazePanel.SetActive(false);
        if (playerMovementScript != null) playerMovementScript.enabled = true;
    }
}