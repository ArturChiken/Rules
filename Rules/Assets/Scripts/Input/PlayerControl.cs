using UnityEngine;
using UnityEngine.InputSystem;

namespace DreamMovement
{
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
        public System.Action<Vector2> OnMove;
        public System.Action<Vector2> OnLook;
        public System.Action OnJump;
        public System.Action OnInteract;
        public System.Action OnAttack;
        public System.Action OnCrouch;
        public System.Action OnSprint;
        public System.Action OnPrevious;
        public System.Action OnNext;

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
            inputActions.Player.Previous.performed += OnPreviousPerformed;
            inputActions.Player.Next.performed += OnNextPerformed;
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
            inputActions.Player.Previous.performed -= OnPreviousPerformed;
            inputActions.Player.Next.performed -= OnNextPerformed;

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

        private void OnPreviousPerformed(InputAction.CallbackContext context)
        {
            OnPrevious?.Invoke();
        }

        private void OnNextPerformed(InputAction.CallbackContext context)
        {
            OnNext?.Invoke();
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
}