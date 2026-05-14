using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public GameObject inventoryPanel;
    public GameObject slotPrefab;     // Тот самый префаб ячейки
    public Transform slotContainer;   // Ссылка на SlotContainer
    public int maxSlots = 8;          // Сколько всего ячеек будет

    private List<Sprite> items = new List<Sprite>(); // Список картинок предметов

    void Start()
    {
        inventoryPanel.SetActive(false); // Скрываем при старте
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I)) // Открыть/Закрыть на I
        {
            inventoryPanel.SetActive(!inventoryPanel.activeSelf);
            if (inventoryPanel.activeSelf) RefreshUI();
        }
    }

    public void AddItem(Sprite icon)
    {
        if (items.Count < maxSlots)
        {
            items.Add(icon);
            // Если инвентарь открыт, сразу обновляем
            if (inventoryPanel.activeSelf) RefreshUI();
        }
    }

    void RefreshUI()
    {
        // Удаляем старые ячейки перед отрисовкой новых
        foreach (Transform child in slotContainer)
        {
            Destroy(child.gameObject);
        }

        // Рисуем ячейки для каждого предмета в списке
        foreach (Sprite itemIcon in items)
        {
            GameObject newSlot = Instantiate(slotPrefab, slotContainer);
            // Ищем картинку внутри префаба (у нас она вторая)
            Image iconDisplay = newSlot.transform.GetChild(0).GetComponent<Image>();
            iconDisplay.sprite = itemIcon;
            iconDisplay.color = Color.white;
        }

        // Добавляем пустые ячейки до лимита (для красоты)
        for (int i = items.Count; i < maxSlots; i++)
        {
            GameObject emptySlot = Instantiate(slotPrefab, slotContainer);
            Image iconDisplay = emptySlot.transform.GetChild(0).GetComponent<Image>();
            iconDisplay.color = new Color(0, 0, 0, 0); // Прозрачная иконка
        }
    }

    // Добавь это внутрь класса InventoryManager
    public int ItemsCount => items.Count;
}