using System.Collections;
using UnityEngine;

namespace DefaultNamespace
{
    public class Living_Events : MonoBehaviour
    {
        public enum StepType
        {
            FirstStep_Rotate,
            SecondStep_Puzzle,
            FinalStep_RunAway
        }
        public ParticleSystem _ps;

        [Header("Настройки этапа")] 
        public StepType currentStep; // Выберите тип триггера в инспекторе для каждого объекта

        [Header("Настройки графики")]
        public GameObject blackOverlay; // Черный фон на весь экран (нужен для FirstStep)

        [Header("Настройки графики (2 этап)")]
        public SpriteRenderer paintingRenderer; // Ссылка на спрайт картины на стене
        public Color glowColor = new Color(1f, 0.5f, 1f, 1f);
        
        [Header("Настройки графики (2 этап)")]
        public GameObject backgroundObject;
        
        [Header("Ссылки на NPC")] 
        public NPCMovement npcScript; // Ссылка на скрипт NPC
        public Transform exitPoint;   // Точка побега (нужна только для второго триггера)

        [Header("Подсказка")] 
        public GameObject hintObject;  // Объект надписи "Нажми E"

        [Header("Настройки Пятнашек (для этапа ThirdStep_Puzzle)")]
        [SerializeField] private GameObject puzzleWindow;
        
        private bool _canInteract = false;    // Находится ли игрок в зоне
        private bool _eventTriggered = false; // Защита от повторного нажатия

        // Глобальный флаг-замок: проверяет, завершился ли первый этап
        public static bool IsFirstStepCompleted { get; private set; } = false;
        public static bool IsPuzzleCompleted { get; private set; } = false;

        void Start()
        {
            // Сбрасываем состояние при перезапуске сцены
            if (currentStep == StepType.FirstStep_Rotate)
            {
                IsFirstStepCompleted = false;
                IsPuzzleCompleted = false;
            }
        }

        void Update()
        {
            if (_canInteract && Input.GetKeyDown(KeyCode.E))
            {
                // Если это второй шаг, кнопка сработает только после выполнения первого шага
                if (currentStep == StepType.SecondStep_Puzzle && !IsFirstStepCompleted)
                {
                    Debug.Log($"Нажата Е! Текущий шаг объекта: {currentStep}. Свет выключен: {IsFirstStepCompleted}");
                    return;
                }

                if (currentStep == StepType.FinalStep_RunAway && !IsPuzzleCompleted)
                {
                    Debug.LogWarning("Шкаф заблокирован, потому что пазл еще не собран!");
                    return;
                }

                if (_canInteract && Input.GetKeyDown(KeyCode.E) && !_eventTriggered)
                {
                    Debug.Log("Все проверки пройдены successfully! Запускаю корутину.");
                    StartCoroutine(ExecuteStep());
                }
            }
        }

        IEnumerator ExecuteStep()
        {
            if (hintObject != null) hintObject.SetActive(false);

            // ЛОГИКА ДЛЯ ПЕРВОГО ТРИГГЕРА (Мигание света + Кручение NPC)
            if (currentStep == StepType.FirstStep_Rotate)
            {
                _eventTriggered = true;
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
            else if (currentStep == StepType.SecondStep_Puzzle)
            {
                if (puzzleWindow != null)
                {
                    puzzleWindow.SetActive(true); // Включаем UI пятнашек
                }
                
                // ВАЖНО: мы НЕ ставим тут флаг IsPuzzleCompleted = true. 
                // Его поставит сам менеджер пятнашек, ТОЛЬКО когда игрок выиграет!
            }
            // ЛОГИКА ДЛЯ ВТОРОГО ТРИГГЕРА (NPC убегает)
            else if (currentStep == StepType.FinalStep_RunAway)
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
        
        public void CompletePuzzleStep()
        {
            _eventTriggered = true;
            IsPuzzleCompleted = true;
            Debug.Log("Доступ к финальному испугу открыт!");
            
            if (paintingRenderer != null)
            {
                paintingRenderer.color = glowColor;
                _ps.Play();
            }

            // 2. Заставляем NPC крутиться ЕЩЕ быстрее
            if (npcScript != null)
            {
                // Для этого в вашем скрипте NPCMovement должна быть функция увеличения скорости.
                // Ниже мы добавим её поддержку.
                npcScript.Invoke("MakeRotationFaster", 0.5f); 
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && !_eventTriggered)
            {
                // Если это второй триггер, не даем войти в него, пока не нажат первый
                if (currentStep == StepType.SecondStep_Puzzle && !IsFirstStepCompleted) return;
                if (currentStep == StepType.FinalStep_RunAway && !IsPuzzleCompleted) return;

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