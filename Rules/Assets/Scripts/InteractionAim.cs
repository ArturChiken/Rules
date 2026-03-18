using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class PlayerAim : MonoBehaviour
{
    [Header("Aim Settings")]
    [SerializeField] private float maxAimDistance = 100f;
    [SerializeField] private LayerMask aimLayers = -1;

    [Header("UI References")]
    [SerializeField] private Image aimImage;
    [SerializeField] private Sprite defaultAimSprite;
    [SerializeField] private Sprite interactableAimSprite;
    [SerializeField] private TMP_Text interactionPromptText; // Текст для подсказки

    [Header("Colors")]
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color interactableColor = Color.yellow;
    [SerializeField] private Color blockedColor = Color.red;

    private Camera playerCamera;
    private IInteractable currentInteractable;
    private GameObject currentHitObject;
    private InputSystem_Actions input;

    private void Awake()
    {
        input = new InputSystem_Actions();
        input.Player.Interact.performed += OnInteractPerformed;
    }

    private void Start()
    {
        playerCamera = GetComponent<Camera>();

        if (playerCamera == null)
            playerCamera = GetComponent<Camera>();

        // Скрываем текст подсказки по умолчанию
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

            // Проверяем, есть ли интерфейс IInteractable на объекте или его родителях
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null && interactable.CanInteract())
            {
                // Навели на интерактивный объект
                currentInteractable = interactable;
                UpdateInteractionPrompt(true, interactable.GetInteractionPrompt());
            }
            else
            {
                // Навели на обычный объект
                currentInteractable = null;
                UpdateInteractionPrompt(false, "");
            }
        }
        else
        {
            // Ничего не нашли
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

            // Показываем подсказку
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

            // Скрываем подсказку
            if (interactionPromptText != null)
                interactionPromptText.gameObject.SetActive(false);
        }
    }

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        if (currentInteractable != null)
        {
            currentInteractable.OnInteract();
        }
    }

    public IInteractable GetCurrentInteractable()
    {
        return currentInteractable;
    }

    public GameObject GetCurrentTarget()
    {
        return currentHitObject;
    }

    public bool HasInteractable()
    {
        return currentInteractable != null;
    }

    private void OnEnable()
    {
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }

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
