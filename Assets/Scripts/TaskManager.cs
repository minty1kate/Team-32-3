using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class TaskManager : MonoBehaviour
{
    public GameObject tasksPanel;
    public TextMeshProUGUI taskDisplay;

    // Список пустой по умолчанию, он заполняется динамически для каждой сцены
    private List<string> allTasks = new List<string>();
    private int currentTaskIndex = 0;
    private int collectedPieces = 0;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            tasksPanel.SetActive(!tasksPanel.activeSelf);
        }
    }

    // ЭТОТ МЕТОД ЗАПУСКАЕТСЯ ИЗ ДРУГИХ СКРИПТОВ ПРИ СТАРТЕ СЦЕНЫ
    public void SetSceneTasks(string[] newTasks)
    {
        allTasks.Clear();
        allTasks.AddRange(newTasks);
        currentTaskIndex = 0;
        collectedPieces = 0; // Сбрасываем счетчик предметов для новой сцены
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

    // Универсальный метод для квестов по поиску предметов (3 штуки)
    public void IncrementMirrorPieces()
    {
        collectedPieces++;

        // Автоматически обновляем вторую задачу (индекс 1) в списке, какая бы она ни была
        if (allTasks.Count > 1)
        {
            // Берем чистый текст задачи (до скобок) и добавляем актуальный счетчик
            string baseTaskText = allTasks[1];
            if (baseTaskText.Contains("("))
            {
                baseTaskText = baseTaskText.Split('(')[0].Trim();
            }
            allTasks[1] = $"{baseTaskText} ({collectedPieces}/3)";
        }

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
        if (taskDisplay == null) return;

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

    public int GetCurrentTaskIndex()
    {
        return currentTaskIndex;
    }
}