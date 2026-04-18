using System;
using EffectSystem;
using EffectSystem.Effects;
using Player.Data;
using Player.Variables;
using UnityEngine;
using Utils.VariablesSystem;
using VContainer;

namespace Player
{
    public class PlayerMovement : MonoBehaviour, IEffectAddicted
    {
        [SerializeField] private Rigidbody2D rb;

        [Inject] private PlayerInput _input;
        [Inject] private PlayerSettingsSO _playerSettings;
        [Inject] private IVariablesResolver<PlayerVariableBlockerType, Action, Action> _playerVariables;

        private readonly PlayerVariableBlocker _stunBlocker =
            new(PlayerVariableBlockerType.MOVEMENT, PlayerVariableBlockerType.SEND_SIGNAL);

        private BaseEffect _currentEffect;
        
        private void Update()
        {
            if (_playerVariables.IsVariableBlocked(PlayerVariableBlockerType.MOVEMENT))
                return;

            rb.linearVelocity = _input.ReadMoveVector() * _playerSettings.MoveSpeed;
        }

        public bool ApplyEffect(BaseEffect effect)
        {
            if (effect is not StunEffect || _currentEffect is not (null or { IsActive: false }))
                return false;
            
            _currentEffect = effect;
            _playerVariables.RegisterBlocker(_stunBlocker);
            return true;
        }

        public void DisposeEffect(BaseEffect effect)
        {
            if (effect is not StunEffect || _currentEffect == null || _currentEffect != effect)
                return;
            
            _stunBlocker.Dispose();
            _currentEffect = null;
        }
    }
}