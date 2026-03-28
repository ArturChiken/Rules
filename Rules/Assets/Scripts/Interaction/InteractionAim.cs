using DreamMovement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAim : MonoBehaviour
{
    [Header("Aim Settings")]
    [SerializeField] private float maxAimDistance = 100f;
    [SerializeField] private LayerMask aimLayers = -1;

    [Header("UI References")]
    [SerializeField] private Image aimImage;
    [SerializeField] private Sprite defaultAimSprite;
    [SerializeField] private Sprite interactableAimSprite;
    [SerializeField] private TMP_Text interactionPromptText;

    [Header("Colors")]
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color interactableColor = Color.yellow;
    [SerializeField] private Color blockedColor = Color.red;

    private Camera playerCamera;
    private IInteractable currentInteractable;
    private GameObject currentHitObject;
    private PlayerControl playerControl;

    private void Awake()
    {
        playerControl = GetComponent<PlayerControl>();
        if (playerControl == null)
            playerControl = FindFirstObjectByType<PlayerControl>();
    }

    private void OnEnable()
    {
        if (playerControl != null)
        {
            playerControl.OnInteract += HandleInteract;
        }
    }

    private void OnDisable()
    {
        if (playerControl != null)
        {
            playerControl.OnInteract -= HandleInteract;
        }
    }

    private void Start()
    {
        playerCamera = GetComponent<Camera>();

        if (playerCamera == null)
            playerCamera = GetComponent<Camera>();

        if (interactionPromptText != null)
            interactionPromptText.gameObject.SetActive(false);
    }

    private void Update()
    {
        UpdateInteractionAim();
    }

    private void UpdateInteractionAim()
    {
        if (playerCamera == null) return;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxAimDistance, aimLayers))
        {
            currentHitObject = hit.collider.gameObject;
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null && interactable.CanInteract())
            {
                currentInteractable = interactable;
                UpdateInteractionPrompt(true, interactable.GetInteractionPrompt());
            }
            else
            {
                currentInteractable = null;
                UpdateInteractionPrompt(false, "");
            }
        }
        else
        {
            currentHitObject = null;
            currentInteractable = null;
            UpdateInteractionPrompt(false, "");
        }
    }

    private void UpdateInteractionPrompt(bool isInteractable, string prompt = "")
    {
        if (aimImage == null) return;

        if (isInteractable)
        {
            aimImage.color = interactableColor;
            if (interactableAimSprite != null)
                aimImage.sprite = interactableAimSprite;

            if (interactionPromptText != null)
            {
                interactionPromptText.text = prompt;
                interactionPromptText.gameObject.SetActive(true);
            }
        }
        else
        {
            aimImage.color = defaultColor;
            if (defaultAimSprite != null)
                aimImage.sprite = defaultAimSprite;

            if (interactionPromptText != null)
                interactionPromptText.gameObject.SetActive(false);
        }
    }

    private void HandleInteract()
    {
        if (currentInteractable != null)
        {
            currentInteractable.OnInteract();
        }
    }

    public IInteractable GetCurrentInteractable() => currentInteractable;
    public GameObject GetCurrentTarget() => currentHitObject;
    public bool HasInteractable() => currentInteractable != null;

    private void OnDrawGizmosSelected()
    {
        if (playerCamera != null)
        {
            Gizmos.color = Color.yellow;
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            Gizmos.DrawRay(ray.origin, ray.direction * maxAimDistance);
        }
    }
}