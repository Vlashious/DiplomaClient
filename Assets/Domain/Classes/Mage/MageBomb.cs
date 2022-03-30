using Domain.UI;

namespace Domain.Classes.Mage
{
    public struct MageBomb
    {
        public readonly int EntityId;
        public float Duration;
        public float MaxDuration;
        public UIEffectProvider EffectProvider;

        public MageBomb(float duration, int entityId, UIEffectProvider effectProvider)
        {
            Duration = MaxDuration = duration;
            EntityId = entityId;
            EffectProvider = effectProvider;
        }
    }
}