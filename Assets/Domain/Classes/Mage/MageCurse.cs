using Domain.UI;

namespace Domain.Classes.Mage
{
    public struct MageCurse
    {
        public float Duration;
        public float MaxDuration;
        public UIEffectProvider EffectProvider;

        public MageCurse(float duration, float maxDuration, UIEffectProvider effectProvider)
        {
            Duration = duration;
            MaxDuration = maxDuration;
            EffectProvider = effectProvider;
        }
    }
}