using UnityEngine;
using UnityEngine.AI;

public class EnemyVoiceAI : MonoBehaviour
{
    [Header("Налаштування")]
    public Transform player;
    public float detectionThreshold = 0.05f;

    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
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
    }
}