/*using UnityEngine;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Налаштування")]
    public float interactionDistance = 3f;
    public LayerMask interactableLayer;

    [Header("Зв'язки")]
    public Transform playerCamera;
    public InventoryManager inventoryManager;

    [Header("UI Елементи")]
    public GameObject worldSpacePrompt;
    public Image crosshair;

    public Vector3 promptOffset = new Vector3(0, 0.5f, 0);
    public Color defaultCrosshairColor = new Color(1, 1, 1, 0.5f);
    public Color activeCrosshairColor = new Color(1, 0, 0, 0.8f);

    void Start()
    {
        if (worldSpacePrompt != null) worldSpacePrompt.SetActive(false);
        if (crosshair != null) crosshair.color = defaultCrosshairColor;
    }

    void Update()
    {
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        RaycastHit hit;

        bool hitSomething = Physics.Raycast(ray, out hit, interactionDistance, interactableLayer);

        if (hitSomething)
        {
            InteractableItem targetItem = hit.collider.GetComponent<InteractableItem>();

            if (targetItem != null)
            {
                // 1. Змінюємо колір прицілу на активний
                if (crosshair != null) crosshair.color = activeCrosshairColor;

                // 2. Показуємо і рухаємо підказку [F]
                if (worldSpacePrompt != null)
                {
                    worldSpacePrompt.SetActive(true);

                    // Переміщуємо текст на позицію предмета + відступ вверх
                    worldSpacePrompt.transform.position = hit.transform.position + promptOffset;

                    // Змушуємо текст "дивитись" на камеру гравця (щоб не був боком)
                    worldSpacePrompt.transform.LookAt(worldSpacePrompt.transform.position + playerCamera.rotation * Vector3.forward,
                        playerCamera.rotation * Vector3.up);
                }

                // 3. Логіка підбору
                if (Input.GetKeyDown(KeyCode.F))
                {
                    bool added = inventoryManager.AddItem(targetItem.itemData);
                    if (added)
                    {
                        Destroy(hit.collider.gameObject);
                        // Одразу ховаємо UI, бо предмет зник
                        worldSpacePrompt.SetActive(false);
                        crosshair.color = defaultCrosshairColor;
                    }
                }
                return;
            }
        }

        // Ховаємо підказку
        if (worldSpacePrompt != null) worldSpacePrompt.SetActive(false);

        // Повертаємо звичайний колір прицілу
        if (crosshair != null) crosshair.color = defaultCrosshairColor;
    }
}*/