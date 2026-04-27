using UnityEngine;
using UnityEngine.SceneManagement; // Обязательно добавь эту строку!

public class DoorHighlight : MonoBehaviour
{
    private SpriteRenderer _sr;
    private Color _originalColor;
    private bool _isPlayerInside = false;

    [SerializeField] private Color highlightColor = Color.gray;
    [SerializeField] private string sceneToLoad; // Название сцены, куда идем

    void Start()
    {
        _sr = GetComponent<SpriteRenderer>();
        _originalColor = _sr.color;
    }

    void Update()
    {
        // Если игрок в зоне И нажал клавишу E
        if (_isPlayerInside && Input.GetKeyDown(KeyCode.E))
        {
            LoadNewScene();
        }
    }

    private void LoadNewScene()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogError("Название сцены не введено в инспекторе двери!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _isPlayerInside = true;
            _sr.color = highlightColor;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _isPlayerInside = false;
            _sr.color = _originalColor;
        }
    }
}