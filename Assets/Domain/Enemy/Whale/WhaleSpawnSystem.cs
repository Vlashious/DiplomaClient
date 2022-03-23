using Domain.Health;
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
        private readonly Transform[] _spawnPoints;

        public WhaleSpawnSystem(PrefabProvider prefabProvider, Transform[] spawnPoints)
        {
            _prefabProvider = prefabProvider;
            _spawnPoints = spawnPoints;
        }

        public void Init(EcsSystems systems)
        {
            var world = systems.GetWorld();

            foreach (Transform spawnPoint in _spawnPoints)
            {
                WhaleProvider whale = Object.Instantiate(_prefabProvider.Whale);
                whale.transform.position = spawnPoint.position;
                whale.transform.rotation = spawnPoint.rotation;
                var whaleEntity = world.NewEntity();
                whale.gameObject.AddComponent<PackedEntity>().Entity = world.PackEntity(whaleEntity);
                ref var health = ref world.GetPool<HealthComponent>().Add(whaleEntity);
                health = new HealthComponent(200);
                ref var transform = ref world.GetPool<TransformComponent>().Add(whaleEntity);
                transform.Transform = whale.transform;
                ref var name = ref world.GetPool<NameComponent>().Add(whaleEntity);
                name = new NameComponent("Whale");
                ref var inspector = ref world.GetPool<CreatureInspector>().Add(whaleEntity);
                inspector.CreatureInspectorProvider = whale.CreatureInspectorProvider;
            }
        }
    }
}