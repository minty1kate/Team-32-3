using UnityEngine;

public class HidingSpot : MonoBehaviour
{
    public GameObject player; // Ссылка на ГГ

    [Header("Настройки подсветки")]
    public Color highlightColor = new Color(0.8f, 1f, 0.8f, 1f); // Цвет при приближении (чуть зеленоватый/яркий)

    private bool inRange = false;
    private SpriteRenderer furnitureSR; // Спрайт самого кресла/шкафа
    private Color originalColor; // Исходный цвет мебели

    void Start()
    {
        // Получаем SpriteRenderer самого предмета мебели, на котором висит этот скрипт
        furnitureSR = GetComponent<SpriteRenderer>();
        if (furnitureSR != null)
        {
            originalColor = furnitureSR.color;
        }

        // Если забыла перетащить игрока в инспекторе, попробуем найти его по тегу
        if (player == null)
        {
            player = GameObject.FindWithTag("Player");
        }
    }

    void Update()
    {
        // Если игрока нет рядом — ничего не делаем
        if (!inRange) return;

        // ПРОВЕРКА: Ищем экзорциста на сцене
        ExorcistPatrol exorcist = FindFirstObjectByType<ExorcistPatrol>();

        // Если экзорцист погиб (стал null или выключен), прятаться больше нельзя
        if (exorcist == null || !exorcist.gameObject.activeInHierarchy)
        {
            // Если игрок в этот момент сидел внутри, принудительно высаживаем его
            Player_Movement move = player.GetComponent<Player_Movement>();
            if (move != null && move.isHidden)
            {
                ToggleHide();
            }

            // Возвращаем мебели обычный цвет и выходим
            if (furnitureSR != null && furnitureSR.color != originalColor)
            {
                furnitureSR.color = originalColor;
            }
            return;
        }

        // Если экзорцист жив, обычная логика укрытия работает
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

        // Переключаем состояние скрытности
        move.isHidden = !move.isHidden;

        if (move.isHidden)
        {
            // Игрок спрятался: выключаем его скрипт движения и делаем невидимым
            move.enabled = false;
            if (playerSR != null)
            {
                Color c = playerSR.color;
                c.a = 0f;
                playerSR.color = c;
            }
        }
        else
        {
            // Игрок вышел из укрытия: возвращаем управление и видимость
            move.enabled = true;
            if (playerSR != null)
            {
                Color c = playerSR.color;
                c.a = 1f;
                playerSR.color = c;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == player)
        {
            // Проверяем, жив ли враг, прежде чем включать подсветку
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
            // Выключаем подсветку и возвращаем исходный цвет
            if (furnitureSR != null) furnitureSR.color = originalColor;

            // На случай, если игрок умудрился выйти из триггера, будучи невидимым
            Player_Movement move = player.GetComponent<Player_Movement>();
            if (move != null && move.isHidden)
            {
                ToggleHide();
            }
        }
    }
}