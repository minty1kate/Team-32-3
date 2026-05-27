using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class IngredientPickupZone : MonoBehaviour
{
    [Header("Настройки продукта")]
    public string ingredientName; // Уникальное имя: Cola, Sugar или Soap
    public GameObject smallSpritePrefab;

    private GameObject currentDraggedItem;
    private MiniGameManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<MiniGameManager>();

        // МАГИЯ: При загрузке сцены спрашиваем менеджера, бросали ли мы уже этот продукт?
        if (MiniGameManager.usedIngredients.Contains(ingredientName))
        {
            // Если да, просто выключаем эту невидимую зону навсегда
            gameObject.SetActive(false); 
        }
    }

    void OnMouseDown()
    {
        Vector3 mousePosition = GetMouseWorldPosition();
        currentDraggedItem = Instantiate(smallSpritePrefab, mousePosition, Quaternion.identity);
    }

    void OnMouseDrag()
    {
        if (currentDraggedItem != null)
        {
            currentDraggedItem.transform.position = GetMouseWorldPosition();
        }
    }

    void OnMouseUp()
    {
        if (currentDraggedItem != null)
        {
            CheckDrop();
        }
    }

    private void CheckDrop()
    {
        Vector2 mousePos2D = GetMouseWorldPosition();
        Collider2D hit = Physics2D.OverlapPoint(mousePos2D);

        if (hit != null && hit.CompareTag("Pot"))
        {
            // Передаем менеджеру СВОЕ уникальное имя
            gameManager.AddIngredient(ingredientName);
            
            // Выключаем эту зону клика, чтобы нельзя было взять продукт второй раз
            gameObject.SetActive(false);
        }

        Destroy(currentDraggedItem);
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        return mousePos;
    }
}