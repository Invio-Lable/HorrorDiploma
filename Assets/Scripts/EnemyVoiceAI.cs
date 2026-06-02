using UnityEngine;
using UnityEngine.AI;

public class EnemyVoiceAI : MonoBehaviour
{
    // Створюємо стани для нашого монстра
    public enum AIState { Patrol, Investigate }

    [Header("Головні налаштування")]
    public Transform player;
    public float dangerThreshold = 0.05f;

    [Header("Патрулювання")]
    public float patrolRadius = 20f; // Як далеко може зайти шукаючи випадкову точку
    public float waitTimeAtPoint = 4f; // Скільки стоїть на місці перед наступним кроком

    [Header("Розслідування")]
    public float investigateWaitTime = 6f; // Скільки часу шукає гравця на місці шуму

    [Header("Анімації")]
    public Animator animator;

    private NavMeshAgent agent;
    private AIState currentState = AIState.Patrol;

    private Vector3 lastHeardPosition;
    private float waitTimer = 0f;
    private bool isWaiting = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (animator == null) animator = GetComponent<Animator>();

        // Одразу відправляємо його в першу випадкову точку
        SetRandomPatrolPoint();
    }

    void Update()
    {
        // 1. АНІМАЦІЯ ХОДЬБИ (Завжди оновлюємо швидкість)
        animator.SetFloat("Speed", agent.velocity.magnitude);

        // 2. СЛУХ (Пріоритет №1)
        float currentLoudness = MicInput.MicLoudness;
        if (currentLoudness >= dangerThreshold)
        {
            // Якщо почув нас - перемикається в режим розслідування
            currentState = AIState.Investigate;
            lastHeardPosition = player.position; // Запам'ятовує точку, де ми шуміли!

            agent.SetDestination(lastHeardPosition);
            isWaiting = false; // Скидаємо очікування, якщо він стояв
        }

        // 3. ЛОГІКА СТАНІВ
        switch (currentState)
        {
            case AIState.Patrol:
                PatrolLogic();
                break;
            case AIState.Investigate:
                InvestigateLogic();
                break;
        }
    }

    private void PatrolLogic()
    {
        // Якщо дійшов до випадкової точки (залишилось менше півметра)
        if (!agent.pathPending && agent.remainingDistance <= 0.5f)
        {
            if (!isWaiting)
            {
                isWaiting = true;
                waitTimer = waitTimeAtPoint;
                animator.SetTrigger("Look"); // Озирається, коли прийшов на точку
            }
            else
            {
                waitTimer -= Time.deltaTime;
                if (waitTimer <= 0f)
                {
                    SetRandomPatrolPoint(); // Час вийшов, йдемо в нове місце
                }
            }
        }
    }

    private void InvestigateLogic()
    {
        // Якщо дійшов до місця, де останній раз чув гравця
        if (!agent.pathPending && agent.remainingDistance <= 0.5f)
        {
            if (!isWaiting)
            {
                isWaiting = true;
                waitTimer = investigateWaitTime;
                animator.SetTrigger("Look"); // Уважно озирається на місці шуму
            }
            else
            {
                waitTimer -= Time.deltaTime;
                if (waitTimer <= 0f)
                {
                    // Нікого не знайшов і більше не чув шуму — повертаємось до патруля
                    currentState = AIState.Patrol;
                    SetRandomPatrolPoint();
                }
            }
        }
    }

    // Метод для пошуку випадкової точки на NavMesh
    private void SetRandomPatrolPoint()
    {
        // Шукаємо випадкові координати в радіусі patrolRadius
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += transform.position;

        NavMeshHit hit;
        // Перевіряємо, чи є в цьому радіусі підлога (NavMesh)
        if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, 1))
        {
            agent.SetDestination(hit.position);
            isWaiting = false;
        }
    }
}