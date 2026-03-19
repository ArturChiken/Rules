using UnityEngine;
using DreamMovement;

public class WorldItem : InteractableBase
{
    [Header("Item Data")]
    [SerializeField] private InventoryItem itemData;
    [SerializeField] private int amount = 1;

    [Header("Visual")]
    [SerializeField] private float rotationSpeed = 30f;
    [SerializeField] private bool rotateInWorld = true;

    private void Update()
    {
        if (rotateInWorld)
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
    }

    public override void OnInteract()
    {
        if (!canInteract) return;

        if (Inventory.Instance != null)
        {
            bool added = Inventory.Instance.AddItem(itemData, amount);

            if (added)
            {
                Debug.Log($"Подобрано: {itemData.itemName} x{amount}");
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("Инвентарь полон!");
            }
        }
    }

    public override string GetInteractionPrompt()
    {
        return $"Нажмите E чтобы подобрать {itemData.itemName} x{amount}";
    }
}