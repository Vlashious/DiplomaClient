using Domain.Health;
using Domain.Shared;
using Leopotam.EcsLite;

namespace Domain.Damage
{
    public class DealDamageSystem : IEcsRunSystem
    {
        public void Run(EcsSystems systems)
        {
            var world = systems.GetWorld();

            foreach (int entity in world.Filter<DealDamageComponent>().Inc<HealthComponent>().End())
            {
                var damage = world.GetPool<DealDamageComponent>().Get(entity);
                ref var health = ref world.GetPool<HealthComponent>().Get(entity);
                health.Health -= damage.Damage;

                world.GetPool<DealDamageComponent>().Del(entity);
            }
        }
    }
}