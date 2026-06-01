using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class DoorHighlight : MonoBehaviour
{
    private SpriteRenderer _sr;
    private Color _originalColor;
    private bool _isPlayerInside = false;
    private bool _isUnlocked = false;

    [SerializeField] private Color highlightColor = Color.gray;
    [SerializeField] private string sceneToLoad;

    [Header("Проверка квеста (TaskManager)")]
    [SerializeField] private TaskManager taskManager;
    [Tooltip("Индекс задачи, при наступлении/выполнении которой дверь откроется. Например, для 'Починить зеркало' это индекс 2, а для его выполнения индекс должен стать 3.")]
    [SerializeField] private int requiredTaskIndex = 3;

    [Header("Звуковые эффекты")]
    [SerializeField] private AudioSource doorAudioSource;
    [SerializeField] private AudioClip openDoorSound;

    void Start()
    {
        _sr = GetComponent<SpriteRenderer>();
        _originalColor = _sr.color;

        // Если забыли перетащить TaskManager в инспекторе, пытаемся найти его на сцене автоматически
        if (taskManager == null)
        {
            taskManager = FindAnyObjectByType<TaskManager>();
        }
    }

    void Update()
    {
        // Проверяем состояние задачи динамически
        CheckTaskCondition();

        if (_isUnlocked && _isPlayerInside && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(LoadSceneWithSoundRoutine());
        }
    }

    private void CheckTaskCondition()
    {
        if (taskManager != null)
        {
            // Дверь разблокируется, если текущий индекс задачи равен или больше требуемого
            if (taskManager.GetCurrentTaskIndex() >= requiredTaskIndex)
            {
                if (!_isUnlocked)
                {
                    UnlockDoor();
                }
            }
            else
            {
                // Если задача ещё не готова, держим закрытой
                _isUnlocked = false;
            }
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
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        if (doorAudioSource != null && openDoorSound != null)
        {
            doorAudioSource.PlayOneShot(openDoorSound);
            yield return new WaitForSeconds(openDoorSound.length);
        }
        else
        {
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