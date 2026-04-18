using UnityEngine;
using VContainer;

namespace Player
{
    public class PlayerInput
    {
        [Inject] private InputSystem_Actions _inputActions;

        public PlayerInput()
        {
            Bind();
        }

        ~PlayerInput() => Expose();

        public Vector2 ReadMoveVector() => _inputActions.Player.Move.ReadValue<Vector2>(); // Normalized in asset

        private void Bind()
        {
            
        }

        private void Expose()
        {
            
        }
    }
}