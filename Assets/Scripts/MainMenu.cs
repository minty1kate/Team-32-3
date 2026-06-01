using UnityEngine;
using UnityEngine.SceneManagement; // Этот модуль нужен для управления сценами

public class MainMenu : MonoBehaviour
{
    // Этот метод сработает при нажатии на кнопку "Начать игру"
    public void OnStartButtonClick()
    {
        // Переключает сцену на следующую по списку в настройках игры
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    // Этот метод сработает при нажатии на кнопку "Выход"
    public void OnQuitButtonClick()
    {
        // Выводит сообщение в консоль Unity, чтобы мы видели, что кнопка работает
        Debug.Log("Игра закрывается...");

        // Полностью закрывает запущенную игру
        Application.Quit();
    }
}
