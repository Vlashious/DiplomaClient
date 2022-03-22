using System;
using Domain.Health;
using Domain.Selection;
using Domain.UI;
using Leopotam.EcsLite;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Domain.Classes.Mage
{
    public class MageBombSystem : IEcsRunSystem
    {
        private readonly UIProvider _uiProvider;
        private UIEffectProvider _effectProvider;

        public MageBombSystem(UIProvider uiProvider)
        {
            _uiProvider = uiProvider;
        }

        public void Run(EcsSystems systems)
        {
            var world = systems.GetWorld();

            foreach (int entity in world.Filter<SelectedTag>().Inc<MageBomb>().End())
            {
                ref var bomb = ref world.GetPool<MageBomb>().Get(entity);

                if (world.GetPool<SelectedTag>().Has(entity))
                {
                    _effectProvider ??= Object.Instantiate(_uiProvider.CreatureInspectorProvider.UIEffectProviderPrefab,
                        _uiProvider.CreatureInspectorProvider.EffectsRoot);
                    _effectProvider.Name.SetText("Bomb");
                    _effectProvider.Duration.SetText($"{bomb.Duration:F1} s.");
                    _effectProvider.Image.fillAmount = bomb.Duration / bomb.MaxDuration;
                }
            }

            foreach (int entity in world.Filter<SelectedTag>().Exc<MageBomb>().End())
            {
                if (_effectProvider != null)
                {
                    Object.Destroy(_effectProvider.gameObject);
                    _effectProvider = null;
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