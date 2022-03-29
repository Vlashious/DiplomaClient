using Domain.Network;
using Domain.Shared;
using Leopotam.EcsLite;
using UnityEngine;

namespace Domain.Health
{
    public class HealthSystem : IEcsRunSystem
    {
        private readonly SynchronizeMap _synchronizeMap;

        public HealthSystem(SynchronizeMap synchronizeMap)
        {
            _synchronizeMap = synchronizeMap;
        }

        public void Run(EcsSystems systems)
        {
            EcsWorld world = systems.GetWorld();

            foreach (int entity in world.Filter<HealthUpdateEvent>().End())
            {
                var healthUpdateEvent = world.GetPool<HealthUpdateEvent>().Get(entity);
                var innerId = _synchronizeMap.GetInnerId(healthUpdateEvent.EntityId);
                var transformPool = world.GetPool<TransformComponent>();
                var healthPool = world.GetPool<HealthComponent>();

                if (healthPool.Has(innerId))
                {
                    ref var health = ref healthPool.Get(innerId);
                    health.Health = healthUpdateEvent.NewHealth;
                }

                if (transformPool.Has(innerId))
                {
                    var go = world.GetPool<TransformComponent>().Get(innerId);

                    if (healthUpdateEvent.NewHealth <= 0)
                    {
                        Object.Destroy(go.Transform.gameObject);
                        world.DelEntity(entity);
                    }
                }
            }
        }
    }
}