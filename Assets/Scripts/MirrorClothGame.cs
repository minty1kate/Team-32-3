using UnityEngine;
using UnityEngine.UI;

public class MirrorClothGame : MonoBehaviour
{
    [Header("UI элементы")]
    [SerializeField] private Slider clothSlider;        
    [SerializeField] private TableTrigger tableTrigger; 

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

        // 1. Логика хаотичного призрачного ветра
        windTimer += Time.deltaTime;
        if (windTimer >= windFrequencyCurrent)
        {
            currentWindPulse = Random.Range(0.2f, windForce);
            windTimer = 0f;
            windFrequencyCurrent = Random.Range(0.6f, 1.6f); 
        }

        currentWindPulse = Mathf.MoveTowards(currentWindPulse, 0f, Time.deltaTime * 1.5f);

        // 2. Считываем относительное движение мыши ВВЕРХ
        if (Input.GetMouseButton(0) && activationTimer > 0.3f)
        {
            float mouseY = Input.GetAxis("Mouse Y"); 

            // ИЗМЕНЕНО: Если mouseY положительный — значит мышь движется ВВЕРХ
            if (mouseY > 0f)
            {
                isHolding = true;

                // Переводим сдвиг мыши в скорость
                float moveSpeed = Mathf.Abs(mouseY) * pullSensitivity;

                // Проверяем на слишком резкий рывок
                if (moveSpeed / Time.deltaTime > maxSafeSpeed * 100f)
                {
                    BreakGrip();
                }
                else
                {
                    // Двигаем ткань (уменьшаем значение от 1 к 0)
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

        // 3. Силы, тянущие ткань обратно наверх (в сторону 1)
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

        // 4. ПРОВЕРКА ПОБЕДЫ
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

    private System.Collections.IEnumerator WaitAndCloseScreen()
    {
        yield return new WaitForSeconds(3.0f);

        Canvas parentCanvas = GetComponentInParent<Canvas>();
        if (parentCanvas != null)
        {
            parentCanvas.gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(false); 
        }

        if (tableTrigger != null)
        {
            tableTrigger.EndMiniGameAndOpenNewspaper(); 
        }
    }
}



