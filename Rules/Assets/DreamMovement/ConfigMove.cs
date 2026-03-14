using UnityEngine;

namespace DreamMovement
{
    [System.Serializable]
    public class ConfigMove
    {
        public float speed;
        public float gravitySpeed = 9.8f;
        public float jumpForce;
        [SerializeField] private float _verticalVelocity;
        [SerializeField] public Transform bodyTransform;

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