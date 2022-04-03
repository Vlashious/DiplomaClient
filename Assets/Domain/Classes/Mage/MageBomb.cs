using Domain.UI;
using UnityEngine;

namespace Domain.Classes.Mage
{
    public struct MageBomb
    {
        public readonly int EntityId;
        public float Duration;
        public float MaxDuration;
        public UIEffectProvider EffectProvider;
        public GameObject Visuals;

        public MageBomb(float duration, int entityId, UIEffectProvider effectProvider, GameObject visuals)
        {
            Duration = MaxDuration = duration;
            EntityId = entityId;
            EffectProvider = effectProvider;
            Visuals = visuals;
        }
    }
}