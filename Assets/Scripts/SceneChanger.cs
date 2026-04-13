using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;

    // ВАЖНО: именно OnTriggerEnter2D и (Collider2D other)
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Кто-то вошел в триггер!"); // Эта строка покажет в консоли, работает ли вход

        if (other.CompareTag("Player"))
        {
            Debug.Log("Игрок обнаружен! Загружаю сцену...");
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}