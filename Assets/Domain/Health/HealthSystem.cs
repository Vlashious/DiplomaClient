using Domain.Shared;
using Leopotam.EcsLite;
using UnityEngine;

namespace Domain.Health
{
    public class HealthSystem : IEcsRunSystem
    {
        public void Run(EcsSystems systems)
        {
            EcsWorld world = systems.GetWorld();

            foreach (int entity in world.Filter<HealthComponent>().Inc<TransformComponent>().End())
            {
                var health = world.GetPool<HealthComponent>().Get(entity);
                var go = world.GetPool<TransformComponent>().Get(entity);

                if (health.Health <= 0)
                {
                    Object.Destroy(go.Transform.gameObject);
                    world.DelEntity(entity);
                }
            }
        }
    }
}