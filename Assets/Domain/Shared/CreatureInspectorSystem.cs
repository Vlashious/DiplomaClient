using Domain.Health;
using Domain.Utils;
using Leopotam.EcsLite;
using UnityEngine;

namespace Domain.Shared
{
    public class CreatureInspectorSystem : IEcsRunSystem
    {
        private readonly UtilCamera _camera;

        public CreatureInspectorSystem(UtilCamera camera)
        {
            _camera = camera;
        }

        public void Run(EcsSystems systems)
        {
            var world = systems.GetWorld();

            foreach (int entity in world.Filter<CreatureInspector>().Inc<HealthComponent>().Inc<NameComponent>().End())
            {
                ref var inspector = ref world.GetPool<CreatureInspector>().Get(entity);
                ref var health = ref world.GetPool<HealthComponent>().Get(entity);
                ref var name = ref world.GetPool<NameComponent>().Get(entity);

                inspector.CreatureInspectorProvider.Name.SetText(name.Name);
                inspector.CreatureInspectorProvider.SetValue(health.Health, health.MaxHealth);
            }

            foreach (int entity in world.Filter<CreatureInspector>().End())
            {
                ref var inspector = ref world.GetPool<CreatureInspector>().Get(entity);

                inspector.CreatureInspectorProvider.transform.LookAt(
                    inspector.CreatureInspectorProvider.transform.position + _camera.Camera.transform.forward);
            }
        }
    }
}