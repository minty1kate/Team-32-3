using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class TaskManager : MonoBehaviour
{
    public GameObject tasksPanel;
    public TextMeshProUGUI taskDisplay;

    private List<string> allTasks = new List<string> {
        "Встать с кровати (Клавиша C)",
        "Собрать осколки зеркала (0/3)", // Теперь это индекс 1
        "Починить зеркало",
        "Найти брата"
    };

    private int currentTaskIndex = 0;
    private int collectedPieces = 0;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            tasksPanel.SetActive(!tasksPanel.activeSelf);
        }
    }

    public void CompleteCurrentTask()
    {
        if (currentTaskIndex < allTasks.Count)
        {
            currentTaskIndex++;
            UpdateTaskUI();
        }
    }

    public void IncrementMirrorPieces()
    {
        collectedPieces++;

        // Обновляем текст под индексом 1 (теперь это осколки)
        allTasks[1] = $"Собрать осколки зеркала ({collectedPieces}/3)";

        if (collectedPieces >= 3)
        {
            // Завершаем текущую задачу (осколки), индекс станет 2
            CompleteCurrentTask();
        }
        else
        {
            UpdateTaskUI();
        }
    }

    void UpdateTaskUI()
    {
        string text = "<b>ЗАДАЧИ:</b>\n\n";
        for (int i = 0; i <= currentTaskIndex; i++)
        {
            if (i >= allTasks.Count) break;

            if (i < currentTaskIndex)
                text += "<s>" + allTasks[i] + "</s>\n";
            else
                text += "• " + allTasks[i] + "\n";
        }
        taskDisplay.text = text;
    }
}