using UnityEngine;
using System.Collections;

public class LightControl : MonoBehaviour
{
    public GameObject blackOverlay; // Тот самый черный фон на весь экран
    public DefaultNamespace.NPCMovement Brother; // Ссылка на твоего NPC
    public Transform exitPoint;    // Точка у двери, куда бежать
    public GameObject hintObject;  // Подсказка "Нажми E"

    private bool _canInteract = false;
    private bool _eventStarted = false;

    void Update()
    {
        // Игрок может нажать свет только если рядом и событие еще не произошло
        if (_canInteract && Input.GetKeyDown(KeyCode.E) && !_eventStarted)
        {
            StartCoroutine(LightEventRoutine());
        }
    }

    IEnumerator LightEventRoutine()
    {
        _eventStarted = true;
        if (hintObject != null) hintObject.SetActive(false);

        // 1. Выключаем свет (включаем черный экран)
        blackOverlay.SetActive(true);

        // 2. Ждем 2 секунды в полной темноте
        yield return new WaitForSeconds(2f);

        // 3. Включаем свет
        blackOverlay.SetActive(false);
        
        yield return new WaitForSeconds(1.5f);

        // 4. NPC пугается и бежит к двери
        if (Brother != null)
        {
            Brother.StopParticles();
            Brother.StartConfused();
        }
    }

    // Используем твою логику триггеров
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) _canInteract = true;
        if (hintObject != null && !_eventStarted) hintObject.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) _canInteract = false;
        if (hintObject != null) hintObject.SetActive(false);
    }
}