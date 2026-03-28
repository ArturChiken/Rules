using UnityEngine;

[System.Serializable]
public class InventorySlot
{
    [SerializeField] private InventoryItem item;
    [SerializeField] private int amount;

    public InventoryItem Item => item;
    public int Amount => amount;
    public bool IsEmpty => item == null || amount <= 0;

    public void SetItem(InventoryItem newItem, int newAmount)
    {
        item = newItem;
        amount = newAmount;
    }

    public void Clear()
    {
        item = null;
        amount = 0;
    }

    public void AddAmount(int value)
    {
        if (value > 0)
            amount += value;
    }

    public bool RemoveAmount(int value)
    {
        if (value <= 0 || amount < value) return false;

        amount -= value;
        if (amount <= 0)
            Clear();

        return true;
    }
}