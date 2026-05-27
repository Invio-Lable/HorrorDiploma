using UnityEngine;

public class AutoOpenDoorPhysicsFeel : MonoBehaviour
{
    [Header("Налаштування")]
    [Tooltip("Максимальний кут відкриття (наприклад, 110 градусів)")]
    public float maxOpenAngle = 110f;
    public float openSpeed = 2.5f;

    private bool isOpening = false;
    private Quaternion closedRotation;
    private Quaternion targetRotation;

    void Start()
    {
        // Запам'ятовуємо абсолютну закриту позицію на початку гри
        closedRotation = transform.rotation;
    }

    void Update()
    {
        if (isOpening)
        {
            // Плавно крутимо до цілі
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * openSpeed);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Відчиняємо лише для гравця
        if (other.CompareTag("Player"))
        {
            // Визначаємо напрямок (з якого боку гравець)
            Vector3 fromDoorToPlayer = other.transform.position - transform.position;
            fromDoorToPlayer.y = 0;

            float dotProduct = Vector3.Dot(transform.forward, fromDoorToPlayer.normalized);

            // Визначаємо кут: якщо гравець спереду, кут від'ємний, якщо ззаду — додатний
            float finalAngle = (dotProduct < 0) ? maxOpenAngle : -maxOpenAngle;

            // ВАЖЛИВО: Множимо закриту ротацію на новий кут. 
            // Це гарантує, що ми додаємо 110 градусів до початкового стану, а не до поточного.
            targetRotation = closedRotation * Quaternion.Euler(0, finalAngle, 0);

            isOpening = true;
        }
    }
}