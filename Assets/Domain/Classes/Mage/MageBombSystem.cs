using System.Collections.Generic;
using Domain.Health;
using Domain.Shared;
using Domain.UI;
using Leopotam.EcsLite;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Domain.Classes.Mage
{
    public class MageBombSystem : IEcsRunSystem
    {
        private readonly Dictionary<int, UIEffectProvider> _effectProviders = new();

        public void Run(EcsSystems systems)
        {
            var world = systems.GetWorld();

            foreach (int entity in world.Filter<MageBomb>().Inc<CreatureInspector>().End())
            {
                ref var bomb = ref world.GetPool<MageBomb>().Get(entity);
                ref var inspector = ref world.GetPool<CreatureInspector>().Get(entity);

                if (!_effectProviders.ContainsKey(entity))
                {
                    _effectProviders[entity] = Object.Instantiate(inspector.CreatureInspectorProvider.UIEffectProviderPrefab,
                        inspector.CreatureInspectorProvider.EffectsRoot);
                }

                _effectProviders[entity].Name.SetText("Bomb");
                _effectProviders[entity].Duration.SetText($"{bomb.Duration:F1} s.");
                _effectProviders[entity].Image.fillAmount = bomb.Duration / bomb.MaxDuration;
            }

            foreach (int entity in world.Filter<CreatureInspector>().Exc<MageBomb>().End())
            {
                if (_effectProviders.ContainsKey(entity))
                {
                    Object.Destroy(_effectProviders[entity].gameObject);
                    _effectProviders.Remove(entity);
                }
            }

            foreach (int entity in world.Filter<MageBomb>().Inc<HealthComponent>().End())
            {
                ref var bomb = ref world.GetPool<MageBomb>().Get(entity);
                ref var health = ref world.GetPool<HealthComponent>().Get(entity);

                bomb.Duration -= Time.deltaTime;

                if (bomb.Duration < 0)
                {
                    health.Health -= bomb.Damage;
                    world.GetPool<MageBomb>().Del(entity);
                }
            }
        }
    }
}