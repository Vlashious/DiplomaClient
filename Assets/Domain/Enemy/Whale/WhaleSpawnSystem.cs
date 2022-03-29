using Domain.Health;
using Domain.Network;
using Domain.Providers;
using Domain.Shared;
using Leopotam.EcsLite;
using UnityEngine;

namespace Domain.Enemy.Whale
{
    public class WhaleSpawnSystem : IEcsRunSystem
    {
        private readonly PrefabProvider _prefabProvider;
        private readonly SynchronizeMap _synchronizeMap;

        public WhaleSpawnSystem(PrefabProvider prefabProvider, SynchronizeMap synchronizeMap)
        {
            _prefabProvider = prefabProvider;
            _synchronizeMap = synchronizeMap;
        }

        public void Run(EcsSystems systems)
        {
            var world = systems.GetWorld();

            foreach (var entity in world.Filter<SpawnWhaleEvent>().End())
            {
                var whaleSpawnData = world.GetPool<SpawnWhaleEvent>().Get(entity);
                WhaleProvider whale = Object.Instantiate(_prefabProvider.Whale);
                whale.transform.position = whaleSpawnData.Position;
                var whaleEntity = world.NewEntity();
                whale.gameObject.AddComponent<PackedEntity>().Entity = world.PackEntity(whaleEntity);
                ref var health = ref world.GetPool<HealthComponent>().Add(whaleEntity);
                health = new HealthComponent(whaleSpawnData.Health);
                ref var transform = ref world.GetPool<TransformComponent>().Add(whaleEntity);
                transform.Transform = whale.transform;
                ref var name = ref world.GetPool<NameComponent>().Add(whaleEntity);
                name = new NameComponent("Whale");
                ref var inspector = ref world.GetPool<CreatureInspector>().Add(whaleEntity);
                inspector.CreatureInspectorProvider = whale.CreatureInspectorProvider;
                _synchronizeMap.Add(whaleEntity, whaleSpawnData.Id);

                world.DelEntity(entity);
            }
        }
    }
}