using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartGame : MonoBehaviour
{
    // Имя твоей самой первой сцены (например, "MainMenu" или "Bedroom")
    [SerializeField] private string firstSceneName = "Главное меню";

    public void LoadFirstScene()
    {
        SceneManager.LoadScene(firstSceneName);
    }
}