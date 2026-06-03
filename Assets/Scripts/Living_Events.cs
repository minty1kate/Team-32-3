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

        [Header("Настройки спрайтов и подсветки")]
        [SerializeField] private SpriteRenderer lampRenderer; // Светильник (1 этап)
        [SerializeField] private SpriteRenderer booksRenderer; // Книги/Шкаф (финальный этап)
        [SerializeField] private Color darkColor = new Color(0.3f, 0.3f, 0.4f, 1f); // Цвет затемнения для светильника и картины

        private Color _originalLampColor;
        private Color _originalPaintingColor;
        private Color _originalBooksColor;

        [Header("Ссылки на NPC")]
        public NPCMovement npcScript;
        public Transform exitPoint;

        [Header("Подсказка")]
        public GameObject hintObject;

        [Header("Настройки Пятнашек (для этапа ThirdStep_Puzzle)")]
        [SerializeField] private GameObject puzzleWindow;

        [Header("Настройки звука")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip finalStepSound;
        [SerializeField] private AudioClip firstStepSound;
        [SerializeField] private AudioClip secondStepSound;
        [SerializeField] private AudioClip initialLoopingSound;
        [SerializeField] private AudioClip postPuzzleLoopingSound;

        private bool _canInteract = false;
        private bool _eventTriggered = false;

        // Сделали изменяемыми извне, чтобы этапы переключались корректно
        public static bool IsFirstStepCompleted { get; set; } = false;
        public static bool IsPuzzleCompleted { get; set; } = false;

        void Start()
        {
            // Сохраняем исходные цвета
            if (lampRenderer != null) _originalLampColor = lampRenderer.color;
            if (paintingRenderer != null) _originalPaintingColor = paintingRenderer.color;

            if (booksRenderer != null)
            {
                _originalBooksColor = booksRenderer.color;
                // Скрываем черный квадрат триггера книг в самом начале
                if (currentStep == StepType.FinalStep_RunAway)
                {
                    booksRenderer.color = new Color(_originalBooksColor.r, _originalBooksColor.g, _originalBooksColor.b, 0f);
                }
            }

            if (audioSource != null && initialLoopingSound != null)
            {
                audioSource.clip = initialLoopingSound;
                audioSource.loop = true;
                audioSource.Play();
            }

            if (currentStep == StepType.FirstStep_Rotate)
            {
                IsFirstStepCompleted = false;
                IsPuzzleCompleted = false;

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
                if (currentStep == StepType.SecondStep_Puzzle && !IsFirstStepCompleted) return;
                if (currentStep == StepType.FinalStep_RunAway && !IsPuzzleCompleted) return;

                if (!_eventTriggered)
                {
                    StartCoroutine(ExecuteStep());
                }
            }
        }

        IEnumerator ExecuteStep()
        {
            if (hintObject != null) hintObject.SetActive(false);

            if (currentStep == StepType.FirstStep_Rotate)
            {
                if (audioSource != null && firstStepSound != null)
                {
                    audioSource.PlayOneShot(firstStepSound);
                }
                _eventTriggered = true;

                if (audioSource != null)
                {
                    audioSource.Stop();
                    audioSource.loop = false;
                }

                if (blackOverlay != null) blackOverlay.SetActive(true);
                yield return new WaitForSeconds(2f);
                if (firstStepSound != null)
                {
                    audioSource.PlayOneShot(firstStepSound);
                }

                if (blackOverlay != null) blackOverlay.SetActive(false);

                if (npcScript != null) npcScript.StartConfused();

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

                if (audioSource != null && finalStepSound != null)
                {
                    audioSource.PlayOneShot(finalStepSound);
                }

                if (backgroundObject != null) backgroundObject.SetActive(false);

                yield return new WaitForSeconds(0.2f);

                if (npcScript != null) npcScript.StartFeeling(exitPoint);

                yield return new WaitForSeconds(1.5f);

                if (dialogueManager != null)
                {
                    TogglePlayerMovement(false);

                    string[] runAwayDialogue = new string[]
                    {
                        "Да! Получилось! Уноси свои ноги, расхититель чужих жизней!",
                        "Гримуары на полу, проклятый круг разорван... Дом снова принадлежит мне.",
                        "*Грохот из глубины коридора*",
                        "Погодите ка... А это еще что за чертовщина? Что за странные звуки...",
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

            // Светильник навсегда возвращает обычный цвет и больше не подсвечивается
            if (lampRenderer != null) lampRenderer.color = _originalLampColor;

            TaskManager tm = FindAnyObjectByType<TaskManager>();
            if (tm != null) tm.CompleteCurrentTask();
        }

        public void CompletePuzzleStep()
        {
            _eventTriggered = true;
            IsPuzzleCompleted = true;

            // Картина возвращает базовый цвет (или твой glowColor)
            if (paintingRenderer != null) paintingRenderer.color = glowColor;

            if (audioSource != null && postPuzzleLoopingSound != null)
            {
                audioSource.Stop();
                audioSource.clip = postPuzzleLoopingSound;
                audioSource.loop = true;
                audioSource.Play();
            }

            if (_ps != null) _ps.Play();

            if (npcScript != null) npcScript.MakeRotationFaster();

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
            if (booksRenderer != null) booksRenderer.color = new Color(_originalBooksColor.r, _originalBooksColor.g, _originalBooksColor.b, 0f);

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
                // ЖЕСТКАЯ ПРОВЕРКА: Подсветка включается ТОЛЬКО если шаг объекта совпадает с текущим этапом игры
                if (currentStep == StepType.FirstStep_Rotate && !IsFirstStepCompleted)
                {
                    _canInteract = true;
                    if (hintObject != null) hintObject.SetActive(true);
                    if (lampRenderer != null) lampRenderer.color = darkColor;
                }
                else if (currentStep == StepType.SecondStep_Puzzle && IsFirstStepCompleted && !IsPuzzleCompleted)
                {
                    _canInteract = true;
                    if (hintObject != null) hintObject.SetActive(true);
                    if (paintingRenderer != null) paintingRenderer.color = darkColor;
                }
                else if (currentStep == StepType.FinalStep_RunAway && IsPuzzleCompleted)
                {
                    _canInteract = true;
                    if (hintObject != null) hintObject.SetActive(true);
                    if (booksRenderer != null) booksRenderer.color = new Color(0f, 0f, 0f, 0.7f); // Полупрозрачный черный триггер
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _canInteract = false;
                if (hintObject != null) hintObject.SetActive(false);

                // Возвращаем цвета обратно, только если ивент еще не завершен
                if (!_eventTriggered)
                {
                    if (currentStep == StepType.FirstStep_Rotate && lampRenderer != null)
                        lampRenderer.color = _originalLampColor;

                    if (currentStep == StepType.SecondStep_Puzzle && paintingRenderer != null)
                        paintingRenderer.color = _originalPaintingColor;

                    if (currentStep == StepType.FinalStep_RunAway && booksRenderer != null)
                        booksRenderer.color = new Color(_originalBooksColor.r, _originalBooksColor.g, _originalBooksColor.b, 0f);
                }
            }
        }
    }
}