using UnityEngine;

public class HallManager : MonoBehaviour
{
    public int totalCandles = 5;
    private int _extinguishedCount = 0;
    public GameObject doorToBasement; // Дверь в подвал

    public void CandleExtinguished()
    {
        _extinguishedCount++;
        Debug.Log("Погашено свечей: " + _extinguishedCount);

        if (_extinguishedCount >= totalCandles)
        {
            OpenBasement();
        }
    }

    void OpenBasement()
    {
        // Логика открытия двери (например, смена спрайта или SetActive(false))
        doorToBasement.SetActive(false);
        Debug.Log("Путь в подвал открыт!");
    }
}