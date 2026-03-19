using System.Collections.Generic;
using DreamMovement;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Header("Inventory Settings")]
    [SerializeField] private int slotCount = 4;
    [SerializeField] private int activeSlotIndex = 0;

    [Header("Debug")]
    [SerializeField] private bool logToConsole = true;

    // События для оповещения других классов (для будущего GUI)
    public System.Action<int, InventorySlot> OnSlotChanged;
    public System.Action<int> OnActiveSlotChanged;

    // Singleton
    public static Inventory Instance { get; private set; }

    private List<InventorySlot> slots = new List<InventorySlot>();
    private PlayerControl playerControl;

    public int ActiveSlotIndex => activeSlotIndex;
    public InventorySlot ActiveSlot => slots.Count > activeSlotIndex ? slots[activeSlotIndex] : null;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeInventory();
        }
        else
        {
            Destroy(gameObject);
        }
        playerControl = FindFirstObjectByType<PlayerControl>();
    }

    private void Start()
    {
        if (playerControl == null)
        {
            Debug.LogError("PlayerControl не найден! Инвентарь не будет работать.");
            return;
        }
        {
            Debug.Log($"PlayerControl найден: {playerControl.gameObject.name}");
        }
    }

    private void OnEnable()
    {
        if (playerControl != null)
        {
            playerControl.OnSlotSelected += HandleSlotSelected;
            playerControl.OnScrollWheel += HandleScrollWheel;
            Debug.Log("Inventory подписался на события слотов");
        }
    }

    private void OnDisable()
    {
        if (playerControl != null)
        {
            playerControl.OnSlotSelected -= HandleSlotSelected;
            playerControl.OnScrollWheel -= HandleScrollWheel;
        }
    }

    private void InitializeInventory()
    {
        slots.Clear();
        for (int i = 0; i < slotCount; i++)
        {
            slots.Add(new InventorySlot());
        }

        if (logToConsole)
            Debug.Log($"Инвентарь инициализирован на {slotCount} слотов");
    }

    private void HandleSlotSelected(int slotIndex)
    {
        Debug.Log($"HandleSlotSelected вызван! Слот: {slotIndex}"); // Добавьте
        SetActiveSlot(slotIndex);
    }

    private void HandleScrollWheel(float scrollValue)
    {
        Debug.Log($"HandleScrollWheel вызван! Значение: {scrollValue}"); // Добавьте
        // Нормализуем значение колесика
        int direction = scrollValue > 0 ? -1 : 1;
        SwitchSlot(direction);
    }

    public bool AddItem(InventoryItem item, int amount = 1)
    {
        if (item == null || amount <= 0) return false;

        if (logToConsole)
            Debug.Log($"Попытка добавить {item.itemName} x{amount}");

        // Для стакаемых предметов - ищем существующий стек
        if (item.isStackable)
        {
            foreach (var slot in slots)
            {
                if (!slot.IsEmpty && slot.Item == item && slot.Amount < item.maxStackSize)
                {
                    int spaceLeft = item.maxStackSize - slot.Amount;
                    int toAdd = Mathf.Min(spaceLeft, amount);

                    slot.AddAmount(toAdd);
                    amount -= toAdd;

                    int index = slots.IndexOf(slot);
                    OnSlotChanged?.Invoke(index, slot);

                    if (logToConsole)
                        Debug.Log($"Добавлено в существующий слот {index + 1}: теперь {slot.Amount}");

                    if (amount <= 0) return true;
                }
            }
        }

        // Ищем пустой слот для оставшихся предметов
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].IsEmpty)
            {
                slots[i].SetItem(item, amount);
                OnSlotChanged?.Invoke(i, slots[i]);

                if (logToConsole)
                    Debug.Log($"Добавлено в новый слот {i + 1}: {item.itemName} x{amount}");

                return true;
            }
        }

        if (logToConsole)
            Debug.Log("Нет свободных слотов!");

        return false;
    }

    public bool RemoveActiveItem(int amount = 1)
    {
        return RemoveItem(activeSlotIndex, amount);
    }

    public bool RemoveItem(int slotIndex, int amount = 1)
    {
        if (slotIndex < 0 || slotIndex >= slots.Count) return false;

        var slot = slots[slotIndex];
        if (slot.IsEmpty) return false;

        bool success = slot.RemoveAmount(amount);

        if (success)
        {
            OnSlotChanged?.Invoke(slotIndex, slot);

            if (logToConsole)
                Debug.Log($"Удалено {amount} из слота {slotIndex + 1}");
        }

        return success;
    }

    public void SetActiveSlot(int slotIndex)
    {
        Debug.Log($"SetActiveSlot: пытаемся установить слот {slotIndex}"); // Добавьте
        if (slotIndex < 0 || slotIndex >= slotCount) return;

        if (slotIndex != activeSlotIndex)
        {
            activeSlotIndex = slotIndex;
            OnActiveSlotChanged?.Invoke(activeSlotIndex);

            if (logToConsole)
            {
                var slot = slots[activeSlotIndex];
                if (!slot.IsEmpty)
                    Debug.Log($"Активный слот: {activeSlotIndex + 1} - {slot.Item.itemName} x{slot.Amount}");
                else
                    Debug.Log($"Активный слот: {activeSlotIndex + 1} - пусто");
            }
        }
    }

    public void SwitchSlot(int direction)
    {
        int newIndex = activeSlotIndex + direction;
        if (newIndex < 0) newIndex = slotCount - 1;
        if (newIndex >= slotCount) newIndex = 0;

        SetActiveSlot(newIndex);
    }

    public void UseActiveItem()
    {
        if (ActiveSlot != null && !ActiveSlot.IsEmpty)
        {
            Debug.Log($"Использован {ActiveSlot.Item.itemName}");
            RemoveActiveItem(1);
        }
        else
        {
            Debug.Log("Активный слот пуст");
        }
    }

    public InventorySlot GetSlot(int index)
    {
        return index >= 0 && index < slots.Count ? slots[index] : null;
    }

    public List<InventorySlot> GetAllSlots()
    {
        return slots;
    }

    // Для отладки - вывести содержимое инвентаря в консоль
    public void PrintInventory()
    {
        Debug.Log("=== СОДЕРЖИМОЕ ИНВЕНТАРЯ ===");
        for (int i = 0; i < slots.Count; i++)
        {
            var slot = slots[i];
            if (slot.IsEmpty)
                Debug.Log($"Слот {i + 1}: пусто");
            else
                Debug.Log($"Слот {i + 1}: {slot.Item.itemName} x{slot.Amount}");
        }
        Debug.Log($"Активный слот: {activeSlotIndex + 1}");
    }
}