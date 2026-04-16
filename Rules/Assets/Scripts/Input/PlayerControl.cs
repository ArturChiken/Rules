using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Центральный класс для управления всеми инпутами игрока
/// </summary>
public class PlayerControl : MonoBehaviour
{
    [Header("Input Settings")]
    [SerializeField] private bool cursorLocked = true;
    [SerializeField] private bool cursorVisible = false;

    // Единственный экземпляр Input Actions
    private InputSystem_Actions inputActions;

    // Свойство для доступа к Input Actions из других классов
    public InputSystem_Actions InputActions => inputActions;

    // События для всех действий (чтобы другие классы могли подписываться)
        
    // Управление персонажем
    public System.Action<Vector2> OnMove;
    public System.Action<Vector2> OnLook;
    public System.Action OnJump;
    public System.Action OnInteract;
    public System.Action OnAttack;
    public System.Action OnCrouch;
    public System.Action OnSprint;
    public System.Action OnNoclip;
    public System.Action OnToggleWatch;

    // Управление часами
    public System.Action OnWatchLeftClick;
    public System.Action OnWatchRightClick;
    public System.Action OnWatchBackspace;
    public System.Action<int> OnDigitPressed;

    // Встроенные события
    public System.Action OnPrevious;
    public System.Action OnNext;

    // Инвентарь
    public System.Action<int> OnSlotSelected;
    public System.Action<float> OnScrollWheel;

    [Header("Input Blocking")]
    private bool isWatchOpen = false;

    public void BlockOtherInputs(bool block)
    {
        isWatchOpen = block;

        // Здесь можно блокировать другие системы
        // Например, через события или флаги
    }


    private void Awake()
    {
        // Создаем экземпляр Input Actions
        inputActions = new InputSystem_Actions();

        // Настраиваем курсор
        if (cursorLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = cursorVisible;
        }
    }

    private void OnEnable()
    {
        inputActions.Enable();

        // Подписываемся на все действия
        inputActions.Player.Move.performed += OnMovePerformed;
        inputActions.Player.Move.canceled += OnMoveCanceled;

        inputActions.Player.Look.performed += OnLookPerformed;
        inputActions.Player.Look.canceled += OnLookCanceled;

        inputActions.Player.Jump.performed += OnJumpPerformed;
        inputActions.Player.Interact.performed += OnInteractPerformed;
        inputActions.Player.Attack.performed += OnAttackPerformed;
        inputActions.Player.Crouch.performed += OnCrouchPerformed;
        inputActions.Player.Sprint.performed += OnSprintPerformed;
        inputActions.Player.Noclip.performed += OnNoclipPerformed;

        inputActions.Player.ToggleWatch.performed += OnToggleWatchPerformed;
        inputActions.Player.WatchLeftClick.performed += OnWatchLeftClickPerformed;
        inputActions.Player.WatchRightClick.performed += OnWatchRightClickPerformed;
        inputActions.Player.WatchBackspace.performed += OnWatchBackspacePerformed;
        inputActions.Player.Digit0.performed += OnDigitPerformed;
        inputActions.Player.Digit1.performed += OnDigitPerformed;
        inputActions.Player.Digit2.performed += OnDigitPerformed;
        inputActions.Player.Digit3.performed += OnDigitPerformed;
        inputActions.Player.Digit4.performed += OnDigitPerformed;
        inputActions.Player.Digit5.performed += OnDigitPerformed;
        inputActions.Player.Digit6.performed += OnDigitPerformed;
        inputActions.Player.Digit7.performed += OnDigitPerformed;
        inputActions.Player.Digit8.performed += OnDigitPerformed;
        inputActions.Player.Digit9.performed += OnDigitPerformed;

        inputActions.Player.Previous.performed += OnPreviousPerformed;
        inputActions.Player.Next.performed += OnNextPerformed;

        inputActions.Player.Slot1.performed += OnSlot1Performed;
        inputActions.Player.Slot2.performed += OnSlot2Performed;
        inputActions.Player.Slot3.performed += OnSlot3Performed;
        inputActions.Player.Slot4.performed += OnSlot4Performed;
        inputActions.Player.ScrollWheel.performed += OnScrollWheelPerformed;
    }

    private void OnDisable()
    {
        // Отписываемся от всех действий
        inputActions.Player.Move.performed -= OnMovePerformed;
        inputActions.Player.Move.canceled -= OnMoveCanceled;

        inputActions.Player.Look.performed -= OnLookPerformed;
        inputActions.Player.Look.canceled -= OnLookCanceled;

        inputActions.Player.Jump.performed -= OnJumpPerformed;
        inputActions.Player.Interact.performed -= OnInteractPerformed;
        inputActions.Player.Attack.performed -= OnAttackPerformed;
        inputActions.Player.Crouch.performed -= OnCrouchPerformed;
        inputActions.Player.Sprint.performed -= OnSprintPerformed;
        inputActions.Player.Noclip.performed -= OnNoclipPerformed;

        inputActions.Player.ToggleWatch.performed -= OnToggleWatchPerformed;
        inputActions.Player.WatchLeftClick.performed -= OnWatchLeftClickPerformed;
        inputActions.Player.WatchRightClick.performed -= OnWatchRightClickPerformed;
        inputActions.Player.WatchBackspace.performed -= OnWatchBackspacePerformed;
        inputActions.Player.Digit0.performed -= OnDigitPerformed;
        inputActions.Player.Digit1.performed -= OnDigitPerformed;
        inputActions.Player.Digit2.performed -= OnDigitPerformed;
        inputActions.Player.Digit3.performed -= OnDigitPerformed;
        inputActions.Player.Digit4.performed -= OnDigitPerformed;
        inputActions.Player.Digit5.performed -= OnDigitPerformed;
        inputActions.Player.Digit6.performed -= OnDigitPerformed;
        inputActions.Player.Digit7.performed -= OnDigitPerformed;
        inputActions.Player.Digit8.performed -= OnDigitPerformed;
        inputActions.Player.Digit9.performed -= OnDigitPerformed;
        inputActions.Player.Digit0.performed -= OnDigitPerformed;
            
        inputActions.Player.Previous.performed -= OnPreviousPerformed;
        inputActions.Player.Next.performed -= OnNextPerformed;

        inputActions.Player.Slot1.performed -= OnSlot1Performed;
        inputActions.Player.Slot2.performed -= OnSlot2Performed;
        inputActions.Player.Slot3.performed -= OnSlot3Performed;
        inputActions.Player.Slot4.performed -= OnSlot4Performed;
        inputActions.Player.ScrollWheel.performed -= OnScrollWheelPerformed;

        inputActions.Disable();
    }

    // Обработчики Move
    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        OnMove?.Invoke(context.ReadValue<Vector2>());
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        OnMove?.Invoke(Vector2.zero);
    }

    // Обработчики Look
    private void OnLookPerformed(InputAction.CallbackContext context)
    {
        OnLook?.Invoke(context.ReadValue<Vector2>());
    }

    private void OnLookCanceled(InputAction.CallbackContext context)
    {
        OnLook?.Invoke(Vector2.zero);
    }

    // Обработчики кнопок
    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        OnJump?.Invoke();
    }

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        if (isWatchOpen) return;  // Блокируем взаимодействие при открытых часах
        OnInteract?.Invoke();
    }

    private void OnAttackPerformed(InputAction.CallbackContext context)
    {
        OnAttack?.Invoke();
    }

    private void OnCrouchPerformed(InputAction.CallbackContext context)
    {
        OnCrouch?.Invoke();
    }

    private void OnSprintPerformed(InputAction.CallbackContext context)
    {
        OnSprint?.Invoke();
    }

    private void OnNoclipPerformed(InputAction.CallbackContext context)
    {
        OnNoclip?.Invoke();
    }

    private void OnToggleWatchPerformed(InputAction.CallbackContext context)
    {
        OnToggleWatch?.Invoke();
    }

    private void OnWatchLeftClickPerformed(InputAction.CallbackContext context)
    {
        OnWatchLeftClick?.Invoke();
    }

    private void OnWatchRightClickPerformed(InputAction.CallbackContext context)
    {
        OnWatchRightClick?.Invoke();
    }

    private void OnWatchBackspacePerformed(InputAction.CallbackContext context)
    {
        OnWatchBackspace?.Invoke();
    }

    private void OnDigitPerformed(InputAction.CallbackContext context)
    {
        int digit = int.Parse(context.action.name.Replace("Digit", ""));
        OnDigitPressed?.Invoke(digit);
    }

private void OnPreviousPerformed(InputAction.CallbackContext context)
    {
        OnPrevious?.Invoke();
    }

    private void OnNextPerformed(InputAction.CallbackContext context)
    {
        OnNext?.Invoke();
    }

    private void OnSlot1Performed(InputAction.CallbackContext context)
    {
        if (isWatchOpen) return;
        OnSlotSelected?.Invoke(0);
    }

    private void OnSlot2Performed(InputAction.CallbackContext context)
    {
        if (isWatchOpen) return;
        OnSlotSelected?.Invoke(1);
    }

    private void OnSlot3Performed(InputAction.CallbackContext context)
    {
        if (isWatchOpen) return;
        OnSlotSelected?.Invoke(2);
    }

    private void OnSlot4Performed(InputAction.CallbackContext context)
    {
        if (isWatchOpen) return;
        OnSlotSelected?.Invoke(3);
    }

    private void OnScrollWheelPerformed(InputAction.CallbackContext context)
    {
        if (isWatchOpen) return;
        Vector2 value = context.ReadValue<Vector2>();
        OnScrollWheel?.Invoke(value.y);
    }

    // Методы для прямого доступа к значениям (для тех, кому удобнее)
    public Vector2 GetMoveInput()
    {
        return inputActions.Player.Move.ReadValue<Vector2>();
    }

    public Vector2 GetLookInput()
    {
        return inputActions.Player.Look.ReadValue<Vector2>();
    }

    public bool GetJumpPressed()
    {
        return inputActions.Player.Jump.WasPressedThisFrame();
    }

    public bool GetInteractPressed()
    {
        return inputActions.Player.Interact.WasPressedThisFrame();
    }

    public bool GetAttackPressed()
    {
        return inputActions.Player.Attack.WasPressedThisFrame();
    }

    public bool GetCrouchPressed()
    {
        return inputActions.Player.Crouch.WasPressedThisFrame();
    }

    public bool GetSprintHeld()
    {
        return inputActions.Player.Sprint.IsPressed();
    }

    // Методы для управления курсором
    public void LockCursor(bool lockCursor)
    {
        Cursor.lockState = lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !lockCursor;
    }

    public void ToggleCursor()
    {
        Cursor.lockState = Cursor.lockState == CursorLockMode.Locked
            ? CursorLockMode.None
            : CursorLockMode.Locked;
        Cursor.visible = Cursor.lockState != CursorLockMode.Locked;
    }
}