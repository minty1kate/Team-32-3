using UnityEngine;
using TMPro;
using System.Collections;

public class NotificationManager : MonoBehaviour
{
    public TextMeshProUGUI notificationText; // Ссылка на отдельный текст для уведомлений
    public float displayDuration = 2f;     // Сколько секунд текст горит полностью
    public float fadeDuration = 1f;        // Сколько секунд он плавно исчезает

    private Coroutine _fadeCoroutine;

    void Start()
    {
        if (notificationText != null)
        {
            // Скрываем текст при старте сцены
            notificationText.color = new Color(notificationText.color.r, notificationText.color.g, notificationText.color.b, 0f);
        }
    }

    public void ShowNotification(string text)
    {
        if (notificationText == null) return;

        if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);
        _fadeCoroutine = StartCoroutine(FadeTextRoutine(text));
    }

    IEnumerator FadeTextRoutine(string text)
    {
        notificationText.text = text;
        Color textColor = notificationText.color;

        // 1. Делаем текст полностью видимым
        textColor.a = 1f;
        notificationText.color = textColor;

        // 2. Ждем 1-2 секунды
        yield return new WaitForSeconds(displayDuration);

        // 3. Медленно гасим текст через Alpha-канал
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            textColor.a = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            notificationText.color = textColor;
            yield return null;
        }

        // Убеждаемся, что в конце он точно в нуле
        textColor.a = 0f;
        notificationText.color = textColor;
    }
}