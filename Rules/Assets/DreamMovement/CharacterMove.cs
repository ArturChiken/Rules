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

            data.wasGrounded = data.controller.isGrounded;
        }

        private void Update()
        {

            if (data.controller.isGrounded && !data.wasGrounded)
            {
                data.currentVelocity.y = 0;
                config.SetVerticalVelocity(-0.5f);
            }

            Vector3 desiredMoveDirection = GetDesiredMoveDirection(data.move);

            if (data.controller.isGrounded)
            {
                HandleGroundedMovement(desiredMoveDirection);

                if (config.autoJump && data.move.magnitude > 0.1f)
                {
                    AutoJump();
                }
            }
            else
            {
                HandleAirMovement(desiredMoveDirection);
            }
            Gravity();


            data.controller.Move(data.currentVelocity * Time.deltaTime);

            data.wasGrounded = data.controller.isGrounded;
        }

        private Vector3 GetDesiredMoveDirection(Vector2 move)
        {
            if (move.magnitude < 0.1f)
            {
                return Vector3.zero;
            }
            Vector3 moveDirection = new Vector3(move.x, 0, move.y);

            if (move.magnitude > 1f);
            {
                moveDirection.Normalize();
            }

            Vector3 bodyForward = config.bodyTransform.forward;
            bodyForward.y = 0;
            bodyForward.Normalize();

            Vector3 bodyRight = config.bodyTransform.right;
            bodyRight.y = 0;
            bodyRight.Normalize();

            return (bodyForward * moveDirection.z + bodyRight * moveDirection.x).normalized;
        }

        private void HandleGroundedMovement(Vector3 dir)
        {
            if (dir.magnitude > 0.1f)
            {
                Vector3 targetVelocity = dir * config.speed;
                data.currentVelocity = Vector3.Lerp(data.currentVelocity, targetVelocity, Time.deltaTime * 10f);
            }
            else
            {
                data.currentVelocity = Vector3.Lerp(data.currentVelocity, Vector3.zero, Time.deltaTime * 5f);
            }
        }

        private void HandleAirMovement(Vector3 dir)
        {
            Vector3 horizontalVelocity = new Vector3(data.currentVelocity.x, 0, data.currentVelocity.z);

            horizontalVelocity -= horizontalVelocity * config.airDrag * Time.deltaTime * config.airDragMultiplier;

            if (dir.magnitude > 0.1f)
            {
                Vector3 airAcceliration = dir * config.speed * config.airControl * Time.deltaTime;
                horizontalVelocity += airAcceliration;

                if (horizontalVelocity.magnitude > config.maxAirSpeed)
                {
                    horizontalVelocity = horizontalVelocity.normalized * config.maxAirSpeed;
                }
            }

            data.currentVelocity.x = horizontalVelocity.x;
            data.currentVelocity.z = horizontalVelocity.z;
        }

        private void Gravity()
        {
            if (data.controller.isGrounded)
            {
                if (config.GetVerticalVelocity() < 0)
                {
                    config.SetVerticalVelocity(-0.5f);
                }
            }
            else
            {
                config.SetVerticalVelocity(config.GetVerticalVelocity() - config.gravitySpeed * Time.deltaTime);
            }

            data.currentVelocity.y = config.GetVerticalVelocity();
        }

        private void AutoJump()
        {
            if (true)
            {
                JumpHandler();
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
                data.currentVelocity.y = config.jumpForce;
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
