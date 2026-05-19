using System.Collections;
using UnityEngine;

namespace DefaultNamespace
{
    public class ClosetInteract : MonoBehaviour
    {
        [Header("Настройки графики")]
        public GameObject backgroundObject; // Объект заднего фона, который нужно ВЫКЛЮЧИТЬ

        [Header("Ссылки на NPC")] public DefaultNamespace.NPCMovement npcScript; // Ссылка на скрипт вашего NPC
        public Transform exitPoint; // Точка, к которой побежит NPC перед удалением

        private bool _eventTriggered = false; // Защита от повторного срабатывания

        public GameObject hintObject;  // Подсказка "Нажми E"

        private bool _canInteract = false;
        private bool _eventStarted = false;
        
        void Update()
        {
            // Событие начнется, только если игрок внутри зоны, нажал E и событие еще не запускалось
            if (_canInteract && Input.GetKeyDown(KeyCode.E) && !_eventTriggered)
            {
                StartCoroutine(RunZoneEvent());
            }
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player")) _canInteract = true;
            if (hintObject != null && !_eventStarted) hintObject.SetActive(true);
        }

        IEnumerator RunZoneEvent()
        {
            _eventTriggered = true; // Блокируем повторный вход в этот триггер

            if (hintObject != null) hintObject.SetActive(false);
            // 1. Выключаем задний фон игры
            if (backgroundObject != null)
            {
                backgroundObject.SetActive(false);
            }

            // Небольшая пауза (0.2 секунды) для драматического эффекта перед бегством
            yield return new WaitForSeconds(0.2f);

            // 2. Даем команду NPC выключить частицы и убежать
            if (npcScript != null)
            {
                // Используем метод тотальной остановки частиц, который мы написали ранее

                // Включаем логику бегства. Напоминание: внутри MoveTowards вашего NPC 
                // уже встроен метод Destroy(gameObject), который сотрет его при достижении exitPoint.
                npcScript.StartFeeling(exitPoint);
            }
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player")) _canInteract = false;
            if (hintObject != null) hintObject.SetActive(false);
        }
    }
}