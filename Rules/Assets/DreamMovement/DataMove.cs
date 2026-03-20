using UnityEngine;

namespace DreamMovement
{
    [RequireComponent(typeof(CharacterController))]
    public class DataMove
    {
        public InputSystem_Actions input;
        public CharacterController controller;
        public Vector2 move;

        public Vector3 currentVelocity = Vector3.zero;
        public bool wasGrounded;

        public DataMove()
        {
            input = new InputSystem_Actions();
        }
    }
}