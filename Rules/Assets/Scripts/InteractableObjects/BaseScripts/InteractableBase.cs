using UnityEngine;

public abstract class InteractableBase : MonoBehaviour, IInteractable
{
    [Header("Interaction Settings")]
    [SerializeField] protected string interactionPrompt = "Press E to interact";
    [SerializeField] protected bool canInteract = true;

    public virtual void OnInteract()
    {
        // Будет переопределено в дочерних классах
    }

    public virtual string GetInteractionPrompt()
    {
        return interactionPrompt;
    }

    public virtual bool CanInteract()
    {
        return canInteract;
    }
}