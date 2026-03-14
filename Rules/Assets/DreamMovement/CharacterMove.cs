using UnityEngine;
using UnityEngine.InputSystem;

namespace DreamMovement
{
    public class CharacterMove : MonoBehaviour
    {
        [SerializeField] public ConfigMove config;
        public DataMove data;


        private void Awake()
        {
            data = new DataMove();

            data.input.Player.Jump.performed += OnJumpPerformed;

            data.input.Player.Move.performed += OnMovePerformed;
            data.input.Player.Move.canceled += OnMoveCanceled;
        }

        private void Start()
        {
            data.controller = GetComponent<CharacterController>();
        }

        private void Update()
        {
            Vector3 moveDirection = new Vector3(data.move.x, 0, data.move.y).normalized;

            Vector3 bodyForward = config.bodyTransform.forward;
            bodyForward.y = 0;
            bodyForward.Normalize();

            Vector3 bodyRight = config.bodyTransform.right;
            bodyRight.y = 0;
            bodyRight.Normalize();

            Vector3 worldMoveDirection = bodyForward * moveDirection.z + bodyRight * moveDirection.x;

            worldMoveDirection.y = config.GetVerticalVelocity();

            data.controller.Move(worldMoveDirection * config.speed * Time.deltaTime);

            Gravity();
        }

        private void Gravity()
        {
            if (data.controller.isGrounded)
            {
                config.SetVerticalVelocity(-0.5f);
            }
            else
            {
                config.SetVerticalVelocity(config.GetVerticalVelocity() - config.gravitySpeed * Time.deltaTime);
            }
        }

        private void OnJumpPerformed(InputAction.CallbackContext context)
        {
            JumpHandler();
        }

        private void JumpHandler()
        {
            if (data.controller.isGrounded)
            {
                config.SetVerticalVelocity(config.jumpForce);
            }
        }

        private void OnMovePerformed(InputAction.CallbackContext context)
        {
            data.move = context.ReadValue<Vector2>();
        }

        private void OnMoveCanceled(InputAction.CallbackContext context)
        {
            data.move = Vector2.zero;
        }

        private void OnEnable()
        {
            data.input.Enable();
        }

        private void OnDisable()
        {
            data.input.Disable();
        }
    }
}
