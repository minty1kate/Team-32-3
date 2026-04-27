using UnityEngine;


public class Player_Movement : MonoBehaviour
{
    [SerializeField] private float speed = 120f;
    [SerializeField] private float sprintMultiplier = 2f;

    [Header("Настройка спрайтов")]
    [SerializeField] private Sprite sideSprite; // Спрайт "Боком" (для влево, вправо и ВНИЗ)
    [SerializeField] private Sprite backSprite; // Спрайт "Со спины" (только для ВВЕРХ)

    private Rigidbody2D _rb;
    private SpriteRenderer _sr;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();

        // Настройки физики, чтобы ГГ не падал и не крутился
        _rb.freezeRotation = true;
        _rb.gravityScale = 0f;
    }

    void Update()
    {
        float x = 0f;
        float y = 0f;

        // Считываем нажатия
        if (Input.GetKey(KeyCode.W)) y += 1;
        if (Input.GetKey(KeyCode.S)) y -= 1;
        if (Input.GetKey(KeyCode.A)) x -= 1;
        if (Input.GetKey(KeyCode.D)) x += 1;

        Vector2 movement = new Vector2(x, y).normalized;
        float currentSpeed = speed;

        if (Input.GetKey(KeyCode.LeftShift)) currentSpeed *= sprintMultiplier;

        // Движение через физику
        _rb.linearVelocity = movement * currentSpeed;

        // Логика смены спрайтов
        HandleSprites(x, y);
    }

    void HandleSprites(float x, float y)
    {
        // 1. Если идем строго вверх
        if (y > 0)
        {
            _sr.sprite = backSprite;
            _sr.flipX = false; // Со спины обычно симметричен
        }
        // 2. Если идем вниз, влево или вправо
        else if (y < 0 || x != 0)
        {
            _sr.sprite = sideSprite;

            // Зеркалим только если нажата клавиша влево (A)
            // Если идем вниз или вправо, оставляем как есть
            if (x < 0)
            {
                _sr.flipX = true;
            }
            else if (x > 0)
            {
                _sr.flipX = false;
            }
            // Если x == 0 (идем чисто вниз), оставляем тот поворот flipX, 
            // который был до этого, чтобы персонаж не дергался.
        }
    }
}