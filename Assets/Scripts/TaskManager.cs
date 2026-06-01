using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class TaskManager : MonoBehaviour
{
    public GameObject tasksPanel;
    public TextMeshProUGUI taskDisplay;

    private List<string> allTasks = new List<string> {
        "Встать с кровати (Клавиша C)",
        "Собрать осколки картины (0/3)",
        "Собрать картину",
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

    // МЕТОД ДЛЯ СМЕНЫ ЗАДАЧ НА СЦЕНЕ (Загружает сразу цепочку новых квестов)
    public void SetSceneTasks(string[] newTasks)
    {
        allTasks.Clear();
        allTasks.AddRange(newTasks);
        currentTaskIndex = 0;
        UpdateTaskUI();
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
        allTasks[1] = $"Собрать осколки зеркала ({collectedPieces}/3)";

        if (collectedPieces >= 3)
        {
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

    // Позволяет другим скриптам (например, дверям) узнать текущий прогресс по квестам
    public int GetCurrentTaskIndex()
    {
        return currentTaskIndex;
    }
}