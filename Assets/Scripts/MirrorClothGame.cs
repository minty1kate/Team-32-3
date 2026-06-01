using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MirrorClothGame : MonoBehaviour
{
    [Header("UI элементы")]
    [SerializeField] private Slider clothSlider;        
    [SerializeField] private TableTrigger tableTrigger; 

    [Header("Система Диалогов")]
    [SerializeField] private DialogueManager dialogueManager; // Ссылка на менеджер диалогов

    [Header("Настройки механики")]
    [SerializeField] private float baseResistance = 0.4f; 
    [SerializeField] private float pullSensitivity = 0.5f; 
    [SerializeField] private float maxSafeSpeed = 0.8f;    

    [Header("Настройки призрачного ветра")]
    [SerializeField] private float windForce = 1.2f;       
    [SerializeField] private float windFrequency = 1.2f;   

    private bool isHolding = false;
    private bool isGameFinished = false;
    
    private float windTimer = 0f;
    private float currentWindPulse = 0f;
    private float windFrequencyCurrent = 1.2f;

    private float activationTimer = 0f;

    private void OnEnable()
    {
        if (clothSlider != null)
        {
            clothSlider.value = 1f; 
            clothSlider.interactable = false; 
        }
        isHolding = false;
        isGameFinished = false;
        windTimer = 0f;
        currentWindPulse = 0f;
        windFrequencyCurrent = windFrequency;
        activationTimer = 0f;
    }

    private void Update()
    {
        if (clothSlider == null || isGameFinished) return;

        activationTimer += Time.deltaTime;

        windTimer += Time.deltaTime;
        if (windTimer >= windFrequencyCurrent)
        {
            currentWindPulse = Random.Range(0.2f, windForce);
            windTimer = 0f;
            windFrequencyCurrent = Random.Range(0.6f, 1.6f); 
        }

        currentWindPulse = Mathf.MoveTowards(currentWindPulse, 0f, Time.deltaTime * 1.5f);

        if (Input.GetMouseButton(0) && activationTimer > 0.3f)
        {
            float mouseY = Input.GetAxis("Mouse Y"); 

            if (mouseY > 0f)
            {
                isHolding = true;
                float moveSpeed = Mathf.Abs(mouseY) * pullSensitivity;

                if (moveSpeed / Time.deltaTime > maxSafeSpeed * 100f)
                {
                    BreakGrip();
                }
                else
                {
                    clothSlider.value -= moveSpeed;
                }
            }
            else
            {
                isHolding = false;
            }
        }
        else
        {
            isHolding = false; 
        }

        float totalBackwardsForce = baseResistance + currentWindPulse;
        
        if (!isHolding && clothSlider.value < 1f)
        {
            clothSlider.value += totalBackwardsForce * Time.deltaTime;
        }
        else if (isHolding && currentWindPulse > 0.4f)
        {
            clothSlider.value += (currentWindPulse * 0.3f) * Time.deltaTime;
        }

        clothSlider.value = Mathf.Clamp01(clothSlider.value);

        if (clothSlider.value <= 0.01f)
        {
            WinGame();
        }
    }

    private void BreakGrip()
    {
        isHolding = false;
        clothSlider.value = Mathf.MoveTowards(clothSlider.value, 1f, 0.3f);
        Debug.Log("Слишком резко! Ткань вырвало ветром.");
    }

    private void WinGame()
    {
        isGameFinished = true;
        isHolding = false;
        clothSlider.value = 0f; 
        StartCoroutine(WaitAndCloseScreen());
    }

    private IEnumerator WaitAndCloseScreen()
    {
        if (clothSlider != null) clothSlider.interactable = false;

        // Ждем 3 секунды, пока игрок смотрит в пустое зеркало
        yield return new WaitForSeconds(3.0f);

        // Закрываем окно мини-игры с зеркалом
        Canvas parentCanvas = GetComponentInParent<Canvas>();
        if (parentCanvas != null) parentCanvas.gameObject.SetActive(false);
        else gameObject.SetActive(false);

        // ЗАПУСК ИТОГОВОГО МОНОЛОГА
        if (dialogueManager != null)
        {
            string[] linesAfterMirror = new string[]
            {
                "Где я? Почему там... только пустая комната?",
                "Пожалуйста, пусть это будет сном... проснись... ПРОСНИСЬ!",
                "Господи... я вижу доски пола сквозь собственные ладони...",
                "Я... пропадаю... меня здесь нет"
            };

            // Блокируем скрипт движения перед выводом текста
            if (tableTrigger != null && tableTrigger.GetPlayerObject() != null)
            {
                Player_Movement movement = tableTrigger.GetPlayerObject().GetComponent<Player_Movement>();
                if (movement != null) movement.enabled = false;
            }

            dialogueManager.StartTutorial(linesAfterMirror);

            // Настраиваем кнопку закрытия, чтобы запустить исчезновение персонажа
            if (dialogueManager.closeButton != null)
            {
                dialogueManager.closeButton.onClick.RemoveAllListeners();
                dialogueManager.closeButton.onClick.AddListener(dialogueManager.CloseDialogue);
                dialogueManager.closeButton.onClick.AddListener(StartPlayerFade);
            }
        }
        else
        {
            StartPlayerFade();
        }
    }

    private void StartPlayerFade()
    {
        StartCoroutine(FadePlayerAndLoadEndScene());
    }

    private IEnumerator FadePlayerAndLoadEndScene()
    {
        // ПЛАВНОЕ ИСЧЕЗНОВЕНИЕ (РАСТВОРЕНИЕ) ПЕРСОНАЖА ПОВЕРХ ЗАКРЫТОГО ДИАЛОГА
        if (tableTrigger != null && tableTrigger.GetPlayerObject() != null)
        {
            GameObject playerObj = tableTrigger.GetPlayerObject();
            SpriteRenderer playerSprite = playerObj.GetComponent<SpriteRenderer>();
            
            if (playerSprite != null)
            {
                float fadeDuration = 3.0f; 
                float startAlpha = playerSprite.color.a;

                for (float t = 0; t < fadeDuration; t += Time.deltaTime)
                {
                    float normalizedTime = t / fadeDuration;
                    float newAlpha = Mathf.Lerp(startAlpha, 0f, normalizedTime);

                    playerSprite.color = new Color(playerSprite.color.r, playerSprite.color.g, playerSprite.color.b, newAlpha);
                    yield return null; 
                }

                playerSprite.color = new Color(playerSprite.color.r, playerSprite.color.g, playerSprite.color.b, 0f);
            }
        }

        yield return new WaitForSeconds(1.5f);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Финал"); 
    }
}