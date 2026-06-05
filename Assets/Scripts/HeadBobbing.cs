using UnityEngine;

public class HeadBobbing : MonoBehaviour
{
    [Header("Налаштування Ходьби")]
    public float walkBobbingSpeed = 14f;       // Швидкість кроку
    public float walkBobbingAmountY = 0.05f;   // Амплітуда вгору-вниз
    public float walkBobbingAmountX = 0.04f;   // Амплітуда вліво-вправо

    [Header("Налаштування Бігу")]
    public float runBobbingSpeed = 18f;
    public float runBobbingAmountY = 0.1f;
    public float runBobbingAmountX = 0.08f;

    [Header("Плавність")]
    public float smoothTransition = 10f;

    private float defaultPosY = 0;
    private float defaultPosX = 0;
    private float timer = 0;

    void Start()
    {
        // Запам'ятовуємо стандартну позицію камери на старті
        defaultPosY = transform.localPosition.y;
        defaultPosX = transform.localPosition.x;
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        bool isMoving = Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f;
        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        if (isMoving)
        {
            float currentSpeed = isRunning ? runBobbingSpeed : walkBobbingSpeed;
            float currentAmountY = isRunning ? runBobbingAmountY : walkBobbingAmountY;
            float currentAmountX = isRunning ? runBobbingAmountX : walkBobbingAmountX;

            timer += Time.deltaTime * currentSpeed;

            // МАГІЯ РЕАЛІЗМУ:
            // Sin відповідає за вертикальні стрибки (один крок - один стрибок)
            float newY = defaultPosY + Mathf.Sin(timer) * currentAmountY;

            // Cos(timer / 2) відповідає за перенос ваги (один цикл на ДВА кроки)
            float newX = defaultPosX + Mathf.Cos(timer / 2f) * currentAmountX;

            transform.localPosition = new Vector3(newX, newY, transform.localPosition.z);
        }
        else
        {
            // Плавно повертаємо камеру рівно в центр по обох осях
            timer = 0;
            transform.localPosition = new Vector3(
                Mathf.Lerp(transform.localPosition.x, defaultPosX, Time.deltaTime * smoothTransition),
                Mathf.Lerp(transform.localPosition.y, defaultPosY, Time.deltaTime * smoothTransition),
                transform.localPosition.z
            );
        }
    }
}