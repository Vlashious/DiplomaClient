using Domain.Input;
using Domain.Providers;
using Leopotam.EcsLite;
using UnityEngine;

namespace Domain.Player
{
    public sealed class PlayerSpawnSystem : IEcsInitSystem
    {
        private readonly PrefabProvider _prefabProvider;

        public PlayerSpawnSystem(PrefabProvider prefabProvider)
        {
            _prefabProvider = prefabProvider;
        }

        public void Init(EcsSystems systems)
        {
            PlayerProvider player = Object.Instantiate(_prefabProvider.Player);
            EcsWorld world = systems.GetWorld();
            int entity = world.NewEntity();
            world.GetPool<PlayerTag>().Add(entity);
            ref TransformComponent transformComponent = ref world.GetPool<TransformComponent>().Add(entity);
            transformComponent = new TransformComponent(player.Transform);
        }
    }
}