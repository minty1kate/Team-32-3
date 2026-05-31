using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class Puzzle : MonoBehaviour
    {
        private Manager manager;
            [HideInInspector] public int correctIndex; // Правильный индекс этой части картины
        
            public void Init(Manager mgr, int index, Sprite sprite)
            {
                manager = mgr;
                correctIndex = index;
                GetComponent<Image>().sprite = sprite;
                
                // Привязываем клик по кнопке
                GetComponent<Button>().onClick.AddListener(OnTileClick);
            }
        
            private void OnTileClick()
            {
                // Передаем менеджеру команду проверить, можно ли сдвинуть эту плитку
                manager.TryMoveTile(this.gameObject);
            }
        }
    }
