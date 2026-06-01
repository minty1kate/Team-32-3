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
        public StepType currentStep;

        [Header("Система Диалогов")]
        [SerializeField] private DialogueManager dialogueManager;

        [Header("Настройки графики")]
        public GameObject blackOverlay;

        [Header("Настройки графики (2 этап)")]
        public SpriteRenderer paintingRenderer;
        public Color glowColor = new Color(1f, 0.5f, 1f, 1f);

        [Header("Настройки графики (2 этап)")]
        public GameObject backgroundObject;

        [Header("Ссылки на NPC")]
        public NPCMovement npcScript;
        public Transform exitPoint;

        [Header("Подсказка")]
        public GameObject hintObject;

        [Header("Настройки Пятнашек (для этапа ThirdStep_Puzzle)")]
        [SerializeField] private GameObject puzzleWindow;

        private bool _canInteract = false;
        private bool _eventTriggered = false;

        public static bool IsFirstStepCompleted { get; private set; } = false;
        public static bool IsPuzzleCompleted { get; private set; } = false;

        void Start()
        {
            if (currentStep == StepType.FirstStep_Rotate)
            {
                IsFirstStepCompleted = false;
                IsPuzzleCompleted = false;

                // Передаем задачи гостиной в TaskManager прямо отсюда!
                TaskManager tm = FindAnyObjectByType<TaskManager>();
                if (tm != null)
                {
                    string[] livingRoomTasks = new string[]
                    {
                        "Напугать существо",
                        "Собрать картину",
                        "Уронить книжки",
                        "Проверить кухню"
                    };
                    tm.SetSceneTasks(livingRoomTasks);
                }
            }
        }

        void Update()
        {
            if (_canInteract && Input.GetKeyDown(KeyCode.E))
            {
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

                if (!_eventTriggered)
                {
                    Debug.Log("Все проверки пройдены successfully! Запускаю корутину.");
                    StartCoroutine(ExecuteStep());
                }
            }
        }

        IEnumerator ExecuteStep()
        {
            if (hintObject != null) hintObject.SetActive(false);

            if (currentStep == StepType.FirstStep_Rotate)
            {
                _eventTriggered = true;

                if (blackOverlay != null) blackOverlay.SetActive(true);
                yield return new WaitForSeconds(2f);
                if (blackOverlay != null) blackOverlay.SetActive(false);

                yield return new WaitForSeconds(0.5f);

                if (npcScript != null)
                {
                    npcScript.StartConfused();
                }

                yield return new WaitForSeconds(1f);

                if (dialogueManager != null)
                {
                    TogglePlayerMovement(false);

                    string[] lightDialogue = new string[]
                    {
                        "Я проскочил прямо у него перед носом, а он даже не повернулся...",
                        "Но что это за жуткая картина на стене? Раньше её здесь не было!",
                        "Этот чернокнижник запечатывает мою душу в холст!"
                    };

                    dialogueManager.StartTutorial(lightDialogue);

                    if (dialogueManager.closeButton != null)
                    {
                        dialogueManager.closeButton.onClick.RemoveAllListeners();
                        dialogueManager.closeButton.onClick.AddListener(dialogueManager.CloseDialogue);
                        dialogueManager.closeButton.onClick.AddListener(OnFirstStepDialogueFinished);
                    }
                }
                else
                {
                    OnFirstStepDialogueFinished();
                }
            }
            else if (currentStep == StepType.SecondStep_Puzzle)
            {
                if (puzzleWindow != null)
                {
                    puzzleWindow.SetActive(true);
                }
            }
            else if (currentStep == StepType.FinalStep_RunAway)
            {
                _eventTriggered = true;

                if (backgroundObject != null)
                {
                    backgroundObject.SetActive(false);
                }

                yield return new WaitForSeconds(0.2f);

                if (npcScript != null)
                {
                    npcScript.StartFeeling(exitPoint);
                }

                yield return new WaitForSeconds(1.5f);

                if (dialogueManager != null)
                {
                    TogglePlayerMovement(false);

                    string[] runAwayDialogue = new string[]
                    {
                        "Да! Получилось! Уноси свои ноги, расхититель чужих жизней!",
                        "Гримуары на полу, проклятый круг разорван... Дом снова принадлежит мне.",
                        "*Грохот из глубины коридора*",
                        "Погоди... А это еще что за чертовщина? Звук шел со стороны кухни...",
                        "Будто там кто-то яростно скребется по кафелю... Надо проверить."
                    };

                    dialogueManager.StartTutorial(runAwayDialogue);

                    if (dialogueManager.closeButton != null)
                    {
                        dialogueManager.closeButton.onClick.RemoveAllListeners();
                        dialogueManager.closeButton.onClick.AddListener(dialogueManager.CloseDialogue);
                        dialogueManager.closeButton.onClick.AddListener(OnFinalDialogueFinished);
                    }
                }
                else
                {
                    OnFinalDialogueFinished();
                }
            }
        }

        private void OnFirstStepDialogueFinished()
        {
            TogglePlayerMovement(true);
            IsFirstStepCompleted = true;

            TaskManager tm = FindAnyObjectByType<TaskManager>();
            if (tm != null) tm.CompleteCurrentTask();
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

            if (npcScript != null)
            {
                npcScript.MakeRotationFaster();
            }

            if (dialogueManager != null)
            {
                TogglePlayerMovement(false);

                string[] puzzleDialogue = new string[]
                {
                    "И эти книги на полках... Они подпитывают его проклятие, я кожей чувствую их энергию!",
                    "Нет... я не позволю разрушить мой дом!",
                    "Я разорву этот круг, я вышвырну все его черные гримуары из шкафа!"
                };

                dialogueManager.StartTutorial(puzzleDialogue);

                if (dialogueManager.closeButton != null)
                {
                    dialogueManager.closeButton.onClick.RemoveAllListeners();
                    dialogueManager.closeButton.onClick.AddListener(dialogueManager.CloseDialogue);
                    dialogueManager.closeButton.onClick.AddListener(OnPuzzleDialogueFinished);
                }
            }
        }

        private void OnPuzzleDialogueFinished()
        {
            TogglePlayerMovement(true);

            TaskManager tm = FindAnyObjectByType<TaskManager>();
            if (tm != null) tm.CompleteCurrentTask();
        }

        private void OnFinalDialogueFinished()
        {
            TogglePlayerMovement(true);

            TaskManager tm = FindAnyObjectByType<TaskManager>();
            if (tm != null) tm.CompleteCurrentTask();
        }

        private void TogglePlayerMovement(bool state)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                Player_Movement movement = player.GetComponent<Player_Movement>();
                if (movement != null) movement.enabled = state;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && !_eventTriggered)
            {
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