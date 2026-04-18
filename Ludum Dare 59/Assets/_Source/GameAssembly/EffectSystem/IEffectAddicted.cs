namespace EffectSystem
{
    public interface IEffectAddicted
    {
        void ApplyEffect(BaseEffect effect);
        void DisposeEffect(BaseEffect effect);
    }
}