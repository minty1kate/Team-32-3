using UnityEngine;

public class UIElementShaker : MonoBehaviour
{
    [Header("Настройки тряски")]
    [SerializeField] private float shakeSpeed = 50f;   // Скорость дрожания
    [SerializeField] private float shakeAmount = 5f;    // Сила/амплитуда дрожания (в пикселях)

    private RectTransform rectTransform;
    private Vector2 originalPosition;

    private void Awake()
    {
        // Получаем компонент RectTransform, который управляет позицией UI-элемента
        rectTransform = GetComponent<RectTransform>();
        // Запоминаем изначальное положение руки на экране
        originalPosition = rectTransform.anchoredPosition;
    }

    private void OnEnable()
    {
        // Сбрасываем позицию к исходной каждый раз, когда открывается мини-игра
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = originalPosition;
        }
    }

    private void Update()
    {
        // Используем Mathf.Sin для создания цикличного хаотичного дрожания по осям X и Y
        float shakeX = Mathf.Sin(Time.time * shakeSpeed) * shakeAmount;
        float shakeY = Mathf.Sin(Time.time * (shakeSpeed * 1.2f)) * shakeAmount;

        // Прибавляем смещение к исходной позиции
        rectTransform.anchoredPosition = originalPosition + new Vector2(shakeX, shakeY);
    }
}