using Unity.VisualScripting;
using UnityEngine;

namespace DefaultNamespace
{
    public class NPCMovement : MonoBehaviour
    {
        public float speed = 120f;
        public float fleeSpeed = 250f;
        public Transform[] patrolPoints;
        
        private int currentPoint = 0;
        private bool isFeeling = false;
        private Transform escapePoint;
        private SpriteRenderer spriteRenderer;

        void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        void Update()
        {
            if (isFeeling)
            {
                MoveTowards(escapePoint.position, fleeSpeed);
            }
            else
            {
                Patrol();
            }
        }

        void Patrol()
        {
            if (patrolPoints.Length == 0) return;
            
            MoveTowards(patrolPoints[currentPoint].position, speed);

            if (Vector2.Distance(transform.position, patrolPoints[currentPoint].position) <= 0.2f)
            {
                currentPoint = (currentPoint + 1) % patrolPoints.Length;
            }
        }

        void MoveTowards(Vector2 target, float currentSpeed)
        {
            transform.position = Vector2.MoveTowards(transform.position, target, currentSpeed * Time.deltaTime);

            if (target.x > transform.position.x)
            {
                spriteRenderer.flipX = true;
            }
            else
            {
                spriteRenderer.flipX = false;
            }
        }

        public void StartFeeling(Transform exit)
        {
            escapePoint = exit;
            isFeeling = true;
        }
    }
}