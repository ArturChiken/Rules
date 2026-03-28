using UnityEngine;

public class Button : InteractableBase
{
    [SerializeField] private GameObject doorToOpen;

    public override void OnInteract()
    {
        if (!canInteract) return;

        Debug.Log("Button pressed!");
        // Активируйте что-то
    }
}