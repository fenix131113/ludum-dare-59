using System;
using System.Collections;
using EffectSystem;
using EffectSystem.Effects;
using UnityEngine;
using Utils;
using VContainer;

namespace ObstacleSystem
{
    public class StunZone : MonoBehaviour
    {
        [SerializeField] private LayerMask triggerLayers;
        [SerializeField] private float stunDuration;
        [SerializeField] private float stunCooldown;

        [Inject] private EffectBank _effectBank;

        private bool _isCooldown;

        public event Action OnStun;

        private void Start()
        {
            ObjectInjector.InjectObject(this);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!enabled || !LayerService.CheckLayersEquality(other.gameObject.layer, triggerLayers) ||
                !other.gameObject.TryGetComponent(out IEffectAddicted target))
                return;
            
            Stun(target);
        }

        private void Stun(IEffectAddicted target)
        {
            if(_isCooldown)
                return;
            
            _effectBank.RegisterEffect(new StunEffect(target, stunDuration));
            StartCoroutine(StunCoroutine());
            _isCooldown = true;
            OnStun?.Invoke();
        }

        private IEnumerator StunCoroutine()
        {
            yield return new WaitForSeconds(stunCooldown);
            
            _isCooldown = false;
        }
    }
}