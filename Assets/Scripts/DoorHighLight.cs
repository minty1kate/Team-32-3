using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // Добавлено для работы корутин

public class DoorHighlight : MonoBehaviour
{
    private SpriteRenderer _sr;
    private Color _originalColor;
    private bool _isPlayerInside = false;
    private bool _isUnlocked = false;

    [SerializeField] private Color highlightColor = Color.gray;
    [SerializeField] private string sceneToLoad;

    [Header("Звуковые эффекты")]
    [SerializeField] private AudioSource doorAudioSource; // Источник звука
    [SerializeField] private AudioClip openDoorSound;     // Клип открытия двери

    void Start()
    {
        _sr = GetComponent<SpriteRenderer>();
        _originalColor = _sr.color;
    }

    void Update()
    {
        if (_isUnlocked && _isPlayerInside && Input.GetKeyDown(KeyCode.E))
        {
            // Запускаем корутину вместо прямого метода загрузки
            StartCoroutine(LoadSceneWithSoundRoutine());
        }
    }

    public void UnlockDoor()
    {
        _isUnlocked = true;
        if (_isPlayerInside)
        {
            _sr.color = highlightColor;
        }
    }

    private IEnumerator LoadSceneWithSoundRoutine()
    {
        // Выключаем коллайдер, чтобы игрок не нажал "Е" повторно во время задержки
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // Воспроизводим звук
        if (doorAudioSource != null && openDoorSound != null)
        {
            doorAudioSource.PlayOneShot(openDoorSound);
            // Ждем длительность звукового файла перед сменой сцены
            yield return new WaitForSeconds(openDoorSound.length);
        }
        else
        {
            // Если звука нет, ждем 0 кадров, чтобы не было ошибки
            yield return null;
        }

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
            if (_isUnlocked)
            {
                _sr.color = highlightColor;
            }
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