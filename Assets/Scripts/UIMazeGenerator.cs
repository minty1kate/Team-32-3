using UnityEngine;
using UnityEngine.UI;

public class UIMazeGenerator : MonoBehaviour
{
    [Header("Настройки сетки")]
    public int rows = 10;    // Количество строк лабиринта
    public int columns = 10; // Количество колонок лабиринта

    [Header("Ссылки на UI")]
    public RectTransform mazePanelRect; // Твоя панель mazePanel
    public GameObject wallPrefab;       // UI Префаб стены с BoxCollider2D и слоем MazeWall
    public MazeBall playerBall;         // Ссылка на скрипт твоего квадратика

    // Карта лабиринта: 1 — стена, 0 — пустой проход, 2 — Старт (игрок), 3 — Финиш (триггер)
    // Ты можешь нарисовать здесь любой узор! Главное, чтобы размер совпадал с rows и columns
    private int[,] _mazeGrid = {
        { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
        { 1, 2, 0, 0, 1, 0, 0, 0, 0, 1 },
        { 1, 1, 1, 0, 1, 0, 1, 1, 0, 1 },
        { 1, 0, 0, 0, 0, 0, 1, 0, 0, 1 },
        { 1, 0, 1, 1, 1, 1, 1, 0, 1, 1 },
        { 1, 0, 1, 0, 0, 0, 1, 0, 0, 1 },
        { 1, 0, 1, 0, 1, 0, 1, 1, 0, 1 },
        { 1, 0, 0, 0, 1, 0, 0, 1, 0, 1 },
        { 1, 1, 1, 0, 1, 1, 0, 0, 3, 1 },
        { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }
    };

    void Start()
    {
        if (mazePanelRect == null) mazePanelRect = GetComponent<RectTransform>();
        GenerateMaze();
    }

    void GenerateMaze()
    {
        // Вычисляем размеры одного блока на основе размеров панели UI
        float panelWidth = mazePanelRect.rect.width;
        float panelHeight = mazePanelRect.rect.height;

        float cellWidth = panelWidth / columns;
        float cellHeight = panelHeight / rows;

        // Начальная точка отрисовки (левый верхний угол панели)
        float startX = -panelWidth / 2 + cellWidth / 2;
        float startY = panelHeight / 2 - cellHeight / 2;

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                int cellType = _mazeGrid[r, c];

                // Вычисляем UI позицию для текущего блока
                Vector2 spawnPos = new Vector2(startX + (c * cellWidth), startY - (r * cellHeight));

                if (cellType == 1) // СТЕНА
                {
                    SpawnWall(spawnPos, cellWidth, cellHeight);
                }
                else if (cellType == 2) // ТОЧКА СТАРТА ИГРОКА
                {
                    if (playerBall != null)
                    {
                        RectTransform pRect = playerBall.GetComponent<RectTransform>();
                        pRect.sizeDelta = new Vector2(cellWidth * 0.7f, cellHeight * 0.7f); // Чуть меньше клетки, чтобы легче проходить
                        pRect.anchoredPosition = spawnPos;
                        // Перезаписываем стартовую позицию в скрипте мяча, чтобы при проигрыше он возвращался сюда
                        playerBall.ResetToStart();
                    }
                }
                else if (cellType == 3) // ФИНИШ
                {
                    // Находим твой существующий объект финиша на сцене и ставим его в эту клетку
                    GameObject finishObj = GameObject.FindWithTag("MazeFinish");
                    if (finishObj != null)
                    {
                        RectTransform fRect = finishObj.GetComponent<RectTransform>();
                        fRect.sizeDelta = new Vector2(cellWidth, cellHeight);
                        fRect.anchoredPosition = spawnPos;
                    }
                }
            }
        }
    }

    void SpawnWall(Vector2 position, float width, float height)
    {
        // Создаем префаб стены внутри панели лабиринта
        GameObject newWall = Instantiate(wallPrefab, mazePanelRect);

        // Настраиваем UI размеры и позицию
        RectTransform rect = newWall.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(width, height);
        rect.anchoredPosition = position;

        // Подгоняем под эти размеры BoxCollider2D, чтобы детекция работала четко пиксель в пиксель
        BoxCollider2D collider = newWall.GetComponent<BoxCollider2D>();
        if (collider != null)
        {
            collider.size = new Vector2(width, height);
        }
    }
}