using UnityEngine;
using UnityEngine.InputSystem;

namespace DreamMovement
{
    [System.Serializable]
    public class AimMove : MonoBehaviour
    {
        [Header("References")]
        public Transform bodyTransform;

        [Header("Sensitivity")]
        public float sensitivityMultiplier = 1f;
        public float horizontalSensitivity = 1f;
        public float verticalSensitivity = 1f;

        [Header("Restrictions")]
        public float minYRotation = -90f;
        public float maxYRotation = 90f;

        private Vector3 realRotation;

        private float punchDamping = 9.0f;
        private float punchSpringConstant = 65.0f;

        [HideInInspector]
        public Vector2 punchAngle;

        [HideInInspector]
        public Vector2 punchAngleVel;


        private InputSystem_Actions input;
        private InputAction action;

        private void Awake()
        {
            input = new InputSystem_Actions();
            action = input.Player.Look;
        }

        private void OnEnable()
        {
            input.Enable();
        }

        private void OnDisable()
        {
            input.Disable();
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            if (Mathf.Abs(Time.timeScale) <= 0)
                return;

            DecayPunchAngle();

            Vector2 lookInput = action.ReadValue<Vector2>();

            float xMovement = lookInput.x * horizontalSensitivity * sensitivityMultiplier;
            float yMovement = -lookInput.y * verticalSensitivity * sensitivityMultiplier;

            realRotation = new Vector3(Mathf.Clamp(realRotation.x + yMovement, minYRotation, maxYRotation), realRotation.y + xMovement, realRotation.z);
            realRotation.z = Mathf.Lerp(realRotation.z, 0f, Time.deltaTime * 3f);

            bodyTransform.rotation = Quaternion.Euler(0f, realRotation.y, 0f);

            Vector3 cameraEulerPunchApplied = realRotation;
            cameraEulerPunchApplied.x += punchAngle.x;
            cameraEulerPunchApplied.y += punchAngle.y;

            transform.eulerAngles = cameraEulerPunchApplied;
        }

        public void ViewPunch(Vector2 punchAmount)
        {
            punchAngle = Vector2.zero;

            punchAngleVel -= punchAmount * 20;
        }

        private void DecayPunchAngle()
        {
            if (punchAngle.sqrMagnitude > 0.001 || punchAngleVel.sqrMagnitude > 0.001)
            {
                punchAngle += punchAngleVel * Time.deltaTime;
                float damping = 1 - (punchDamping * Time.deltaTime);

                if (damping < 0)
                    damping = 0;

                punchAngleVel *= damping;

                float springForceMagnitude = punchSpringConstant * Time.deltaTime;
                punchAngleVel -= punchAngle * springForceMagnitude;
            }
            else
            {
                punchAngle = Vector2.zero;
                punchAngleVel = Vector2.zero;
            }
        }
    }
}
