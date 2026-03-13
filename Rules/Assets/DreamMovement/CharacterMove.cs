using UnityEngine;
using UnityEngine.InputSystem;

namespace DreamMovement
{
    public class CharacterMove : MonoBehaviour
    {
        [SerializeField] public MoveConfig config;
        public MoveData data;


        private void Awake()
        {
            data = new MoveData();

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
            data.moveDir = new Vector3(data.move.x, config.GetVerticalVelocity(), data.move.y);

            if (data.moveDir.magnitude > 0)
            {
                data.controller.Move(data.moveDir * config.speed * Time.deltaTime);
            }

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
