using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement; // Обязательно для перезапуска сцены

public class ScaryStaircaseQte : MonoBehaviour
{
    [Header("Настройки QTE")]
    [Tooltip("Сколько раз нужно нажать ЛКМ, чтобы отбиться")]
    public int targetClicks = 10;
    
    [Tooltip("Время на прохождение (в секундах)")]
    public float timeLimit = 4f;

    [Header("Ссылки на объекты")]
    public GameObject playerObject;       // Объект вашего игрока
    public GameObject handsGroup;         // Объект с руками (включится при триггере)
    public GameObject qteTextUI;          // Текст "ЖМИ ЛКМ!" из Canvas
    public Image bloodOverlay;            // Полупрозрачный красный экран из Canvas

    [Header("Настройки эффектов")]
    [Tooltip("Сила бешеной тряски самих рук")]
    public float handShakeIntensity = 0.1f;
    [Tooltip("Сила тряски камеры при каждом клике")]
    public float cameraShakeIntensity = 0.15f;

    private int _currentClicks = 0;
    private bool _isActive = false;
    private bool _isTriggered = false;
    
    private Vector3 _originalHandsPosition;
    private Transform _mainCameraTransform;
    private Vector3 _originalCameraPosition;

    void Start()
    {
        // Находим главную камеру на сцене для эффекта тряски
        if (Camera.main != null)
        {
            _mainCameraTransform = Camera.main.transform;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Проверяем, что вошел игрок, и QTE еще не срабатывало
        if (other.CompareTag("Player") && !_isTriggered)
        {
            _isTriggered = true;
            StartQTE();
        }
    }

    void StartQTE()
    {
        _isActive = true;
        _currentClicks = 0;

        // 1. НАМЕРТВО ОСТАНАВЛИВАЕМ ИГРОКА
        // Отключаем ваш скрипт Movement, чтобы заблокировать кнопки ходьбы
        var movementScript = playerObject.GetComponent<Movement>();
        if (movementScript != null) movementScript.enabled = false;

        // Обнуляем скорость в Rigidbody2D, чтобы убрать скольжение по инерции
        var rb = playerObject.GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero; 

        // 2. ВКЛЮЧАЕМ ВИЗУАЛ
        if (handsGroup != null)
        {
            handsGroup.SetActive(true);
            _originalHandsPosition = handsGroup.transform.localPosition; // Запоминаем начальную позицию рук
        }
        if (qteTextUI != null) qteTextUI.SetActive(true);
        if (_mainCameraTransform != null) _originalCameraPosition = _mainCameraTransform.localPosition; // Запоминаем позицию камеры

        // 3. ЗАПУСКАЕМ ТАЙМЕР
        StartCoroutine(TimeLimitRoutine());
    }

    void Update()
    {
        if (!_isActive) return;

        // ЭФФЕКТ 1: Бешеная тряска рук каждый кадр
        if (handsGroup != null)
        {
            float randomX = Random.Range(-handShakeIntensity, handShakeIntensity);
            float randomY = Random.Range(-handShakeIntensity, handShakeIntensity);
            handsGroup.transform.localPosition = _originalHandsPosition + new Vector3(randomX, randomY, 0);
        }

        // ЭФФЕКТ 2: Пульсация багрового экрана от страха
        if (bloodOverlay != null)
        {
            float alpha = Mathf.PingPong(Time.time * 3f, 0.4f); // Пульсирует от 0 до 0.4 прозрачности
            Color c = bloodOverlay.color;
            c.a = alpha;
            bloodOverlay.color = c;
        }

        // Считывание кликов борьбы
        if (Input.GetMouseButtonDown(0))
        {
            _currentClicks++;
            
            // ЭФФЕКТ 3: Тряска камеры при каждом клике мышкой
            StartCoroutine(ShakeCameraRoutine(0.1f, cameraShakeIntensity));

            if (_currentClicks >= targetClicks)
            {
                StopAllCoroutines(); // Игрок успел, останавливаем таймер проиграша
                WinQTE();
            }
        }
    }

    // Корутина тряски камеры при ударе/клике
    IEnumerator ShakeCameraRoutine(float duration, float intensity)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float x = Random.Range(-intensity, intensity);
            float y = Random.Range(-intensity, intensity);
            
            if (_mainCameraTransform != null)
                _mainCameraTransform.localPosition = new Vector3(_originalCameraPosition.x + x, _originalCameraPosition.y + y, _originalCameraPosition.z);
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        if (_mainCameraTransform != null) _mainCameraTransform.localPosition = _originalCameraPosition; // Возвращаем камеру строго на место
    }

    // Этот таймер срабатывает, ЕСЛИ ИГРОК НЕ УСПЕЛ набрать клики
    IEnumerator TimeLimitRoutine()
    {
        yield return new WaitForSeconds(timeLimit);
        
        _isActive = false; // Блокируем клики

        // МГНОВЕННЫЙ СБРОС СЦЕНЫ (Телепортация в начало)
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    // Срабатывает, если игрок успел отбиться
    void WinQTE()
    {
        _isActive = false;

        // Отключаем визуал и возвращаем объекты на исходные позиции
        if (handsGroup != null)
        {
            handsGroup.transform.localPosition = _originalHandsPosition;
            handsGroup.SetActive(false);
        }

        if (qteTextUI != null) qteTextUI.SetActive(false);

        if (bloodOverlay != null)
        {
            Color c = bloodOverlay.color;
            c.a = 0f;
            bloodOverlay.color = c;
        }

        if (_mainCameraTransform != null) _mainCameraTransform.localPosition = _originalCameraPosition;

        // ВОЗВРАЩАЕМ УПРАВЛЕНИЕ ХОДЬБОЙ
        var movementScript = playerObject.GetComponent<Movement>();
        if (movementScript != null) movementScript.enabled = true;

        // Самоуничтожаем триггер, чтобы сцена проигрывалась только 1 раз за уровень
        Destroy(gameObject);
    }
}
