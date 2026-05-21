using UnityEngine;
using UnityEngine.SceneManagement;

public class ExorcistPatrol : MonoBehaviour
{
    public Transform[] waypoints;
    public float speed = 2f;

    private int currentPointIndex = 0;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (waypoints.Length == 0) return;

        Transform targetPoint = waypoints[currentPointIndex];
        transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, speed * Time.deltaTime);

        if (targetPoint.position.x > transform.position.x)
            spriteRenderer.flipX = false;
        else if (targetPoint.position.x < transform.position.x)
            spriteRenderer.flipX = true;

        if (Vector2.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            currentPointIndex = (currentPointIndex + 1) % waypoints.Length;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Проверка: спрятан ли игрок
            Player_Movement move = collision.GetComponent<Player_Movement>();
            if (move != null && move.isHidden)
            {
                return; // Экзорцист не замечает спрятавшегося игрока
            }

            // Если не спрятан — логика поражения
            if (DoorSpawnPoint.spawnPosition != Vector2.zero)
            {
                collision.transform.position = DoorSpawnPoint.spawnPosition;
                currentPointIndex = 0;
            }
            else
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }
}