using UnityEngine;
using UnityEngine.UI;

public class GhostHandGame : MonoBehaviour
{
    [Header("UI элементы")]
    [SerializeField] private Slider uiSlider;          // Ссылка на компонент Slider
    [SerializeField] private RectTransform greenZone;   // Визуальная зеленая зона (чтобы менять её размер)
    [SerializeField] private TableTrigger tableTrigger; // Ссылка на первый скрипт для завершения

    [Header("Настройки визуала газеты")]
    [SerializeField] private Image newspaperImageUI;   // Ссылка на картинку газеты на Canvas
    [SerializeField] private Sprite[] newspaperSprites;
    
    [Header("Настройки механики")]
    [SerializeField] private float currentSpeed = 2.5f;
    [SerializeField] private float phase1Speed = 2.5f;
    [SerializeField] private float phase2Speed = 4.5f;
    [SerializeField] private float phase3Speed = 6.5f;
    public static bool isNewspaperPassed = false; 
    
    // Границы зеленой зоны (в диапазоне от -1 до 1 слайдера)
    private float greenMin = -0.1f;
    private float greenMax = 0.1f;

    private float sliderValue = 0f;
    private int direction = 1;
    private int currentPhase = 1;
    private bool isAnimating = false;

    private void OnEnable()
    {
        // Сброс параметров при каждом открытии окна
        sliderValue = 0f;
        currentPhase = 1;
        currentSpeed = phase1Speed;
        greenMin = -0.1f;
        greenMax = 0.1f;
        isAnimating = false;
        
        UpdateGreenZoneVisual();
        UpdateNewspaperSprite();
    }

    private void Update()
    {
        if (isAnimating) return;

        // 1. Движение ползунка туда-сюда
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

        // 2. Клик ЛКМ
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
            // ЭТАП 1: Рука проходит сквозь газету в первый раз
            Debug.Log("Фаза 1: Успешный клик. Рука прошла сквозь газету.");
            Invoke(nameof(ToPhase2), 1.5f); // Задержка на анимацию/эффект
        }
        else if (currentPhase == 2)
        {
            // ЭТАП 2: Вторая попытка, рука снова не может ухватиться или газета выскальзывает
            Debug.Log("Фаза 2: Успешный клик. Газета всё ещё не поддаётся!");
            Invoke(nameof(ToPhase3), 1.5f); // Переходим к финальному этапу
        }
        else if (currentPhase == 3)
        {
            // ЭТАП 3: Финальный триумф, превозмогая призрачную немощь, ГГ берёт газету
            Debug.Log("Фаза 3: Финальный клик! Газета успешно взята в руки.");
            Invoke(nameof(FinishGame), 1.0f);
        }
    }

    private void MissClick()
    {
        // Здесь можно запустить анимацию дрожания руки от неудачной попытки
        Debug.Log("Промах по шкале!");
    }

    private void ToPhase2()
    {
        currentPhase = 2;
        currentSpeed = phase2Speed; // Ползунок ускоряется
        
        // Сужаем зону для второго этапа
        greenMin = -0.07f;
        greenMax = 0.07f;
        
        UpdateGreenZoneVisual();
        UpdateNewspaperSprite();
        Debug.Log("Мысли ГГ (Фаза 2): Что за чертовщина?.. Я её не чувствую!");
        
        isAnimating = false;
    }

    private void ToPhase3()
    {
        currentPhase = 3;
        currentSpeed = phase3Speed; // Ползунок летит очень быстро!
        
        // Экстремально сужаем зону для финального этапа (сложно промахнуться)
        greenMin = -0.04f;
        greenMax = 0.04f;
        
        UpdateGreenZoneVisual();
        UpdateNewspaperSprite();
        Debug.Log("Мысли ГГ (Фаза 3): Ну же! Соберись! Возьми её!");
        
        isAnimating = false;
    }

    private void FinishGame()
    {
        isNewspaperPassed = true;
        gameObject.SetActive(false);

        // Также вызываем метод у триггера стола, чтобы он знал, что игра завершена
        if (tableTrigger != null)
        {
            tableTrigger.EndMiniGameAndOpenNewspaper();
        }

        Debug.Log("Мини-игра закрыта. Сюда можно подключать показ финальных фраз героя.");
    }

    private void UpdateGreenZoneVisual()
    {
        if (greenZone != null && uiSlider != null)
        {
            // Получаем полную ширину всей шкалы слайдера в пикселях
            float totalWidth = uiSlider.GetComponent<RectTransform>().rect.width;
        
            // Рассчитываем долю, которую должна занимать зона (из диапазона шкалы от -1 до 1, то есть всего 2 единицы)
            float zoneShare = (greenMax - greenMin) / 2f; 
        
            // Меняем ТОЛЬКО ширину (X). Высоту (Y) оставляем ровно такой, какую вы задали в редакторе!
            greenZone.sizeDelta = new Vector2(totalWidth * zoneShare, greenZone.sizeDelta.y);
        
            // Принудительно сбрасываем позицию зеленой зоны строго по центру
            greenZone.anchoredPosition = Vector2.zero;
        }
    }
    
    [Header("Масштаб газеты")]
    [Range(0.1f, 2f)] [SerializeField] private float newspaperScale = 1.0f; // Ползунок размера в инспекторе

    private void UpdateNewspaperSprite()
    {
        if (newspaperImageUI != null && newspaperSprites != null && newspaperSprites.Length >= 3)
        {
            // Устанавливаем нужный спрайт
            newspaperImageUI.sprite = newspaperSprites[currentPhase - 1];
            
            // Вместо SetNativeSize задаем масштаб через RectTransform
            RectTransform rect = newspaperImageUI.GetComponent<RectTransform>();
            if (rect != null)
            {
                // Устанавливаем одинаковый масштаб по осям X и Y
                rect.localScale = new Vector3(newspaperScale, newspaperScale, 1f);
            }
        }
    }
}
