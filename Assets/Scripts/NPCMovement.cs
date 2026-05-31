using Unity.VisualScripting;
using UnityEngine;

namespace DefaultNamespace
{
    public class NPCMovement : MonoBehaviour
    {
        public float speed = 120f;
        public float fleeSpeed = 250f;
        public Transform[] patrolPoints;
        
        [Header("Настройки замешательства")]
        [Tooltip("Начальная скорость кручения (было 6)")]
        public float confusionSpeed = 5f; 
        
        private int currentPoint;
        private bool isFeeling;
        private Transform escapePoint;
        private SpriteRenderer spriteRenderer;
        public ParticleSystem _ps; // Перетащи сюда систему частиц в инспекторе
        private bool isConfused;

        void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (_ps == null) _ps = GetComponent<ParticleSystem>();
        }
        
        public void StopParticles()
        {
            if (_ps != null) 
            {
                _ps.Clear();
                _ps.Stop(); // Новые частицы перестанут появляться
            }
        }

        void Update()
        {
            if (isFeeling) {
                MoveTowards(escapePoint.position, fleeSpeed);
            } else if (isConfused) {
                spriteRenderer.flipX = (Mathf.PingPong(Time.time * confusionSpeed, 1) > 0.5f); // Метод для кручения
            } else {
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

            if (!isConfused)
            {
                if (target.x > transform.position.x)
                {
                    spriteRenderer.flipX = true;
                }
                else
                {
                    spriteRenderer.flipX = false;
                }
            }

            if (isFeeling && Vector2.Distance(transform.position, target) < 0.2f)
            {
                Destroy(gameObject); // NPC исчезает, дойдя до точки выхода
            }
        }
        
        public void StartConfused()
        {
            isConfused = true;
            isFeeling = false;
            confusionSpeed = 6f;
        
            StopParticles(); // Отключаем частицы сразу при начале кручения
        }
        
        public void MakeRotationFaster()
        {
            if (isConfused)
            {
                // Увеличиваем скорость пинг-понга (например, до 18, чтобы он вертелся в 3 раза быстрее)
                confusionSpeed = 10f; 
                Debug.Log("NPC паникует! Скорость вращения увеличена.");
            }
        }

        public void StartFeeling(Transform exit)
        {
            escapePoint = exit;
            isConfused = false;
            isFeeling = true;
        }
    }
}