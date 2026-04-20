namespace EffectSystem.Effects
{
    public class SlipEffect : BaseEffect
    {
        public float Deceleration { get; }

        public SlipEffect(IEffectAddicted target, float duration, float deceleration) : base(target, duration)
        {
            Deceleration = deceleration;
        }
    }
}
