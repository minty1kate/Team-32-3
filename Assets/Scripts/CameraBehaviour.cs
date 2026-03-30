using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    void Start()
    {
        player = GameObject.Find("character_0");
    }

    void Update()
    {
        var pos = player.transform.position;
        transform.position = new Vector3(pos.x, pos.y, transform.position.z);
    }

    private GameObject player;
}