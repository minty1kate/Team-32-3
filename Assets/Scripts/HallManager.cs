using System.Collections;
using UnityEngine;
using TMPro; // Обязательно для работы с TextMeshPro

public class HallManager : MonoBehaviour
{
    public int totalCandles = 5;
    private int _extinguishedCount = 0;

    [Header("Ссылки на объекты")]
    public GameObject doorToBasement; // Дверь в подвал
    public ExorcistPatrol exorcist;   // Ссылка на экзорциста

    [Header("Настройки Обучения")]
    public GameObject tutorialPanel; // Черная панель обучения на весь экран
    public TextMeshProUGUI tutorialText; // Компонент текста внутри панели
    public float delayBeforeTutorial = 1f; // Задержка перед появлением плашки

    private Candle[] _allCandles; // Массив для автоматического поиска всех свечей на сцене
    private bool _isTutorialActive = false;

    void Start()
    {
        // Автоматически находим все компоненты Candle на сцене
        _allCandles = FindObjectsByType<Candle>(FindObjectsSortMode.None);

        // Убедимся, что плашка обучения изначально выключена
        if (tutorialPanel != null)
            tutorialPanel.SetActive(false);
    }

    void Update()
    {
        // Если обучение открыто и игрок нажимает любую клавишу — закрываем плашку
        if (_isTutorialActive && Input.anyKeyDown)
        {
            EndTutorial();
        }
    }

    // --- ЛОГИКА ОБУЧЕНИЯ ---

    // Метод вызывается из DialogueManager автоматически после закрытия монолога
    public void StartTutorialSequence()
    {
        StartCoroutine(ShowTutorialWithDelay());
    }

    private IEnumerator ShowTutorialWithDelay()
    {
        yield return new WaitForSeconds(delayBeforeTutorial);

        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(true);

            if (tutorialText != null)
            {
                tutorialText.text = "<b>ЦЕЛЬ: Победить Экзорциста</b>\n\n" +
                                    "• <color=red>Потуши все 5 свечей</color> на локации, чтобы снять барьер с двери.\n\n" +
                                    "• Экзорцист патрулирует коридоры. Не попадайся ему на глаза — вовремя <color=yellow>прячься в укрытия и за мебель</color>.\n\n" +
                                    "• С каждой потушенной свечой Экзорцист злится и <b>двигается быстрее</b>!\n\n" +
                                    "• Если он тебя поймает, прогресс сбросится, а уровень начнется сначала.\n\n" +
                                    "<size=80%><color=#555555>[ Нажми любую клавишу, чтобы начать ]</color></size>";
            }

            _isTutorialActive = true;
        }
    }

    private void EndTutorial()
    {
        _isTutorialActive = false;
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
        }
    }

    // --- ЛОГИКА СВЕЧЕЙ ---

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
                candle.ResetCandle();
            }
        }
    }

    void OpenBasement()
    {
        if (doorToBasement != null) doorToBasement.SetActive(false);
        Debug.Log("Путь в подвал открыт!");
    }
}