using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class InventoryItem : ScriptableObject
{
    public string itemName;
    public Sprite itemIcon; // для будущего GUI
    public bool isStackable = false;
    public int maxStackSize = 1;
}