using System;
using UnityEngine;

namespace EffectSystem
{
    public abstract class BaseEffect
    {
        protected float _duration;
        protected readonly IEffectAddicted _target;

        public IEffectAddicted Target => _target;
        public bool IsActive => _duration > 0;

        public event Action<BaseEffect> OnEffectEnded;

        public BaseEffect(IEffectAddicted target, float duration)
        {
            _duration = duration;
            _target = target;
        }
        
        public void Tick()
        {
            _duration -= Time.deltaTime;

            if (_duration > 0)
                return;
            
            ForceDispose();
        }

        public void ForceDispose()
        {
            _target.DisposeEffect(this);
            OnEffectEnded?.Invoke(this);
            OnEffectEnded = null;
        }
    }
}
