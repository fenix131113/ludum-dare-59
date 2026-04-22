using System;
using EffectSystem;
using EffectSystem.Effects;
using Player.Data;
using Player.Variables;
using UnityEngine;
using Utils;
using Utils.Extensions;
using Utils.VariablesSystem;
using VContainer;
using Random = UnityEngine.Random;

namespace Player
{
    public class PlayerMovement : MonoBehaviour, IEffectAddicted
    {
        private static readonly int _isMoving = Animator.StringToHash("IsMoving");
        private static readonly int _moveY = Animator.StringToHash("MoveY");
        private static readonly int _moveX = Animator.StringToHash("MoveX");
        private static readonly int _slip = Animator.StringToHash("Slip");
        private static readonly int _slipY = Animator.StringToHash("SlipY");
        private static readonly int _slipX = Animator.StringToHash("SlipX");
        private static readonly int _isSlipping = Animator.StringToHash("IsSlipping");
        private static readonly int _isStunned = Animator.StringToHash("IsStunned");

        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private Animator anim;
        [SerializeField] private ParticleSystem dustParticles;
        [SerializeField] private float dustMinInterval;
        [SerializeField] private float dustMaxInterval;
        [SerializeField] private AudioClip[] stepsSounds;
        [SerializeField] private AudioClip[] fallSounds;
        [SerializeField] private float minStepSoundInterval = 0.2f;
        [SerializeField] private float maxStepSoundInterval = 0.3f;
        [SerializeField] private float stepSoundsVolume = 0.7f;
        [SerializeField] private float fallSoundsVolume = 0.7f;

        [Inject] private PlayerInput _input;
        [Inject] private PlayerSettingsSO _playerSettings;
        [Inject] private IVariablesResolver<PlayerVariableBlockerType, Action, Action> _playerVariables;

        private readonly PlayerVariableBlocker _stunBlocker =
            new(PlayerVariableBlockerType.MOVEMENT, PlayerVariableBlockerType.SEND_SIGNAL);

        private StunEffect _currentStunEffect;
        private SlipEffect _currentSlipEffect;
        private Vector2 _slipVelocity;
        private bool _slipAnimTriggered;
        private float _currentDustInterval;
        private float _currentStepInterval;

        private void Start()
        {
            GenerateStepInterval();
            GenerateDustInterval();
        }

        private void Update()
        {
            if (_currentSlipEffect is { IsActive: true })
            {
                anim.SetBool(_isSlipping, true);
                if (!_slipAnimTriggered)
                {
                    _slipAnimTriggered = true;
                    anim.SetFloat(_slipX, _input.ReadMoveVector().x);
                    anim.SetFloat(_slipY, _input.ReadMoveVector().y);
                    anim.SetTrigger(_slip);
                }

                _slipVelocity = Vector2.MoveTowards(
                    _slipVelocity,
                    Vector2.zero,
                    _currentSlipEffect.Deceleration * Time.deltaTime);
                rb.linearVelocity = _slipVelocity;
                return;
            }

            anim.SetBool(_isSlipping, false);
            _slipAnimTriggered = false;

            if (_playerVariables.IsVariableBlocked(PlayerVariableBlockerType.MOVEMENT))
            {
                rb.linearVelocity = Vector2.zero;
                anim.SetBool(_isMoving, false);
                return;
            }

            rb.linearVelocity = _input.ReadMoveVector() * _playerSettings.MoveSpeed;
            anim.SetBool(_isMoving, _input.ReadMoveVector().magnitude > 0f);

            if (_currentDustInterval > 0 && _input.ReadMoveVector().magnitude > 0f)
                _currentDustInterval -= Time.deltaTime;
            else if (_input.ReadMoveVector().magnitude > 0f)
            {
                dustParticles.Emit(1);
                GenerateDustInterval();
            }

            if (_currentStepInterval > 0 && _input.ReadMoveVector().magnitude > 0f)
                _currentStepInterval -= Time.deltaTime;
            else if (_input.ReadMoveVector().magnitude > 0f)
            {
                AudioStarter.PlaySound(stepsSounds.GetRandomElement(), stepSoundsVolume);
                GenerateStepInterval();
            }

            anim.SetFloat(_moveX, _input.ReadMoveVector().x);
            anim.SetFloat(_moveY, _input.ReadMoveVector().y);
        }

        private void GenerateStepInterval() =>
            _currentStepInterval = Random.Range(minStepSoundInterval, maxStepSoundInterval);

        private void GenerateDustInterval() =>
            _currentDustInterval = Random.Range(dustMinInterval, dustMaxInterval);

        public bool ApplyEffect(BaseEffect effect)
        {
            if (effect is StunEffect stunEffect)
            {
                if (_currentStunEffect is { IsActive: true })
                    return false;

                _currentStunEffect = stunEffect;
                anim.SetFloat(_slipX, _input.ReadMoveVector().x);
                anim.SetFloat(_slipY, _input.ReadMoveVector().y);
                anim.SetTrigger(_slip);
                _playerVariables.RegisterBlocker(_stunBlocker);
                anim.SetBool(_isStunned, true);
                return true;
            }

            if (effect is SlipEffect slipEffect)
            {
                if (_currentSlipEffect is { IsActive: true } || rb.linearVelocity.sqrMagnitude <= Mathf.Epsilon)
                    return false;

                AudioStarter.PlaySound(fallSounds.GetRandomElement(), fallSoundsVolume);
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
                anim.SetBool(_isStunned, false);
            }

            if (effect is SlipEffect slipEffect && _currentSlipEffect == slipEffect)
            {
                _currentSlipEffect = null;
                _slipVelocity = Vector2.zero;
            }
        }
    }
}