using System.Collections;
using EffectSystem;
using EffectSystem.Effects;
using UnityEngine;
using Utils;
using VContainer;

namespace ObstacleSystem
{
    public class SlipZone : MonoBehaviour
    {
        [SerializeField] private LayerMask triggerLayers;
        [SerializeField] private float slipDuration;
        [SerializeField] private float slipDeceleration;
        [SerializeField] private float slipCooldown;

        [Inject] private EffectBank _effectBank;

        private bool _isCooldown;

        private void Start()
        {
            ObjectInjector.InjectObject(this);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!enabled || !LayerService.CheckLayersEquality(other.gameObject.layer, triggerLayers) ||
                !other.gameObject.TryGetComponent(out IEffectAddicted target))
                return;
            
            Slip(target);
        }

        private void Slip(IEffectAddicted target)
        {
            if(_isCooldown)
                return;
            
            _effectBank.RegisterEffect(new SlipEffect(target, slipDuration, slipDeceleration));
            StartCoroutine(SlipCoroutine());
            _isCooldown = true;
        }

        private IEnumerator SlipCoroutine()
        {
            yield return new WaitForSeconds(slipCooldown);
            
            _isCooldown = false;
        }
    }
}
