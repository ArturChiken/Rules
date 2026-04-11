using DreamMovement;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryTester : MonoBehaviour
{
    private Inventory inventory;
    private PlayerControl playerControl;

    private void Start()
    {
        inventory = Inventory.Instance;
        playerControl = FindFirstObjectByType<PlayerControl>();

        if (inventory != null)
        {
            inventory.OnActiveSlotChanged += OnActiveSlotChanged;
            inventory.OnSlotChanged += OnSlotChanged;
        }
    }

    private void OnActiveSlotChanged(int slotIndex)
    {
        Debug.Log($"--- Активный слот изменен: {slotIndex + 1} ---");
    }

    private void OnSlotChanged(int slotIndex, InventorySlot slot)
    {
        if (slot.IsEmpty)
            Debug.Log($"Слот {slotIndex + 1}: пусто");
        else
            Debug.Log($"Слот {slotIndex + 1}: {slot.Item.itemName} x{slot.Amount}");
    }

    private void Update()
    {
        // Используем PlayerControl для проверки нажатий
        if (playerControl != null)
        {
            // Например, используем действие, которого нет в основном наборе
            // Или добавляем временную проверку через GetKey из новой системы
            if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
            {
                inventory?.UseActiveItem();
            }

            if (Keyboard.current != null && Keyboard.current.iKey.wasPressedThisFrame)
            {
                inventory?.PrintInventory();
            }
        }
    }

    private void OnDestroy()
    {
        if (inventory != null)
        {
            inventory.OnActiveSlotChanged -= OnActiveSlotChanged;
            inventory.OnSlotChanged -= OnSlotChanged;
        }
    }
}