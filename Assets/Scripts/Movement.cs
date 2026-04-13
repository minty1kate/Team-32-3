using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private float speed = 120f;
    [SerializeField] private float sprintMultiplier = 20f;
    private Rigidbody2D _rb;
    private SpriteRenderer _sr;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        var x = 0f;
        var y = 0f;

        if (Input.GetKey(KeyCode.W)) y += 1;
        if (Input.GetKey(KeyCode.S)) y -= 1;
        if (Input.GetKey(KeyCode.A)) x -= 1;
        if (Input.GetKey(KeyCode.D)) x += 1;

        var movement = new Vector2(x, y).normalized;
        var currentSpeed = speed;
        
        if (Input.GetKey(KeyCode.LeftShift)) currentSpeed *= sprintMultiplier;

        _rb.linearVelocity = movement * currentSpeed;
        
        if (x<0)
            _sr.flipX = true;
        else if (x > 0)
            _sr.flipX = false;

        // if (movement != Vector2.zero)
        // {
        //     var angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg;
        //     transform.rotation = Quaternion.Euler(0, 0, angle);
        // }
    }
}