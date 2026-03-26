using System.Data;
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

            data.input.Player.Noclip.performed += OnNoclipPerformed;
        }

        private void Start()
        {
            data.controller = GetComponent<CharacterController>();

            data.wasGrounded = data.controller.isGrounded;
        }

        private void Update()
        {
            if (data.isNoclip)
            {
                HandleNoclipMovement();

                data.controller.Move(data.currentVelocity *  Time.deltaTime);

                if (data.isNoclip) //config.noclipSpeedMultiplier > 0
                {
                    transform.position = data.controller.transform.position;
                }

                data.wasGrounded = false;
                return;
            }

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

            Transform referenceTransform = data.isNoclip ? transform : config.bodyTransform;

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
            if (data.controller.isGrounded && !data.isNoclip)
            {
                config.SetVerticalVelocity(config.jumpForce);
                data.currentVelocity.y = config.jumpForce;
            }
            else if (data.isNoclip)
            {
                data.currentVelocity.y = config.speed * 0.8f;
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

        public void OnNoclipPerformed(InputAction.CallbackContext context)
        {
            ToggleNoclip();
        }

        public void ToggleNoclip()
        {
            data.isNoclip = !data.isNoclip;

            if (data.isNoclip)
            {
                EnableNoclip();
            }
            else
            {
                DisableNoclip();
            }
        }

        public void EnableNoclip()
        {
            data.noclipVelocity = data.currentVelocity;

            config.SetVerticalVelocity(0);

            if (config.noclipDisableCollisions)
            {
                data.controller.detectCollisions = false;
            }
            Debug.Log("Noclip enabled");
        }

        public void DisableNoclip()
        {
            if (config.noclipDisableCollisions)
            {
                data.controller.detectCollisions = true;
            }

            if (!data.controller.isGrounded)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, Vector3.down, out hit, 10f))
                {
                    Vector3 newPosition = transform.position;
                    newPosition.y = hit.point.y + data.controller.height / 2;
                    data.controller.transform.position = newPosition;
                }
            }

            Debug.Log("Noclip disabled");
        }

        public void HandleNoclipMovement()
        {
            Vector3 dir = GetDesiredMoveDirection(data.move);

            Vector3 targetVelocity = dir * config.speed;

            data.noclipVelocity = Vector3.Lerp(data.noclipVelocity, targetVelocity, Time.deltaTime * 8f);
            data.currentVelocity = data.noclipVelocity;

            config.SetVerticalVelocity(0);

            config.bodyTransform.eulerAngles = Vector3.Scale(transform.eulerAngles, new Vector3(0f, 1f, 0f));
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
