using System.Collections.Generic;
using System.Linq;
using VContainer.Unity;

namespace EffectSystem
{
    public class EffectBank : ITickable
    {
        private readonly List<BaseEffect> _effects = new();
        
        public IReadOnlyList<BaseEffect> Effects => _effects;

        public void Tick()
        {
            if(_effects.Count == 0)
                return;
            
            _effects.ToList().ForEach(x => x.Tick());
        }

        private void OnEffectEnded(BaseEffect effect) => _effects.Remove(effect);

        public void RegisterEffect(BaseEffect effect)
        {
            if(_effects.Contains(effect))
                return;
            
            if(!effect.Target.ApplyEffect(effect))
                return;
            
            _effects.Add(effect);
            effect.OnEffectEnded += OnEffectEnded;
        }

        public void ForceRemoveEffect(BaseEffect effect)
        {
            if (!_effects.Contains(effect))
                return;
            
            effect.ForceDispose();
        }
    }
}