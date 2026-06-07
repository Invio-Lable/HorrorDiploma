using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Video;

public class EnemyVoiceAI : MonoBehaviour
{
    // Додали стан Chase (Переслідування)
    public enum AIState { Patrol, Investigate, Chase, Jumpscare }

    [Header("Головні налаштування")]
    public Transform player;
    public float dangerThreshold = 0.05f;

    [Header("Зір (Найвищий пріоритет)")]
    public float visionDistance = 8f;        // Дальність зору (збільшив для тесту)
    [Range(0, 180)]
    public float visionAngle = 90f;          // Кут зору
    public float eyeHeight = 1.5f;           // Висота очей (щоб лазер не бив у підлогу)

    [Header("Патрулювання та Пошук")]
    public float patrolRadius = 20f;
    public float waitTimeAtPoint = 4f;
    public float investigateWaitTime = 6f;

    [Header("Скрімер (Відео) та Смерть")]
    public float attackDistance = 1.8f;
    public VideoPlayer jumpscareVideo;
    public GameObject gameOverCanvas;
    public MonoBehaviour playerMoveScript;
    public MonoBehaviour playerLookScript;
    public GameObject playerFlashlight;

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

        SetRandomPatrolPoint();
    }

    void Update()
    {
        if (currentState == AIState.Jumpscare) return;

        // 1. ПЕРЕВІРКА НА СМЕРТЬ (Скрімер)
        if (Vector3.Distance(transform.position, player.position) <= attackDistance)
        {
            StartCoroutine(DoJumpscareSequence());
            return;
        }

        // Оновлюємо анімацію ходьби
        animator.SetFloat("Speed", agent.velocity.magnitude);

        // --- ЛОГІКА ПРІОРИТЕТІВ ---

        bool canSee = CanSeePlayer();
        bool canHear = MicInput.MicLoudness >= dangerThreshold;

        // ПРІОРИТЕТ 1: ЗІР
        if (canSee)
        {
            currentState = AIState.Chase;
            lastHeardPosition = player.position; // Запам'ятовуємо, де бачимо гравця, на випадок якщо він сховається
        }
        else
        {
            // Якщо ми тільки що його переслідували, але він забіг за стіну (canSee стало false)
            if (currentState == AIState.Chase)
            {
                currentState = AIState.Investigate;
                agent.SetDestination(lastHeardPosition); // Біжимо туди, де бачили востаннє
                isWaiting = false;
            }
            // ПРІОРИТЕТ 2: СЛУХ (Тільки якщо не бачимо гравця)
            else if (canHear)
            {
                currentState = AIState.Investigate;
                lastHeardPosition = player.position;
                agent.SetDestination(lastHeardPosition);
                isWaiting = false;
            }
        }

        // --- ВИКОНАННЯ СТАНІВ ---
        switch (currentState)
        {
            case AIState.Patrol:
                PatrolLogic();
                break;
            case AIState.Investigate:
                InvestigateLogic();
                break;
            case AIState.Chase:
                ChaseLogic();
                break;
        }
    }

    // ЛОГІКА ЗОРУ (З покращеним лазером)
    private bool CanSeePlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= visionDistance)
        {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

            if (angleToPlayer <= visionAngle / 2f)
            {
                // Піднімаємо лазер на рівень очей монстра і цілимось у груди гравцю (щоб не бити йому в п'яти)
                Vector3 rayStart = transform.position + Vector3.up * eyeHeight;
                Vector3 rayEnd = player.GetComponent<Collider>().bounds.center;
                Vector3 rayDirection = rayEnd - rayStart;

                // МАЛЮЄМО ЛАЗЕР У ВІКНІ SCENE (Для дебагу)
                Debug.DrawRay(rayStart, rayDirection.normalized * visionDistance, Color.red);

                if (Physics.Raycast(rayStart, rayDirection, out RaycastHit hit, visionDistance))
                {
                    if (hit.transform.CompareTag("Player") || hit.transform.root.CompareTag("Player"))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    // --- ЛОГІКА СТАНІВ ---

    private void ChaseLogic()
    {
        // Постійно оновлюємо ціль на Гравця. Монстр буде йти за тобою крок у крок!
        agent.SetDestination(player.position);
        isWaiting = false;
    }

    private void PatrolLogic()
    {
        if (!agent.pathPending && agent.remainingDistance <= 0.5f)
        {
            if (!isWaiting)
            {
                isWaiting = true;
                waitTimer = waitTimeAtPoint;
                animator.SetTrigger("Look");
            }
            else
            {
                waitTimer -= Time.deltaTime;
                if (waitTimer <= 0f) SetRandomPatrolPoint();
            }
        }
    }

    private void InvestigateLogic()
    {
        if (!agent.pathPending && agent.remainingDistance <= 0.5f)
        {
            if (!isWaiting)
            {
                isWaiting = true;
                waitTimer = investigateWaitTime;
                animator.SetTrigger("Look");
            }
            else
            {
                waitTimer -= Time.deltaTime;
                if (waitTimer <= 0f)
                {
                    currentState = AIState.Patrol;
                    SetRandomPatrolPoint();
                }
            }
        }
    }

    private void SetRandomPatrolPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, 1))
        {
            agent.SetDestination(hit.position);
            isWaiting = false;
        }
    }

    private IEnumerator DoJumpscareSequence()
    {
        currentState = AIState.Jumpscare;

        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        if (playerMoveScript != null) playerMoveScript.enabled = false;
        if (playerLookScript != null) playerLookScript.enabled = false;

        if (playerFlashlight != null)
        {
            Renderer[] renderers = playerFlashlight.GetComponentsInChildren<Renderer>();
            foreach (Renderer r in renderers) r.enabled = false;

            Light[] lights = playerFlashlight.GetComponentsInChildren<Light>();
            foreach (Light l in lights) l.enabled = false;
        }

        if (jumpscareVideo != null)
        {
            jumpscareVideo.Play();
            yield return new WaitForSeconds((float)jumpscareVideo.length);
        }
        else
        {
            yield return new WaitForSeconds(2f);
        }

        if (gameOverCanvas != null) gameOverCanvas.SetActive(true);
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}