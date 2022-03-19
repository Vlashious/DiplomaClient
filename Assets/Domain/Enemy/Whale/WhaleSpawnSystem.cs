using Domain.Providers;
using Domain.Shared;
using Domain.Utils;
using Leopotam.EcsLite;
using UnityEngine;

namespace Domain.Enemy.Whale
{
    public class WhaleSpawnSystem : IEcsInitSystem
    {
        private readonly PrefabProvider _prefabProvider;

        public WhaleSpawnSystem(PrefabProvider prefabProvider)
        {
            _prefabProvider = prefabProvider;
        }

        public void Init(EcsSystems systems)
        {
            var world = systems.GetWorld();
            GameObject whale = Object.Instantiate(_prefabProvider.Whale);
            var whaleEntity = world.NewEntity();
            whale.AddComponent<PackedEntity>().Entity = world.PackEntity(whaleEntity);
            ref var health = ref world.GetPool<HealthComponent>().Add(whaleEntity);
            health = new HealthComponent(20);
            ref var transform = ref world.GetPool<TransformComponent>().Add(whaleEntity);
            transform.Transform = whale.transform;
            ref var name = ref world.GetPool<NameComponent>().Add(whaleEntity);
            name = new NameComponent("Whale");
        }
    }
}