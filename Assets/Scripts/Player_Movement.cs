using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    [SerializeField] private float speed = 120f;
    [SerializeField] private float sprintMultiplier = 2f;

    [Header("Настройка спрайтов")]
    [SerializeField] private Sprite sideSprite;
    [SerializeField] private Sprite backSprite;

    private Rigidbody2D _rb;
    private SpriteRenderer _sr;
    public bool isHidden = false;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();

        _rb.freezeRotation = true;
        _rb.gravityScale = 0f;
    }

    void Update()
    {
        if (isHidden)
        {
            _rb.linearVelocity = Vector2.zero;
            return;
        }

        float x = 0f;
        float y = 0f;

        if (Input.GetKey(KeyCode.W)) y += 1;
        if (Input.GetKey(KeyCode.S)) y -= 1;
        if (Input.GetKey(KeyCode.A)) x -= 1;
        if (Input.GetKey(KeyCode.D)) x += 1;

        Vector2 movement = new Vector2(x, y).normalized;
        float currentSpeed = speed;

        if (Input.GetKey(KeyCode.LeftShift)) currentSpeed *= sprintMultiplier;

        _rb.linearVelocity = movement * currentSpeed;

        HandleSprites(x, y);
    }

    void HandleSprites(float x, float y)
    {
        if (y > 0)
        {
            _sr.sprite = backSprite;
            _sr.flipX = false;
        }
        else if (y < 0 || x != 0)
        {
            _sr.sprite = sideSprite;

            if (x < 0)
            {
                _sr.flipX = true;
            }
            else if (x > 0)
            {
                _sr.flipX = false;
            }
        }
    }
}