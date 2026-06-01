using UnityEngine;
using UnityEngine.AI;

public class EnemyVoiceAI : MonoBehaviour
{
    [Header("Налаштування")]
    public Transform player;
    public float detectionThreshold = 0.05f;

    [Header("Анімації")]
    public Animator animator; // Посилання на Аніматор

    private NavMeshAgent agent;

    private float lookTimer = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (animator == null) animator = GetComponent<Animator>();

        // Задаємо випадковий час для першого озирання (від 3 до 7 секунд)
        lookTimer = Random.Range(3f, 7f);
    }

    void Update()
    {
        if (MicInput.MicLoudness > detectionThreshold)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);

        }
        else
        {
            if (!agent.isStopped)
            {
                agent.isStopped = true;
            }
        }

        float currentSpeed = agent.velocity.magnitude;
        animator.SetFloat("Speed", currentSpeed);

        // ЛОГІКА ОЗИРАННЯ:
        if (currentSpeed < 0.1f) // Якщо монстр стоїть на місці
        {
            // Віднімаємо час (Time.deltaTime - це час, що пройшов з минулого кадру)
            lookTimer -= Time.deltaTime;

            if (lookTimer <= 0f)
            {
                // Час вийшов! Натискаємо тригер в Аніматорі
                animator.SetTrigger("Look");

                // Задаємо новий випадковий час для наступного озирання (наприклад, від 4 до 8 секунд)
                lookTimer = Random.Range(4f, 20f);
            }
        }
        else // Якщо монстр йде
        {
            // Скидаємо таймер. Це потрібно, щоб він не почав озиратися в ту саму мілісекунду, коли зупиниться
            lookTimer = Random.Range(3f, 7f);
        }
    }
}