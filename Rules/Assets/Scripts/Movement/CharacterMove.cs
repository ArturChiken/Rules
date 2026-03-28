using System.Data;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DreamMovement
{
    public class CharacterMove : MonoBehaviour
    {
        [SerializeField] public ConfigMove config;
        public DataMove data;

        private int originalExcludeLayers;

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

            originalExcludeLayers = data.controller.excludeLayers;

            if (config.bodyTransform == null)
            {
                Debug.LogError("bodyTransform not assigned in ConfigMove!");
            }
        }

        private void Update()
        {
            if (data.isNoclip)
            {
                HandleNoclipMovement();

                data.controller.Move(data.currentVelocity * Time.deltaTime);

                if (config.speed > 0) //config.noclipSpeedMultiplier > 0
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

            

            if (data.isNoclip)
            {
                Transform cameraTransform = transform;

                Vector3 forward = cameraTransform.forward;
                Vector3 right = cameraTransform.right;

                Vector3 moveVector = (forward * moveDirection.z + right * moveDirection.x).normalized;

                if (move.magnitude <0.1f)
                {
                    return Vector3.zero;
                }
                return moveVector;
            }
            else
            {
                Transform referenceTransform = config.bodyTransform != null ? config.bodyTransform : transform;
                Vector3 forward = referenceTransform.forward;
                Vector3 right = referenceTransform.right;


                forward.y = 0;
                forward.Normalize();

                right.y = 0;
                right.Normalize();

                return (forward * moveDirection.z + right * moveDirection.x).normalized;
            }
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

            float airResistance = 1 - config.airDrag;

            horizontalVelocity -= horizontalVelocity * airResistance * Time.deltaTime * config.airDragMultiplier;

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
                Debug.Log("Cannot jump in noclip mode");
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

            data.controller.detectCollisions = false;
            data.controller.excludeLayers = ~0;

            Debug.Log("Noclip enabled");
        }

        public void DisableNoclip()
        {
            data.controller.detectCollisions = true;
            data.controller.excludeLayers = originalExcludeLayers;

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
            Vector2 inputMove = data.move;

            if (inputMove.magnitude < 0.1f)
            {
                data.noclipVelocity = Vector3.Lerp(data.noclipVelocity, Vector3.zero, Time.deltaTime * 5f);
                data.currentVelocity = data.noclipVelocity;
                return;
            }

            Vector3 moveDirection = new Vector3(inputMove.x, 0, inputMove.y);
            if (moveDirection.magnitude > 1f)
            {
                moveDirection.Normalize();
            }

            Transform cameraTransform = transform;
            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;

            Vector3 targetVelocity = (forward * moveDirection.z + right * moveDirection.x).normalized * config.noclipSpeed;

            data.noclipVelocity = Vector3.Lerp(data.noclipVelocity, targetVelocity, Time.deltaTime * 8f);
            data.currentVelocity = data.noclipVelocity;

            config.SetVerticalVelocity(0);

            if (config.bodyTransform != null)
            {
                config.bodyTransform.eulerAngles = Vector3.Scale(transform.eulerAngles, new Vector3(0f, 1f, 0f));
            }
        }

        private void OnEnable()
        {
            data.input.Enable();
        }

        private void OnDisable()
        {
            data.input.Disable();
        }

        private void OnDestroy()
        {
            data.controller.detectCollisions = true;
            data.controller.excludeLayers = originalExcludeLayers;
        }
    }
}
