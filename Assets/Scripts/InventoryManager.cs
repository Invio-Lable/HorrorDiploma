/*using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [Header("UI Елементи")]
    public Image[] slots;
    public Image[] selectionFrames;

    [Header("Дані")]
    public ItemData[] items;
    public int selectedSlotIndex = 0;

    [Header("Для викидання")]
    public Transform dropPoint;
    public float dropForce = 2f;

    private void Start()
    {
        items = new ItemData[slots.Length];
        UpdateUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SelectSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SelectSlot(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SelectSlot(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SelectSlot(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) SelectSlot(4);

        if (Input.GetKeyDown(KeyCode.G))
        {
            DropItem();
        }
    }

    public void SelectSlot(int index)
    {
        selectedSlotIndex = index;
        UpdateUI();
    }

    public bool AddItem(ItemData itemToAdd)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null)
            {
                items[i] = itemToAdd;

                if (i == 0) SelectSlot(0);
                else UpdateUI();

                return true;
            }
        }
        return false;
    }

    public void DropItem()
    {
        ItemData itemToDrop = items[selectedSlotIndex];

        if (itemToDrop != null && itemToDrop.prefab != null)
        {
            Instantiate(itemToDrop.prefab, dropPoint.position, dropPoint.rotation);

            items[selectedSlotIndex] = null;

            UpdateUI();

            Debug.Log("Викинуто: " + itemToDrop.itemName);
        }
    }

    private void UpdateUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (items[i] != null)
            {
                slots[i].gameObject.SetActive(true);
                slots[i].sprite = items[i].icon;

                // ВІЗУАЛ ВИБОРУ:
                if (i == selectedSlotIndex)
                {
                    slots[i].transform.localScale = new Vector3(1.2f, 1.2f, 1f);
                    slots[i].color = Color.white;
                }
                else
                {
                    slots[i].transform.localScale = Vector3.one;
                    slots[i].color = new Color(1, 1, 1, 0.5f);
                }
            }
            else
            {
                slots[i].gameObject.SetActive(false);
            }
        }
    }
}*/