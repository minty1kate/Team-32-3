using UnityEngine;

public class HallManager : MonoBehaviour
{
    public int totalCandles = 5;
    private int _extinguishedCount = 0;

    [Header("Ссылки на объекты")]
    public GameObject doorToBasement; // Дверь в подвал
    public ExorcistPatrol exorcist;   // Ссылка на экзорциста

    private Candle[] _allCandles; // Массив для автоматического поиска всех свечей на сцене

    void Start()
    {
        // Автоматически находим все компоненты Candle на сцене, чтобы потом их сбрасывать
        _allCandles = FindObjectsOfType<Candle>();
    }

    public void CandleExtinguished()
    {
        _extinguishedCount++;
        Debug.Log("Погашено свечей: " + _extinguishedCount);

        if (_extinguishedCount >= totalCandles)
        {
            if (exorcist != null) exorcist.Die(); // Экзорцист умирает
            OpenBasement();
        }
        else
        {
            // Передаем текущее количество погашенных свечей для увеличения скорости и боли
            if (exorcist != null) exorcist.TakeDamage(_extinguishedCount);
        }
    }

    // Вызывается из ExorcistPatrol, когда игрок пойман
    public void ResetAllCandles()
    {
        _extinguishedCount = 0;
        Debug.Log("Игрок пойман! Все свечи зажжены заново.");

        // Проходимся по каждой свече и возвращаем ей зажженное состояние
        foreach (Candle candle in _allCandles)
        {
            if (candle != null)
            {
                candle.ResetCandle(); // Метод ResetCandle нужно добавить в скрипт Candle.cs (см. ниже)
            }
        }
    }

    void OpenBasement()
    {
        if (doorToBasement != null) doorToBasement.SetActive(false);
        Debug.Log("Путь в подвал открыт!");
    }
}