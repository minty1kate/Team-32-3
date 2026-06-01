using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ExorcistPatrol : MonoBehaviour
{
    public Transform[] waypoints;
    public float baseSpeed = 2f;
    public float speedIncreasePerCandle = 0.5f;

    [Header("Настройки Спрайтов")]
    public Sprite normalSprite;
    public Sprite painSprite;
    public Sprite deathSprite;

    [Header("Звуковые Эффекты")]
    public AudioSource exorcistAudioSource; // Ссылка на источник звука
    public AudioClip painSound;            // Звук получения урона
    public AudioClip deathSound;           // Звук смерти

    [Header("Эффекты боли (UI)")]
    public GameObject painFlashPanel;

    [Header("Финальный монолог после смерти")]
    [TextArea(2, 5)]
    public string[] deathDialogueLines;

    private float _currentSpeed;
    private int currentPointIndex = 0;
    private SpriteRenderer spriteRenderer;
    private bool _isDistracted = false;
    private Color _originalColor;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        _originalColor = spriteRenderer.color;
        _currentSpeed = baseSpeed;
        if (normalSprite != null) spriteRenderer.sprite = normalSprite;
    }

    void Update()
    {
        if (_isDistracted || waypoints.Length == 0) return;

        Transform targetPoint = waypoints[currentPointIndex];
        transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, _currentSpeed * Time.deltaTime);

        if (targetPoint.position.x > transform.position.x)
            spriteRenderer.flipX = false;
        else if (targetPoint.position.x < transform.position.x)
            spriteRenderer.flipX = true;

        if (Vector2.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            currentPointIndex = (currentPointIndex + 1) % waypoints.Length;
        }
    }

    public void TakeDamage(int extinguishedCount)
    {
        _currentSpeed = baseSpeed + (extinguishedCount * speedIncreasePerCandle);

        // Воспроизводим звук боли
        if (exorcistAudioSource != null && painSound != null)
        {
            exorcistAudioSource.PlayOneShot(painSound);
        }

        StartCoroutine(PainReactionRoutine());
    }

    public void Die()
    {
        _isDistracted = true;

        // Воспроизводим звук смерти
        if (exorcistAudioSource != null && deathSound != null)
        {
            exorcistAudioSource.PlayOneShot(deathSound);
        }

        StartCoroutine(DeathRoutine());
    }

    public void ResetExorcist()
    {
        _isDistracted = false;
        _currentSpeed = baseSpeed;
        currentPointIndex = 0;
        spriteRenderer.color = _originalColor;
        if (normalSprite != null) spriteRenderer.sprite = normalSprite;
    }

    private IEnumerator PainReactionRoutine()
    {
        _isDistracted = true;

        if (painSprite != null) spriteRenderer.sprite = painSprite;
        if (painFlashPanel != null) painFlashPanel.SetActive(true);

        yield return new WaitForSeconds(0.6f);

        if (painFlashPanel != null) painFlashPanel.SetActive(false);
        if (normalSprite != null) spriteRenderer.sprite = normalSprite;

        _isDistracted = false;
    }

    private IEnumerator DeathRoutine()
    {
        _isDistracted = true;

        if (deathSprite != null) spriteRenderer.sprite = deathSprite;
        if (painFlashPanel != null) painFlashPanel.SetActive(true);

        float duration = 2.0f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            spriteRenderer.color = (spriteRenderer.color == _originalColor) ? Color.red : Color.orange;

            if (elapsed > 0.3f && painFlashPanel != null && painFlashPanel.activeSelf)
            {
                painFlashPanel.SetActive(false);
            }

            yield return new WaitForSeconds(0.1f);
            elapsed += 0.1f;
        }

        TaskManager taskManager = FindFirstObjectByType<TaskManager>();
        if (taskManager != null)
        {
            taskManager.CompleteCurrentTask();
        }

        MazeManager maze = FindFirstObjectByType<MazeManager>();
        if (maze != null)
        {
            maze.ExorcistIsDead();
        }

        DialogueManager dialogueManager = FindFirstObjectByType<DialogueManager>();
        if (dialogueManager != null && deathDialogueLines != null && deathDialogueLines.Length > 0)
        {
            dialogueManager.StartTutorial(deathDialogueLines);
        }

        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && spriteRenderer.sprite != deathSprite)
        {
            Player_Movement move = collision.GetComponent<Player_Movement>();
            if (move != null && move.isHidden)
            {
                return;
            }

            HallManager hallManager = FindObjectOfType<HallManager>();
            if (hallManager != null)
            {
                hallManager.ResetAllCandles();
            }

            ResetExorcist();

            if (DoorSpawnPoint.spawnPosition != Vector2.zero)
            {
                collision.transform.position = DoorSpawnPoint.spawnPosition;
            }
            else
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }
}