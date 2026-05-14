using System.Collections;
using UnityEngine;

namespace DefaultNamespace
{
    public class Living_Events : MonoBehaviour
    {
        public enum StepType
        {
            FirstStep_Rotate,
            SecondStep_RunAway
        }

        [Header("Настройки этапа")] 
        public StepType currentStep; // Выберите тип триггера в инспекторе для каждого объекта

        [Header("Настройки графики")]
        public GameObject blackOverlay; // Черный фон на весь экран (нужен для FirstStep)

        [Header("Настройки графики (2 этап)")]
        public GameObject backgroundObject;
        
        [Header("Ссылки на NPC")] 
        public NPCMovement npcScript; // Ссылка на скрипт NPC
        public Transform exitPoint;   // Точка побега (нужна только для второго триггера)

        [Header("Подсказка")] 
        public GameObject hintObject;  // Объект надписи "Нажми E"

        private bool _canInteract = false;    // Находится ли игрок в зоне
        private bool _eventTriggered = false; // Защита от повторного нажатия

        // Глобальный флаг-замок: проверяет, завершился ли первый этап
        public static bool IsFirstStepCompleted { get; private set; } = false;

        void Start()
        {
            // Сбрасываем состояние при перезапуске сцены
            if (currentStep == StepType.FirstStep_Rotate)
            {
                IsFirstStepCompleted = false;
            }
        }

        void Update()
        {
            // Если это второй шаг, кнопка сработает только после выполнения первого шага
            if (currentStep == StepType.SecondStep_RunAway && !IsFirstStepCompleted) return;

            if (_canInteract && Input.GetKeyDown(KeyCode.E) && !_eventTriggered)
            {
                StartCoroutine(ExecuteStep());
            }
        }

        IEnumerator ExecuteStep()
        {
            _eventTriggered = true;

            if (hintObject != null) hintObject.SetActive(false);

            // ЛОГИКА ДЛЯ ПЕРВОГО ТРИГГЕРА (Мигание света + Кручение NPC)
            if (currentStep == StepType.FirstStep_Rotate)
            {
                // 1. Включаем темноту
                if (blackOverlay != null) blackOverlay.SetActive(true);

                // 2. Ждем 2 секунды в темноте
                yield return new WaitForSeconds(2f);

                // 3. Выключаем темноту
                if (blackOverlay != null) blackOverlay.SetActive(false);

                // Пауза перед реакцией NPC
                yield return new WaitForSeconds(1.5f);

                // 4. NPC начинает вертеться
                if (npcScript != null)
                {
                    npcScript.StartConfused();
                }

                // Открываем замок для второго триггера
                IsFirstStepCompleted = true;
            }
            // ЛОГИКА ДЛЯ ВТОРОГО ТРИГГЕРА (NPC убегает)
            else if (currentStep == StepType.SecondStep_RunAway)
            {
                if (backgroundObject != null)
                {
                    backgroundObject.SetActive(false);
                }
                
                yield return new WaitForSeconds(0.2f);

                if (npcScript != null)
                {
                    npcScript.StartFeeling(exitPoint);
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && !_eventTriggered)
            {
                // Если это второй триггер, не даем войти в него, пока не нажат первый
                if (currentStep == StepType.SecondStep_RunAway && !IsFirstStepCompleted) return;

                _canInteract = true;
                if (hintObject != null) hintObject.SetActive(true);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _canInteract = false;
                if (hintObject != null) hintObject.SetActive(false);
            }
        }
    }
}