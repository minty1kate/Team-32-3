using UnityEngine;

public class HidingSpot : MonoBehaviour
{
    public GameObject player; // Перетащи сюда ГГ в инспекторе
    private bool inRange = false;

    void Update()
    {
        // Проверка нажатия E, когда игрок рядом
        if (inRange && Input.GetKeyDown(KeyCode.E))
        {
            ToggleHide();
        }
    }

    void ToggleHide()
    {
        Player_Movement move = player.GetComponent<Player_Movement>();
        // Ищем спрайт ВНУТРИ игрока (на случай если он дочерний)
        SpriteRenderer sr = player.GetComponentInChildren<SpriteRenderer>();

        move.isHidden = !move.isHidden;

        // Меняем прозрачность
        if (sr != null)
        {
            Color c = sr.color;
            c.a = move.isHidden ? 0f : 1f;
            sr.color = c;
        }
    }

    private void OnTriggerEnter2D(Collider2D other) { if (other.gameObject == player) inRange = true; }
    private void OnTriggerExit2D(Collider2D other) { if (other.gameObject == player) inRange = false; }
}