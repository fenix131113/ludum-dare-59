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

        private StunEffect _currentStunEffect;
        private SlipEffect _currentSlipEffect;
        private Vector2 _slipVelocity;
        
        private void Update()
        {
            if (_currentSlipEffect is { IsActive: true })
            {
                _slipVelocity = Vector2.MoveTowards(
                    _slipVelocity,
                    Vector2.zero,
                    _currentSlipEffect.Deceleration * Time.deltaTime);
                rb.linearVelocity = _slipVelocity;
                return;
            }

            if (_playerVariables.IsVariableBlocked(PlayerVariableBlockerType.MOVEMENT))
            {
                rb.linearVelocity = Vector2.zero;
                return;
            }

            rb.linearVelocity = _input.ReadMoveVector() * _playerSettings.MoveSpeed;
        }

        public bool ApplyEffect(BaseEffect effect)
        {
            if (effect is StunEffect stunEffect)
            {
                if (_currentStunEffect is { IsActive: true })
                    return false;
                
                _currentStunEffect = stunEffect;
                _playerVariables.RegisterBlocker(_stunBlocker);
                return true;
            }

            if (effect is SlipEffect slipEffect)
            {
                if (_currentSlipEffect is { IsActive: true } || rb.linearVelocity.sqrMagnitude <= Mathf.Epsilon)
                    return false;

                _currentSlipEffect = slipEffect;
                _slipVelocity = rb.linearVelocity;
                return true;
            }

            return false;
        }

        public void DisposeEffect(BaseEffect effect)
        {
            if (effect is StunEffect stunEffect && _currentStunEffect == stunEffect)
            {
                _stunBlocker.Dispose();
                _currentStunEffect = null;
            }

            if (effect is SlipEffect slipEffect && _currentSlipEffect == slipEffect)
            {
                _currentSlipEffect = null;
                _slipVelocity = Vector2.zero;
            }
        }
    }
}
