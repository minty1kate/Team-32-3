using UnityEngine;

public class MazeBall : MonoBehaviour
{
    public float moveSpeed = 300f;
    public RectTransform mazePanelRect;

    private RectTransform _rectTransform;
    private Vector2 _startPosition;

    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _startPosition = _rectTransform.anchoredPosition;
    }

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        Vector2 moveDirection = new Vector2(moveX, moveY).normalized;

        if (moveDirection.magnitude > 0)
        {
            // Считаем позицию для следующего шага
            Vector2 targetPosition = _rectTransform.anchoredPosition + moveDirection * moveSpeed * Time.deltaTime;

            // Смещаем точку в целевую позицию
            _rectTransform.anchoredPosition = targetPosition;
            ClampToPanel();

            // СРАЗУ проверяем: если после этого шага мы оказались в стене — мгновенно на старт!
            if (IsInsideWall())
            {
                ResetToStart();
                return; // Выходим из кадра, чтобы зафиксировать позицию старта
            }
        }
    }

    // Проверка: находится ли точка внутри стены прямо сейчас
    bool IsInsideWall()
    {
        Collider2D myCollider = GetComponent<Collider2D>();
        if (myCollider == null) return false;

        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(LayerMask.GetMask("MazeWall")); // Проверяем только слой MazeWall
        filter.useLayerMask = true;

        Collider2D[] results = new Collider2D[5];
        int count = myCollider.Overlap(filter, results);

        return count > 0;
    }

    public void ResetToStart()
    {
        if (_rectTransform == null) _rectTransform = GetComponent<RectTransform>();
        _rectTransform.anchoredPosition = _startPosition;
    }

    private void ClampToPanel()
    {
        if (mazePanelRect == null) return;

        Vector2 pos = _rectTransform.anchoredPosition;
        float minX = -mazePanelRect.rect.width / 2 + _rectTransform.rect.width / 2;
        float maxX = mazePanelRect.rect.width / 2 - _rectTransform.rect.width / 2;
        float minY = -mazePanelRect.rect.height / 2 + _rectTransform.rect.height / 2;
        float maxY = mazePanelRect.rect.height / 2 - _rectTransform.rect.height / 2;

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        _rectTransform.anchoredPosition = pos;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("MazeFinish"))
        {
            MazeManager manager = FindFirstObjectByType<MazeManager>();
            if (manager != null) manager.WinMaze();
        }
    }
}