using Domain.Providers;
using Leopotam.EcsLite;
using Unity.Mathematics;
using UnityEngine;

namespace Domain.Player
{
    public sealed class PlayerSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly PrefabProvider _prefabProvider;
        private readonly ConfigProvider _configProvider;

        private float _rotationVelocity;

        public PlayerSystem(PrefabProvider prefabProvider, ConfigProvider configProvider)
        {
            _prefabProvider = prefabProvider;
            _configProvider = configProvider;
        }

        public void Init(EcsSystems systems)
        {
            PlayerProvider player = Object.Instantiate(_prefabProvider.Player);
            EcsWorld world = systems.GetWorld();
            ref PlayerComponent playerComponent = ref world.GetPool<PlayerComponent>().Add(world.NewEntity());
            playerComponent = new PlayerComponent(player);
        }

        public void Run(EcsSystems systems)
        {
            EcsWorld world = systems.GetWorld();
            MovePlayer(world);
        }

        private void MovePlayer(EcsWorld world)
        {
            EcsFilter filter = world.Filter<PlayerComponent>()
                                    .Inc<PlayerMovementEvent>()
                                    .End();
            EcsPool<PlayerComponent> playerPool = world.GetPool<PlayerComponent>();
            EcsPool<PlayerMovementEvent> movementPool = world.GetPool<PlayerMovementEvent>();

            foreach (int entity in filter)
            {
                ref PlayerComponent player = ref playerPool.Get(entity);
                ref PlayerMovementEvent movement = ref movementPool.Get(entity);
                Transform transform = player.Player.Transform;
                float3 direction = movement.Direction;
                float targetAngle = math.degrees(math.atan2(direction.x, direction.z));

                transform.rotation = Quaternion.Euler(0, targetAngle, 0);
                player.Player.CharacterController.Move(movement.Direction * _configProvider.PlayerSpeed * Time.deltaTime);
                movementPool.Del(entity);
            }
        }
    }
}