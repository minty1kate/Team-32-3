using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class StaircaseQTE : MonoBehaviour
{
    [Header("Настройки QTE")]
    public float TimeLimit = 5.0f;

    [Header("Ссылки на объекты")]
    public GameObject PlayerObject;
    public GameObject HandsGroup;
    public GameObject QteTextUI;
    public Image BloodOverlay;

    [Header("Система Диалогов")]
    [SerializeField] private DialogueManager dialogueManager;

    [Header("Настройки хаотичного движения рук")]
    [SerializeField] private GameObject[] handPrefabs;
    [SerializeField] private float handMoveSpeed = 300f;

    [Header("Настройки пульсации крови")]
    [SerializeField] private float pulseSpeed = 4f;
    [SerializeField] private float minAlpha = 0.2f;
    [SerializeField] private float maxAlpha = 0.6f;

    [Header("Настройки тряски ВСЕГО ЭКРАНА (Камеры)")]
    [SerializeField] private float cameraShakeIntensity = 0.15f;
    [SerializeField] private float cameraShakeSpeed = 45f;

    private int activeHandsCount = 0;
    private float timer = 0f;
    private bool isQteActive = false;

    private Vector2 minBounds;
    private Vector2 maxBounds;

    private bool hasTriggered = false;
    private static bool isPermanentlyPassed = false;

    private Transform mainCameraTransform;
    private Vector3 originalCameraPosition;

    private void Start()
    {
        if (Camera.main != null)
        {
            mainCameraTransform = Camera.main.transform;
        }

        if (isPermanentlyPassed)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isQteActive && !hasTriggered)
        {
            hasTriggered = true;
            StartQTE();
        }
    }

    public void StartQTE()
    {
        if (handPrefabs == null || handPrefabs.Length == 0 || HandsGroup == null) return;

        if (mainCameraTransform == null && Camera.main != null)
        {
            mainCameraTransform = Camera.main.transform;
        }

        if (mainCameraTransform != null)
        {
            originalCameraPosition = mainCameraTransform.position;
        }

        timer = 0f;
        isQteActive = true;
        activeHandsCount = handPrefabs.Length;

        if (PlayerObject != null)
        {
            MonoBehaviour[] scripts = PlayerObject.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour script in scripts)
            {
                if (script != this) script.enabled = false;
            }

            Rigidbody2D rb = PlayerObject.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }
        }

        if (HandsGroup != null) HandsGroup.SetActive(true);
        if (QteTextUI != null) QteTextUI.SetActive(true);

        if (BloodOverlay != null)
        {
            BloodOverlay.gameObject.SetActive(true);
            Color c = BloodOverlay.color;
            BloodOverlay.color = new Color(c.r, c.g, c.b, minAlpha);
        }

        CalculateSpawnBounds();

        foreach (Transform child in HandsGroup.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < handPrefabs.Length; i++)
        {
            SpawnSpecificHand(handPrefabs[i]);
        }
    }

    private void Update()
    {
        if (!isQteActive) return;

        timer += Time.deltaTime;
        if (timer >= TimeLimit)
        {
            QteFailed();
            return;
        }

        if (BloodOverlay != null)
        {
            float sinValue = Mathf.Sin(Time.time * pulseSpeed);
            float normalizedSin = (sinValue + 1f) / 2f;
            float targetAlpha = Mathf.Lerp(minAlpha, maxAlpha, normalizedSin);

            Color c = BloodOverlay.color;
            BloodOverlay.color = new Color(c.r, c.g, c.b, targetAlpha);
        }

        if (mainCameraTransform != null)
        {
            float shakeX = Mathf.Sin(Time.time * cameraShakeSpeed) * cameraShakeIntensity;
            float shakeY = Mathf.Cos(Time.time * (cameraShakeSpeed * 1.1f)) * cameraShakeIntensity;

            mainCameraTransform.position = originalCameraPosition + new Vector3(shakeX, shakeY, 0f);
        }
    }

    private void CalculateSpawnBounds()
    {
        if (HandsGroup != null)
        {
            RectTransform rect = HandsGroup.GetComponent<RectTransform>();
            Vector3[] corners = new Vector3[4];
            rect.GetLocalCorners(corners);
            minBounds = corners[0];
            maxBounds = corners[2];
        }
    }

    private void SpawnSpecificHand(GameObject prefab)
    {
        GameObject newHand = Instantiate(prefab, HandsGroup.transform);
        RectTransform handRect = newHand.GetComponent<RectTransform>();
        handRect.anchoredPosition = new Vector2(Random.Range(minBounds.x, maxBounds.x), Random.Range(minBounds.y, maxBounds.y));

        // Добавляем внутренний класс, который написан ниже
        MovingHandTarget handScript = newHand.AddComponent<MovingHandTarget>();
        handScript.Setup(this, handMoveSpeed, minBounds, maxBounds);
    }

    public void OnHandClicked()
    {
        activeHandsCount--;

        if (activeHandsCount <= 0)
        {
            QteSuccess();
        }
    }

    private void QteSuccess()
    {
        isQteActive = false;
        isPermanentlyPassed = true;

        if (mainCameraTransform != null)
        {
            mainCameraTransform.position = originalCameraPosition;
        }

        if (HandsGroup != null) HandsGroup.SetActive(false);
        if (BloodOverlay != null) BloodOverlay.gameObject.SetActive(false);
        if (QteTextUI != null) QteTextUI.SetActive(false);

        if (dialogueManager != null)
        {
            string[] linesAfterQTE = new string[]
            {
                "Что это было?! Эти руки тянулись ко мне со всех сторон...",
                "Но почему... почему от их прикосновений мне стало так невыносимо тепло?",
                "Это... это кольцо на пальце... Оно точь-в-точь как у моей мамы.",
                "Нет, это бред! Она жива, она не может быть одной из этих призрачных тварей..."
            };

            dialogueManager.StartTutorial(linesAfterQTE);

            if (dialogueManager.closeButton != null)
            {
                dialogueManager.closeButton.onClick.RemoveAllListeners();
                dialogueManager.closeButton.onClick.AddListener(dialogueManager.CloseDialogue);
                dialogueManager.closeButton.onClick.AddListener(EnablePlayerMovement);
            }
        }
        else
        {
            EnablePlayerMovement();
        }

        Debug.Log("QTE Успешно пройдено! Запущен диалог.");
    }

    public void EnablePlayerMovement()
    {
        if (PlayerObject != null)
        {
            Player_Movement movement = PlayerObject.GetComponent<Player_Movement>();
            if (movement != null)
            {
                movement.enabled = true;
            }
        }
    }

    private void QteFailed()
    {
        isQteActive = false;

        if (mainCameraTransform != null)
        {
            mainCameraTransform.position = originalCameraPosition;
        }

        if (HandsGroup != null) HandsGroup.SetActive(false);
        if (BloodOverlay != null) BloodOverlay.gameObject.SetActive(false);
        if (QteTextUI != null) QteTextUI.SetActive(false);

        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
}

// ВТОРОЙ КЛАСС В ЭТОМ ЖЕ ФАЙЛЕ (НЕ УДАЛЯЙТЕ ЕГО)
public class MovingHandTarget : MonoBehaviour
{
    private StaircaseQTE qteManager;
    private RectTransform rectTransform;
    private Vector2 targetPosition;
    private float speed;
    private Vector2 minB;
    private Vector2 maxB;
    private float directionTimer;

    public void Setup(StaircaseQTE manager, float moveSpeed, Vector2 minBounds, Vector2 maxBounds)
    {
        qteManager = manager;
        speed = moveSpeed;
        minB = minBounds;
        maxB = maxBounds;
        rectTransform = GetComponent<RectTransform>();

        SetNewRandomTarget();
        LookAtCenter();

        Button btn = GetComponent<Button>();
        if (btn == null) btn = gameObject.AddComponent<Button>();

        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(Clicked);
    }

    private void Update()
    {
        rectTransform.anchoredPosition = Vector2.MoveTowards(rectTransform.anchoredPosition, targetPosition, speed * Time.deltaTime);
        LookAtCenter();

        directionTimer += Time.deltaTime;
        if (directionTimer >= 0.6f || Vector2.Distance(rectTransform.anchoredPosition, targetPosition) < 10f)
        {
            SetNewRandomTarget();
            directionTimer = 0f;
        }
    }

    private void SetNewRandomTarget()
    {
        targetPosition = new Vector2(Random.Range(minB.x, maxB.x), Random.Range(minB.y, maxB.y));
    }

    private void LookAtCenter()
    {
        if (rectTransform == null) return;
        Vector2 directionToCenter = Vector2.zero - rectTransform.anchoredPosition;
        float angle = Mathf.Atan2(directionToCenter.y, directionToCenter.x) * Mathf.Rad2Deg;
        rectTransform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void Clicked()
    {
        if (qteManager != null) qteManager.OnHandClicked();
        Destroy(gameObject);
    }
}