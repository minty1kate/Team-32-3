using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic; // Нужно для работы со списками (List)

public class MiniGameManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI scoreText;
    
    [Header("Settings")]
    public string cutsceneSceneName = "Cutscene";
    public string mainSceneName = "Kitchen";
    
    // ГЛОБАЛЬНАЯ ПАМЯТЬ МИНИ-ИГРЫ
    public static int currentScore = 0; 
    public static List<string> usedIngredients = new List<string>(); // Список добавленных продуктов

    private int maxScore = 3;

    void Start()
    {
        UpdateScoreUI();
    }

    // Теперь метод требует передать имя продукта, который мы кидаем
    public void AddIngredient(string ingredientName)
    {
        // Проверяем, нет ли еще этого продукта в супе
        if (!usedIngredients.Contains(ingredientName))
        {
            usedIngredients.Add(ingredientName); // Записываем в блокнот: "этот продукт бросили"
            currentScore++;
            UpdateScoreUI();
        }
    }

    private void UpdateScoreUI()
    {
        scoreText.text = currentScore + "/" + maxScore;
    }

    public void ExitMiniGame()
    {
        if (currentScore >= maxScore)
        {
            // Сбрасываем прогресс мини-игры на случай, если игра начнется заново
            ResetMiniGameProgress(); 
            SceneManager.LoadScene(cutsceneSceneName);
        }
        else
        {
            SceneManager.LoadScene(mainSceneName);
        }
    }

    // Метод для полной очистки прогресса (полезно для главного меню)
    public static void ResetMiniGameProgress()
    {
        currentScore = 0;
        usedIngredients.Clear();
    }
}