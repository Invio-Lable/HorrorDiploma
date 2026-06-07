using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Налаштування")]
    public float interactionDistance = 3f;
    private Camera playerCamera;

    [Header("Інтерфейс")]
    public GameObject interactPrompt; // Напис "[E] Читати" на головному Canvas

    void Start()
    {
        playerCamera = Camera.main;
        if (interactPrompt != null) interactPrompt.SetActive(false);
    }

    void Update()
    {
        // Якщо ми зараз читаємо записку на весь екран - ховаємо підказку і не кидаємо лазер
        if (GameManager.Instance.isReading)
        {
            if (interactPrompt != null) interactPrompt.SetActive(false);
            return;
        }

        // Кидаємо лазер з центру екрана
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance))
        {
            // Чи є на об'єкті скрипт LetterItem?
            LetterItem letter = hit.transform.GetComponent<LetterItem>();

            if (letter != null)
            {
                // Ми дивимось на записку! Показуємо підказку
                if (interactPrompt != null) interactPrompt.SetActive(true);

                // Якщо натиснули E - підбираємо
                if (Input.GetKeyDown(KeyCode.E))
                {
                    letter.PickUp();
                    if (interactPrompt != null) interactPrompt.SetActive(false); // Ховаємо підказку після підбору
                }
                return; // Виходимо, щоб не сховати підказку нижче
            }
        }

        // Якщо ми відвернулися або дивимось у стіну - ховаємо підказку
        if (interactPrompt != null) interactPrompt.SetActive(false);
    }
}