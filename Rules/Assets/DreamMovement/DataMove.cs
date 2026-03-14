using UnityEngine;

namespace DreamMovement
{
    [RequireComponent(typeof(CharacterController))]
    public class DataMove
    {


        public InputSystem_Actions input;
        public CharacterController controller;
        public Vector2 move;

        public DataMove()
        {
            input = new InputSystem_Actions();
        }
    }
}