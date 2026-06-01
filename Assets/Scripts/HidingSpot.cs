using UnityEngine;

public class HidingSpot : MonoBehaviour
{
    public GameObject player; // Ссылка на ГГ

    [Header("Настройки подсветки")]
    public Color highlightColor = new Color(0.8f, 1f, 0.8f, 1f); // Цвет при приближении

    [Header("Звуковые эффекты")]
    public AudioSource hidingAudioSource; // Ссылка на источник звука
    public AudioClip hideSound;          // Звук забирания внутрь (например, скрипт шкафа)
    public AudioClip unhideSound;        // Звук выхода наружу

    private bool inRange = false;
    private SpriteRenderer furnitureSR; // Спрайт самого кресла/шкафа
    private Color originalColor; // Исходный цвет мебели

    void Start()
    {
        furnitureSR = GetComponent<SpriteRenderer>();
        if (furnitureSR != null)
        {
            originalColor = furnitureSR.color;
        }

        if (player == null)
        {
            player = GameObject.FindWithTag("Player");
        }
    }

    void Update()
    {
        if (!inRange) return;

        ExorcistPatrol exorcist = FindFirstObjectByType<ExorcistPatrol>();

        if (exorcist == null || !exorcist.gameObject.activeInHierarchy)
        {
            Player_Movement move = player.GetComponent<Player_Movement>();
            if (move != null && move.isHidden)
            {
                ToggleHide();
            }

            if (furnitureSR != null && furnitureSR.color != originalColor)
            {
                furnitureSR.color = originalColor;
            }
            return;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleHide();
        }
    }

    void ToggleHide()
    {
        if (player == null) return;

        Player_Movement move = player.GetComponent<Player_Movement>();
        SpriteRenderer playerSR = player.GetComponentInChildren<SpriteRenderer>();

        if (move == null) return;

        move.isHidden = !move.isHidden;

        if (move.isHidden)
        {
            // Игрок спрятался
            move.enabled = false;
            if (playerSR != null)
            {
                Color c = playerSR.color;
                c.a = 0f;
                playerSR.color = c;
            }

            // Воспроизводим звук входа
            if (hidingAudioSource != null && hideSound != null)
            {
                hidingAudioSource.PlayOneShot(hideSound);
            }
        }
        else
        {
            // Игрок вышел
            move.enabled = true;
            if (playerSR != null)
            {
                Color c = playerSR.color;
                c.a = 1f;
                playerSR.color = c;
            }

            // Воспроизводим звук выхода
            if (hidingAudioSource != null && unhideSound != null)
            {
                hidingAudioSource.PlayOneShot(unhideSound);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == player)
        {
            ExorcistPatrol exorcist = FindFirstObjectByType<ExorcistPatrol>();
            if (exorcist == null || !exorcist.gameObject.activeInHierarchy) return;

            inRange = true;
            if (furnitureSR != null) furnitureSR.color = highlightColor;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == player)
        {
            inRange = false;
            if (furnitureSR != null) furnitureSR.color = originalColor;

            Player_Movement move = player.GetComponent<Player_Movement>();
            if (move != null && move.isHidden)
            {
                ToggleHide();
            }
        }
    }
}