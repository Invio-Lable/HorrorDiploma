using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Налаштування швидкості")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float slowWalkSpeed = 2.5f;
    public float crouchSpeed = 2.5f;

    [Header("Налаштування Стаміни")]
    public float maxStamina = 100f;
    public float staminaDrain = 15f;
    public float staminaRegen = 5f;

    [Tooltip("Час у секундах, протягом якого не можна бігти після повного виснаження")]
    public float exhaustionCooldown = 5f;

    [SerializeField] private float currentStamina;
    public Image staminaBarUI;

    private bool isExhausted = false;
    private float exhaustionTimer = 0f;

    [Header("Налаштування миші")]
    public float mouseSensitivity = 2f;
    public Transform playerCamera;
    private float xRotation = 0f;

    [Header("Фізика")]
    public float gravity = -9.81f;
    public float crouchHeight = 1f;
    public float standHeight = 2f;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        currentStamina = maxStamina;
    }

    void Update()
    {
        // 1. Гравітація
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // 2. Погляд
        HandleMouseLook();

        // 3. Рух і Стаміна (З логікою тайм-ауту)
        HandleMovementAndStamina();

        // 4. Падіння
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleMovementAndStamina()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        bool isMoving = x != 0 || z != 0;

        float currentSpeed = walkSpeed;
        float currentHeight = standHeight;

        // --- ЛОГІКА СТАМІНИ ТА ВИСНАЖЕННЯ ---

        // Якщо ми зараз виснажені, відраховуємо час таймера
        if (isExhausted)
        {
            exhaustionTimer -= Time.deltaTime;
            if (exhaustionTimer <= 0)
            {
                isExhausted = false; // Тайм-аут пройшов, можна знову бігти
            }
        }

        bool isTryingToRun = Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.LeftControl);

        // Умова бігу: Хочемо бігти + Рухаємось + Немає виснаження + Є стаміна
        if (isTryingToRun && isMoving && !isExhausted && currentStamina > 0)
        {
            currentSpeed = runSpeed;
            currentStamina -= staminaDrain * Time.deltaTime;

            // Якщо стаміна впала до 0 під час бігу — вмикаємо покарання
            if (currentStamina <= 0)
            {
                currentStamina = 0;
                isExhausted = true;
                exhaustionTimer = exhaustionCooldown; // Запускаємо таймер на 5 секунд
                Debug.Log("Гравець видихся! Штраф 5 секунд.");
            }
        }
        else
        {
            // Відновлення сил
            if (currentStamina < maxStamina)
            {
                currentStamina += staminaRegen * Time.deltaTime;
            }
        }

        // --- ВІЗУАЛІЗАЦІЯ ---

        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);

        if (staminaBarUI != null)
        {
            staminaBarUI.fillAmount = currentStamina / maxStamina;

            if (isExhausted)
            {
                staminaBarUI.color = Color.red;
            }
            else
            {
                staminaBarUI.color = Color.green;
            }
        }


        if (Input.GetKey(KeyCode.LeftAlt))
        {
            currentSpeed = slowWalkSpeed;
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            currentSpeed = crouchSpeed;
            currentHeight = crouchHeight;
        }

        controller.height = Mathf.Lerp(controller.height, currentHeight, 10 * Time.deltaTime);

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * currentSpeed * Time.deltaTime);
    }
}