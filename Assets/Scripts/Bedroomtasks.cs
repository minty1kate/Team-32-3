using UnityEngine;

public class BedroomInit : MonoBehaviour
{
    void Start()
    {
        TaskManager tm = FindAnyObjectByType<TaskManager>();
        if (tm != null)
        {
            string[] bedroomTasks = new string[]
            {
                "Встать с кровати (Клавиша C)",
                "Собрать осколки картины (0/3)",
                "Собрать картину",
                "Найти брата"
            };
            tm.SetSceneTasks(bedroomTasks);
        }
    }
}