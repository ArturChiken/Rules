using UnityEngine;

public abstract class InteractableBase : MonoBehaviour, IInteractable
{
    [Header("Interaction Settings")]
    [SerializeField] protected string interactionPrompt = "Press E to interact";
    [SerializeField] protected bool canInteract = true;

    public virtual void OnInteract()
    {
        Debug.Log($"Взаимодействие с {gameObject.name}");
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