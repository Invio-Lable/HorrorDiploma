using UnityEngine;

public class AutoDoor : MonoBehaviour
{
    public enum DoorAxis { X, Y, Z }

    [Header("Налаштування відкриття")]
    [Tooltip("Обери вісь обертання з випадаючого списку")]
    public DoorAxis rotationAxis = DoorAxis.Y; // Меню вибору осі

    public float openAngle = 90f;   // Точний кут відкриття
    public float openSpeed = 5f;    // Швидкість відкриття

    private bool isOpen = false;
    private Quaternion closedRotation;
    private Quaternion targetRotation;

    void Start()
    {
        // Запам'ятовуємо закритий стан на початку гри
        closedRotation = transform.rotation;
        targetRotation = closedRotation;
    }

    void Update()
    {
        // Плавно обертаємо двері до нашої цілі. Коли вони докрутяться, то просто зафіксуються в цій позиції.
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * openSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Працює і для гравця, і для ворога
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            // Якщо двері ще НЕ відкриті — відкриваємо
            if (!isOpen)
            {
                OpenDoor(other.transform);
            }
        }
    }

    void OpenDoor(Transform opener)
    {
        isOpen = true; // Ставимо прапорець, що двері відкриті, і більше цей блок не спрацює

        // Визначаємо, з якого боку підійшли, щоб штовхнути двері ВІД себе
        Vector3 dir = (transform.position - opener.position).normalized;
        dir.y = 0;

        float side = Vector3.Dot(dir, transform.forward);
        float finalAngle = (side > 0) ? openAngle : -openAngle;

        // Перетворюємо твій вибір з інспектора на вектор осі
        Vector3 axisVector = Vector3.up;
        switch (rotationAxis)
        {
            case DoorAxis.X:
                axisVector = Vector3.right;
                break;
            case DoorAxis.Y:
                axisVector = Vector3.up;
                break;
            case DoorAxis.Z:
                axisVector = Vector3.forward;
                break;
        }

        // Задаємо фінальну точку обертання, де двері залишаться
        targetRotation = closedRotation * Quaternion.AngleAxis(finalAngle, axisVector);
    }
}