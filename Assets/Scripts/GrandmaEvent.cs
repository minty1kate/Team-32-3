using UnityEngine;
using System.Collections;

public class GrandmaEvent : MonoBehaviour
{
    [Header("Настройки Бабушки")]
    public Transform grandmaTransform;
    public SpriteRenderer grandmaSpriteRenderer;
    public Sprite cryingSprite;

    [Header("Куда бежать")]
    public Transform targetPosition;
    public float moveSpeed = 4f;

    private bool isTriggered = false;

    // СЛОВО STATIC - ЭТО МАГИЯ. Эта переменная общая для всей игры.
    // Она запомнит свое значение даже если мы уйдем на другую сцену и вернемся.
    public static bool hasGrandmaMoved = false; 

    private void Start()
    {
        // Проверяем при загрузке Кухни: бабка УЖЕ убегала до этого?
        if (hasGrandmaMoved)
        {
            // Если да, мгновенно телепортируем её к двери
            grandmaTransform.position = targetPosition.position;
            
            // Мгновенно меняем картинку на плачущую
            if (cryingSprite != null)
            {
                grandmaSpriteRenderer.sprite = cryingSprite;
            }
            
            // Удаляем этот триггер, так как событие уже произошло
            Destroy(gameObject); 
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isTriggered)
        {
            isTriggered = true;
            StartCoroutine(GrandmaRunSequence());
        }
    }

    private IEnumerator GrandmaRunSequence()
    {
        while (Vector3.Distance(grandmaTransform.position, targetPosition.position) > 0.1f)
        {
            grandmaTransform.position = Vector3.MoveTowards(grandmaTransform.position, targetPosition.position, moveSpeed * Time.deltaTime);
            yield return null;
        }

        if (cryingSprite != null)
        {
            grandmaSpriteRenderer.sprite = cryingSprite;
        }

        // Когда бабка добежала до конца, ЗАПОМИНАЕМ это глобально
        hasGrandmaMoved = true;
        
        Destroy(gameObject);
    }
}