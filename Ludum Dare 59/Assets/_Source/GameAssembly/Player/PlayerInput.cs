using System;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace Player
{
    public class PlayerInput
    {
        private InputSystem_Actions _inputActions;

        public event Action OnSendSignalClicked;

        [Inject]
        private void Construct(InputSystem_Actions inputActions)
        {
            _inputActions =  inputActions;
            Bind();
        }

        ~PlayerInput() => Expose();
        
        private void OnSendSignalClick(InputAction.CallbackContext callbackContext) => OnSendSignalClicked?.Invoke();

        public Vector2 ReadMoveVector() => _inputActions.Player.Move.ReadValue<Vector2>(); // Normalized in asset

        private void Bind()
        {
            _inputActions.Player.SendSignal.performed += OnSendSignalClick;
        }

        private void Expose()
        {
            _inputActions.Player.SendSignal.performed -= OnSendSignalClick;
        }
    }
}