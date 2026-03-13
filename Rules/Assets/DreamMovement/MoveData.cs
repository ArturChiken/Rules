using UnityEngine;

namespace DreamMovement
{
    [RequireComponent(typeof(CharacterController))]
    public class MoveData
    {
        public InputSystem_Actions input;
        public CharacterController controller;
        public Vector3 moveDir;
        public Vector2 move;

        public MoveData()
        {
            input = new InputSystem_Actions();
        }
    }
}