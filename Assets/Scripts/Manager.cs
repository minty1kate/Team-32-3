using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public class Manager : MonoBehaviour
    {
        [SerializeField] private Transform gridContainer; // Ссылка на GridContainer
        [SerializeField] private Sprite[] totalSprites; // Сюда перетащите все 9 нарезанных спрайтов по порядку
        [SerializeField] private GameObject tilePrefab; // Префаб плитки

        private List<GameObject> tiles = new();
        private GameObject emptyTile;
        private int gridSize = 3; // Для сетки 3х3

        void OnEnable()
        {
            foreach (Transform child in gridContainer)
            {
                Destroy(child.gameObject);
            }
            tiles.Clear();

            // Запускаем создание заново при каждом включении окна
            CreatePuzzle();
            ShuffleTiles();
        }
        
        // --- ЧИТ-КОД ---
        void Update()
        {
            // Если нажата клавиша T, автоматически собираем пазл
            if (Input.GetKeyDown(KeyCode.T))
            {
                SolvePuzzleCheat();
            }
        }

        void SolvePuzzleCheat()
        {
            Debug.Log("Активирован чит-код: автоматическая сборка пазла.");
            
            // Расставляем все плитки по их правильным индексам в UI
            for (int i = 0; i < tiles.Count; i++)
            {
                Puzzle tileScript = tiles[i].GetComponent<Puzzle>();
                if (tileScript != null)
                {
                    tiles[i].transform.SetSiblingIndex(tileScript.correctIndex);
                }
            }

            // Запускаем стандартную логику победы
            OnWin();
        }

        void CreatePuzzle()
        {
            for (int i = 0; i < totalSprites.Length; i++)
            {
                GameObject newTile = Instantiate(tilePrefab, gridContainer);
                Puzzle tileScript = newTile.GetComponent<Puzzle>();
                tileScript.Init(this, i, totalSprites[i]);
                tiles.Add(newTile);

                // Последний элемент делаем невидимым (пустое место)
                if (i == totalSprites.Length - 1)
                {
                    emptyTile = newTile;
                    // Скрываем картинку пустого слота
                    newTile.GetComponent<UnityEngine.UI.Image>().color = new Color(0, 0, 0, 0);
                }
            }
        }

        public void TryMoveTile(GameObject clickedTile)
        {
            int clickedIndex = clickedTile.transform.GetSiblingIndex();
            int emptyIndex = emptyTile.transform.GetSiblingIndex();

            // Рассчитываем координаты в сетке X и Y
            int clickX = clickedIndex % gridSize;
            int clickY = clickedIndex / gridSize;
            int emptyX = emptyIndex % gridSize;
            int emptyY = emptyIndex / gridSize;

            // Проверяем, находится ли пустая плитка по соседству (на расстоянии 1 шага по вертикали или горизонтали)
            if ((Mathf.Abs(clickX - emptyX) == 1 && clickY == emptyY) ||
                (Mathf.Abs(clickY - emptyY) == 1 && clickX == emptyX))
            {
                // Меняем их местами в иерархии UI (Grid Layout Group автоматически перестроит их на экране)
                clickedTile.transform.SetSiblingIndex(emptyIndex);
                emptyTile.transform.SetSiblingIndex(clickedIndex);

                CheckWinCondition();
            }
        }

        void ShuffleTiles()
        {
            // Вместо случайной перетасовки делаем 50–100 реальных случайных сдвигов пустой плитки
            List<int> layout = new List<int>();
            for (int i = 0; i < tiles.Count; i++)
            {
                layout.Add(i);
            }

            int emptyLayoutIndex = tiles.Count - 1; // Пустая плитка изначально в конце

            // Делаем 100 легальных ходов в памяти
            for (int i = 0; i < 100; i++)
            {
                List<int> validNeighborIndices = new List<int>();
                int emptyX = emptyLayoutIndex % gridSize;
                int emptyY = emptyLayoutIndex / gridSize;

                // Проверяем 4 направления вокруг пустой плитки
                int[] dx = { -1, 1, 0, 0 };
                int[] dy = { 0, 0, -1, 1 };

                for (int d = 0; d < 4; d++)
                {
                    int nX = emptyX + dx[d];
                    int nY = emptyY + dy[d];

                    // Если координаты соседа не выходят за рамки сетки 3х3
                    if (nX >= 0 && nX < gridSize && nY >= 0 && nY < gridSize)
                    {
                        int neighborLayoutIndex = nY * gridSize + nX;
                        validNeighborIndices.Add(neighborLayoutIndex);
                    }
                }

                // Выбираем случайного соседа и меняем местами в списке
                if (validNeighborIndices.Count > 0)
                {
                    int randomNeighborLayoutIndex = validNeighborIndices[Random.Range(0, validNeighborIndices.Count)];

                    // Свапаем в списке layout
                    (layout[emptyLayoutIndex], layout[randomNeighborLayoutIndex]) = (layout[randomNeighborLayoutIndex], layout[emptyLayoutIndex]);

                    // Обновляем позицию пустой плитки для следующего шага
                    emptyLayoutIndex = randomNeighborLayoutIndex;
                }
            }

            // Применяем перемешанную сетку к UI элементам ОДИН раз в конце
            for (int i = 0; i < layout.Count; i++)
            {
                int tileIndexInList = layout[i];
                tiles[tileIndexInList].transform.SetSiblingIndex(i);
            }
        }

        void CheckWinCondition()
        {
            for (int i = 0; i < tiles.Count; i++)
            {
                // Если индекс в иерархии UI совпадает с правильным индексом картинки
                if (tiles[i].transform.GetSiblingIndex() != tiles[i].GetComponent<Puzzle>().correctIndex)
                {
                    return; // Пазл еще не собран
                }
            }

            OnWin();
        }

        void OnWin()
        {
            Debug.Log("Пятнашки собраны!");
            emptyTile.GetComponent<UnityEngine.UI.Image>().color = new Color(1, 1, 1, 1);

            // 1. Находим скрипт квестов на сцене
            DefaultNamespace.Living_Events[] allEvents = FindObjectsOfType<DefaultNamespace.Living_Events>();
            foreach (var ev in allEvents)
            {
                if (ev.currentStep == DefaultNamespace.Living_Events.StepType.SecondStep_Puzzle)
                {
                    ev.CompletePuzzleStep(); // Передаем картине команду включить свечение и ускорить NPC
                    break;
                }
            }

            Invoke("CloseWindow", 1.5f); // Закрываем интерфейс через 1.5 секунды
        }

        void CloseWindow()
        {
            this.gameObject.SetActive(false);
        }
    }
}