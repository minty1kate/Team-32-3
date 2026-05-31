using UnityEngine;

public class MirrorFrame : MonoBehaviour
{
    public int piecesInInventory = 0; // Сюда твой QuestManager должен передавать количество
    public SpriteRenderer highlight; // Объект подсветки
    private bool _playerInside = false;

    void Update()
    {
        // Если игрок рядом, у него есть 3 осколка и он нажал Е
        if (_playerInside && piecesInInventory >= 3 && Input.GetKeyDown(KeyCode.E))
        {
            PuzzleManager.Instance.OpenPuzzle();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && piecesInInventory >= 3)
        {
            _playerInside = true;
            highlight.color = Color.white; // Включаем подсветку
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _playerInside = false;
            highlight.color = Color.clear; // Выключаем
        }
    }
}