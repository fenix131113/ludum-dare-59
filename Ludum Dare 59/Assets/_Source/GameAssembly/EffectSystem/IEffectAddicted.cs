namespace EffectSystem
{
    public interface IEffectAddicted
    {
        bool ApplyEffect(BaseEffect effect);
        void DisposeEffect(BaseEffect effect);
    }
}