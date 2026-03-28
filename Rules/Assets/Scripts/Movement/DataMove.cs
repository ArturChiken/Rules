using UnityEngine;

namespace DreamMovement
{
    [RequireComponent(typeof(CharacterController))]
    public class DataMove
    {

        public CharacterController controller;
        public Vector2 move;

        public Vector3 currentVelocity = Vector3.zero;
        public bool wasGrounded;

        public bool isNoclip = false;
        public bool wasNoclip = false;
        public Vector3 noclipVelocity;

    }
}