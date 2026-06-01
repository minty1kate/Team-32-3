using UnityEngine;
using UnityEngine.UI;

public class GhostHandGame : MonoBehaviour
{
    [Header("UI элементы")]
    [SerializeField] private Slider uiSlider;
    [SerializeField] private RectTransform greenZone;
    [SerializeField] private TableTrigger tableTrigger;

    [Header("Система Диалогов")]
    [SerializeField] private DialogueManager dialogueManager;

    [Header("Настройки визуала газеты")]
    [SerializeField] private Image newspaperImageUI;
    [SerializeField] private Sprite[] newspaperSprites;

    [Header("Настройки механики")]
    [SerializeField] private float currentSpeed = 2.5f;
    [SerializeField] private float phase1Speed = 2.5f;
    [SerializeField] private float phase2Speed = 4.5f;
    [SerializeField] private float phase3Speed = 6.5f;
    public static bool isNewspaperPassed = false;

    private float greenMin = -0.1f;
    private float greenMax = 0.1f;

    private float sliderValue = 0f;
    private int direction = 1;
    private int currentPhase = 1;
    private bool isAnimating = false;
    private bool isDialogueActive = false; // Блокировка ползунка во время промежуточных монологов

    private void OnEnable()
    {
        sliderValue = 0f;
        currentPhase = 1;
        currentSpeed = phase1Speed;
        greenMin = -0.1f;
        greenMax = 0.1f;
        isAnimating = false;
        isDialogueActive = false;

        UpdateGreenZoneVisual();
        UpdateNewspaperSprite();
    }

    private void Update()
    {
        if (isAnimating || isDialogueActive) return;

        sliderValue += direction * currentSpeed * Time.deltaTime;

        if (sliderValue >= 1f)
        {
            sliderValue = 1f;
            direction = -1;
        }
        else if (sliderValue <= -1f)
        {
            sliderValue = -1f;
            direction = 1;
        }

        uiSlider.value = sliderValue;

        if (Input.GetMouseButtonDown(0))
        {
            if (sliderValue >= greenMin && sliderValue <= greenMax)
            {
                HitGreenZone();
            }
            else
            {
                MissClick();
            }
        }
    }

    private void HitGreenZone()
    {
        isAnimating = true;

        if (currentPhase == 1)
        {
            Debug.Log("Фаза 1: Успешный клик. Рука прошла сквозь газету.");
            Invoke(nameof(ShowPhase1Dialogue), 1.0f);
        }
        else if (currentPhase == 2)
        {
            Debug.Log("Фаза 2: Успешный клик. Газета всё ещё не поддаётся!");
            Invoke(nameof(ShowPhase2Dialogue), 1.0f);
        }
        else if (currentPhase == 3)
        {
            Debug.Log("Фаза 3: Финальный клик! Газета успешно взята в руки.");
            Invoke(nameof(FinishGame), 1.0f);
        }
    }

    private void MissClick()
    {
        Debug.Log("Промах по шкале!");
    }

    // --- ДИАЛОГ ПОСЛЕ 1 ФАЗЫ ---
    private void ShowPhase1Dialogue()
    {
        if (dialogueManager != null)
        {
            isDialogueActive = true;
            string[] lines = new string[]
            {
                "Я... я не чувствую бумаги.",
                "Пальцы прошли сквозь нее, как сквозь дым...",
                "Что со мной?"
            };

            dialogueManager.StartTutorial(lines);

            if (dialogueManager.closeButton != null)
            {
                dialogueManager.closeButton.onClick.RemoveAllListeners();
                dialogueManager.closeButton.onClick.AddListener(dialogueManager.CloseDialogue);
                dialogueManager.closeButton.onClick.AddListener(ToPhase2); // По кнопке закрыть переходим к фазе 2
            }
        }
        else
        {
            ToPhase2();
        }
    }

    private void ToPhase2()
    {
        currentPhase = 2;
        currentSpeed = phase2Speed;

        greenMin = -0.07f;
        greenMax = 0.07f;

        UpdateGreenZoneVisual();
        UpdateNewspaperSprite();

        isAnimating = false;
        isDialogueActive = false; // Запускаем ползунок снова
    }

    // --- ДИАЛОГ ПОСЯЛЕ 2 ФАЗЫ ---
    private void ShowPhase2Dialogue()
    {
        if (dialogueManager != null)
        {
            isDialogueActive = true;
            string[] lines = new string[]
            {
                "Проклятый дом сводит меня с ума!",
                "Мне нужно прочесть... нужно взять газету!"
            };

            dialogueManager.StartTutorial(lines);

            if (dialogueManager.closeButton != null)
            {
                dialogueManager.closeButton.onClick.RemoveAllListeners();
                dialogueManager.closeButton.onClick.AddListener(dialogueManager.CloseDialogue);
                dialogueManager.closeButton.onClick.AddListener(ToPhase3); // По кнопке закрыть переходим к фазе 3
            }
        }
        else
        {
            ToPhase3();
        }
    }

    private void ToPhase3()
    {
        currentPhase = 3;
        currentSpeed = phase3Speed;

        greenMin = -0.04f;
        greenMax = 0.04f;

        UpdateGreenZoneVisual();
        UpdateNewspaperSprite();

        isAnimating = false;
        isDialogueActive = false; // Запускаем ползунок снова
    }

    // --- ФИНАЛ МИНИ-ИГРЫ ---
    private void FinishGame()
    {
        isNewspaperPassed = true;
        gameObject.SetActive(false);

        if (tableTrigger != null)
        {
            tableTrigger.EndMiniGameAndOpenNewspaper();
        }

        if (dialogueManager != null)
        {
            string[] linesAfterGame = new string[]
            {
                "Это... моё лицо на снимке?",
                "Какая глупая шутка... Я ведь стою здесь!",
                "Нет-нет-нет, это не могу быть я.",
                "Зеркало... надо посмотреть в зеркало, оно под тканью.."
            };

            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                Player_Movement movement = player.GetComponent<Player_Movement>();
                if (movement != null) movement.enabled = false;
            }

            dialogueManager.StartTutorial(linesAfterGame);

            if (dialogueManager.closeButton != null)
            {
                dialogueManager.closeButton.onClick.RemoveAllListeners();
                dialogueManager.closeButton.onClick.AddListener(dialogueManager.CloseDialogue);
                dialogueManager.closeButton.onClick.AddListener(EnablePlayerMovement);
            }
        }
    }

    private void EnablePlayerMovement()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            Player_Movement movement = player.GetComponent<Player_Movement>();
            if (movement != null) movement.enabled = true;
        }
    }

    private void UpdateGreenZoneVisual()
    {
        if (greenZone != null && uiSlider != null)
        {
            float totalWidth = uiSlider.GetComponent<RectTransform>().rect.width;
            float zoneShare = (greenMax - greenMin) / 2f;
            greenZone.sizeDelta = new Vector2(totalWidth * zoneShare, greenZone.sizeDelta.y);
            greenZone.anchoredPosition = Vector2.zero;
        }
    }

    [Header("Масштаб газеты")]
    [Range(0.1f, 2f)][SerializeField] private float newspaperScale = 1.0f;

    private void UpdateNewspaperSprite()
    {
        if (newspaperImageUI != null && newspaperSprites != null && newspaperSprites.Length >= 3)
        {
            newspaperImageUI.sprite = newspaperSprites[currentPhase - 1];
            RectTransform rect = newspaperImageUI.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.localScale = new Vector3(newspaperScale, newspaperScale, 1f);
            }
        }
    }
}