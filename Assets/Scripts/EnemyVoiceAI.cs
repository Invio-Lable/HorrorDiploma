using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Video; // Додали бібліотеку для відео

public class EnemyVoiceAI : MonoBehaviour
{
    public enum AIState { Patrol, Investigate, Jumpscare }

    [Header("Головні налаштування")]
    public Transform player;
    public float dangerThreshold = 0.05f;

    [Header("Патрулювання та Пошук")]
    public float patrolRadius = 20f;
    public float waitTimeAtPoint = 4f;
    public float investigateWaitTime = 6f;

    [Header("Скрімер (Відео) та Смерть")]
    public float attackDistance = 1.8f;
    public VideoPlayer jumpscareVideo;
    public GameObject gameOverCanvas;

    [Tooltip("Скрипт, який відповідає за ходьбу (WASD)")]
    public MonoBehaviour playerMoveScript;

    [Tooltip("Скрипт, який відповідає за поворот камери мишкою")]
    public MonoBehaviour playerLookScript;

    [Tooltip("Твій ліхтарик (весь об'єкт з моделлю і світлом)")]
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

        // 1. ПЕРЕВІРКА НА СМЕРТЬ
        if (Vector3.Distance(transform.position, player.position) <= attackDistance)
        {
            StartCoroutine(DoJumpscareSequence());
            return;
        }

        animator.SetFloat("Speed", agent.velocity.magnitude);

        // 2. СЛУХ
        float currentLoudness = MicInput.MicLoudness;
        if (currentLoudness >= dangerThreshold)
        {
            currentState = AIState.Investigate;
            lastHeardPosition = player.position;

            agent.SetDestination(lastHeardPosition);
            isWaiting = false;
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

    private IEnumerator DoJumpscareSequence()
    {
        currentState = AIState.Jumpscare;

        // 1. Зупиняємо монстра та заморожуємо гравця
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        if (playerMoveScript != null) playerMoveScript.enabled = false;
        if (playerLookScript != null) playerLookScript.enabled = false;

        // 2. ІСТИННЕ ПРИХОВУВАННЯ ЛІХТАРИКА (Куленепробивний метод)
        if (playerFlashlight != null)
        {
            // Знаходимо всі компоненти, які малюють модель (MeshRenderer, SkinnedMeshRenderer)
            Renderer[] renderers = playerFlashlight.GetComponentsInChildren<Renderer>();
            foreach (Renderer r in renderers)
            {
                r.enabled = false; // Вимикаємо видимість моделі
            }

            // Знаходимо всі джерела світла (Light)
            Light[] lights = playerFlashlight.GetComponentsInChildren<Light>();
            foreach (Light l in lights)
            {
                l.enabled = false; // Вимикаємо саме світло
            }
        }

        // 3. Запускаємо відео
        if (jumpscareVideo != null)
        {
            jumpscareVideo.Play();
            yield return new WaitForSeconds((float)jumpscareVideo.length);
        }
        else
        {
            yield return new WaitForSeconds(2f);
        }

        // 4. Показуємо Game Over та зупиняємо час
        if (gameOverCanvas != null) gameOverCanvas.SetActive(true);
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
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
}