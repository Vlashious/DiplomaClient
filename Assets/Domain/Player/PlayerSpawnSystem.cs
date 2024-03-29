﻿using System;
using Domain.Classes.Mage;
using Domain.Health;
using Domain.Network;
using Domain.Providers;
using Domain.Shared;
using Leopotam.EcsLite;
using Object = UnityEngine.Object;

namespace Domain.Player
{
    public class PlayerSpawnSystem : IEcsRunSystem
    {
        private readonly PrefabProvider _prefabProvider;
        private readonly ConfigProvider _configProvider;
        private readonly SynchronizeMap _synchronizeMap;

        public PlayerSpawnSystem(PrefabProvider prefabProvider, ConfigProvider configProvider, SynchronizeMap synchronizeMap)
        {
            _prefabProvider = prefabProvider;
            _configProvider = configProvider;
            _synchronizeMap = synchronizeMap;
        }

        public void Run(EcsSystems systems)
        {
            var world = systems.GetWorld();

            foreach (int spawnPlayer in world.Filter<SpawnPlayerEvent>().End())
            {
                var spawnInfo = world.GetPool<SpawnPlayerEvent>().Get(spawnPlayer);
                var player = Object.Instantiate(_prefabProvider.Player);
                var playerEntity = world.NewEntity();
                ref PlayerComponent playerComponent = ref world.GetPool<PlayerComponent>().Add(playerEntity);
                playerComponent = new PlayerComponent(player);
                ref TransformComponent transformComponent = ref world.GetPool<TransformComponent>().Add(playerEntity);
                transformComponent.Transform = player.Transform;
                player.gameObject.AddComponent<PackedEntity>().Entity = world.PackEntity(playerEntity);
                ref var playerHealth = ref world.GetPool<HealthComponent>().Add(playerEntity);
                playerHealth = new HealthComponent(spawnInfo.Health);
                ref var playerName = ref world.GetPool<NameComponent>().Add(playerEntity);
                playerName = new NameComponent("Me");
                world.GetPool<MageTag>().Add(playerEntity);
                _synchronizeMap.Add(playerEntity, spawnInfo.SpawnWithId);
                transformComponent.Transform.position = spawnInfo.Position;

                world.GetPool<SpawnPlayerEvent>().Del(spawnPlayer);
            }
        }
    }
}