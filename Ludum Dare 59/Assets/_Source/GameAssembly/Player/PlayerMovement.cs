using System;
using Player.Data;
using UnityEngine;
using Utils.VariablesSystem;
using VContainer;

namespace Player
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D rb;
        
        [Inject] private PlayerInput _input;
        [Inject] private PlayerSettingsSO _playerSettings;
        [Inject] private IVariablesResolver<PlayerVariableBlockerType, Action, Action> _playerVariables;

        private void Update()
        {
            if(_playerVariables.IsVariableBlocked(PlayerVariableBlockerType.MOVEMENT))
                return;
            
            rb.linearVelocity = _input.ReadMoveVector() * _playerSettings.MoveSpeed;
        }
    }
}