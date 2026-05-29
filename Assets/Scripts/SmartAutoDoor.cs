using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshObstacle))]
public class SmartAutoDoor : MonoBehaviour
{
    [Header("Налаштування")]
    public float openAngle = 90f;

    [Tooltip("Швидкість у ГРАДУСАХ за секунду (постав тут 150 або 200)")]
    public float openSpeed = 200f;
    [Tooltip("Швидкість закриття у градусах за секунду")]
    public float closeSpeed = 150f;

    public float timeToClose = 1.5f;

    private NavMeshObstacle obstacle;

    private int entitiesInRange = 0;
    private Quaternion closedRotation;
    private Quaternion targetRotation;
    private bool isOpeningOrOpen = false;

    private void Awake()
    {
        obstacle = GetComponent<NavMeshObstacle>();
        obstacle.carveOnlyStationary = false;
        obstacle.carving = false;
        obstacle.enabled = false;

        closedRotation = transform.rotation;
        targetRotation = closedRotation;
    }

    private void Update()
    {
        // 1. БЕЗВІДМОВНИЙ РУХ: Завжди крутимо двері до нашої цілі (targetRotation)
        float currentSpeed = isOpeningOrOpen ? openSpeed : closeSpeed;

        // RotateTowards гарантує, що двері рівномірно дійдуть до кута і зупиняться
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, currentSpeed * Time.deltaTime);

        // 2. КЕРУВАННЯ NAVMESH: 
        // Якщо двері відхилились від закритої позиції хоча б на 1 градус — вмикаємо перешкоду
        if (Quaternion.Angle(transform.rotation, closedRotation) > 1f)
        {
            if (!obstacle.enabled)
            {
                obstacle.enabled = true;
                obstacle.carving = true;
            }
        }
        else // Якщо двері повністю закрились
        {
            if (obstacle.enabled)
            {
                obstacle.carving = false;
                obstacle.enabled = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            entitiesInRange++;

            // Якщо зайшла ПЕРША сутність
            if (entitiesInRange == 1)
            {
                CancelInvoke(nameof(CloseDoor)); // Відміняємо закриття, якщо воно планувалось

                // Розраховуємо куди відчиняти
                Vector3 dir = (transform.position - other.transform.position).normalized;
                dir.y = 0;
                float dot = Vector3.Dot(transform.forward, dir);
                float finalAngle = (dot < 0) ? openAngle : -openAngle;

                // Задаємо ціль (Двері самі поїдуть до неї в Update)
                targetRotation = closedRotation * Quaternion.Euler(0, finalAngle, 0);
                isOpeningOrOpen = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            entitiesInRange--;
            if (entitiesInRange < 0) entitiesInRange = 0; // Захист від багів Unity

            // Коли всі вийшли з зони — запускаємо таймер
            if (entitiesInRange == 0)
            {
                Invoke(nameof(CloseDoor), timeToClose);
            }
        }
    }

    private void CloseDoor()
    {
        // Просто змінюємо ціль на "закрито", Update зробить все інше
        targetRotation = closedRotation;
        isOpeningOrOpen = false;
    }
}