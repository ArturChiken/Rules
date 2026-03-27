using UnityEngine;

namespace DreamMovement
{
    [System.Serializable]
    public class ConfigMove
    {
        [Header("Move Config")]
        public float speed;
        public float gravitySpeed = 9.8f;
        public float jumpForce;
        [SerializeField] private float _verticalVelocity;
        [SerializeField] public Transform bodyTransform;

        [Header("Air Control")]
        public float maxAirSpeed = 8.0f;
        [Range(0f, 1f)]
        public float airControl = 0.3f;
        [Range(0f, 1f)]
        public float airDrag = 0.95f;
        public float airDragMultiplier = 2f;

        [Header("Noclip")]
        public bool autoJump = false;
        public bool noclipDisableCollisions = true;
        public float noclipSpeed = 15f;

        public float GetVerticalVelocity()
        {
            return _verticalVelocity;
        }

        public void SetVerticalVelocity(float value)
        {
            _verticalVelocity = value;
        }
    }
}