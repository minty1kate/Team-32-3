using UnityEngine;

public class Movement : MonoBehaviour
{
    private float acceleration = 30;
    private Rigidbody2D rigidBodyComponent;
    private GameObject exit;
    
    void Start()
    {
        rigidBodyComponent = GetComponent<Rigidbody2D>();
        exit = GameObject.Find("Exit");
    }
    
    void Update()
    {
        var w = Input.GetKey(KeyCode.W) ? 1 : 0;
        var a  = Input.GetKey(KeyCode.A) ? 1 : 0;
        var s = Input.GetKey(KeyCode.S) ? -1 : 0;
        var d = Input.GetKey(KeyCode.D) ? -1 : 0;
        var movement = new Vector2(-(a + d), w + s);
        
        rigidBodyComponent.linearVelocity = movement * acceleration;
        if (movement.magnitude > Mathf.Epsilon)
        {
            var angle = new Vector3(0 ,0, movement.magnitude);
            transform.rotation = Quaternion.Euler(angle);
        }
    }
}