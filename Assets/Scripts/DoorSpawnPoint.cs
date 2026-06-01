using UnityEngine;

public class DoorSpawnPoint : MonoBehaviour
{
    public static Vector2 spawnPosition;

    void Awake()
    {
        // «апоминаем точку двери при старте уровн€
        spawnPosition = transform.position;
    }
}