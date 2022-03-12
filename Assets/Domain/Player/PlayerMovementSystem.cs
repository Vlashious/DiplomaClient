using Domain.Providers;
using Domain.Utils;
using Leopotam.EcsLite;
using Unity.Mathematics;
using UnityEngine;

namespace Domain.Player
{
    public sealed class PlayerSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly PrefabProvider _prefabProvider;
        private readonly ConfigProvider _configProvider;
        private readonly UtilCamera _utilCamera;

        private float _rotationVelocity;

        public PlayerSystem(PrefabProvider prefabProvider, ConfigProvider configProvider, UtilCamera utilCamera)
        {
            _prefabProvider = prefabProvider;
            _configProvider = configProvider;
            _utilCamera = utilCamera;
        }

        public void Init(EcsSystems systems)
        {
            PlayerProvider player = Object.Instantiate(_prefabProvider.Player);
            EcsWorld world = systems.GetWorld();
            int playerEntity = world.NewEntity();
            ref PlayerComponent playerComponent = ref world.GetPool<PlayerComponent>().Add(playerEntity);
            playerComponent = new PlayerComponent(player);
        }

        public void Run(EcsSystems systems)
        {
            EcsWorld world = systems.GetWorld();
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            float verticalInput = Input.GetAxisRaw("Vertical");
            var direction = new float3(horizontalInput, 0, verticalInput);
            direction = math.normalizesafe(direction);

            bool isMoving = math.lengthsq(direction) > 0.1;

            foreach (int entity in world.Filter<PlayerComponent>().End())
            {
                ref PlayerComponent player = ref world.GetPool<PlayerComponent>().Get(entity);
                player.Player.Animator.SetBool("IsMoving", isMoving);
            }

            if (!isMoving)
            {
                return;
            }

            foreach (int entity in world.Filter<PlayerComponent>().End())
            {
                ref PlayerComponent player = ref world.GetPool<PlayerComponent>().Get(entity);
                Transform transform = player.Player.Transform;
                float targetAngle = math.degrees(math.atan2(direction.x, direction.z)) + _utilCamera.Camera.transform.eulerAngles.y;

                transform.rotation = Quaternion.Euler(0, targetAngle, 0);
                float3 moveDirection = math.normalizesafe(Quaternion.Euler(0, targetAngle, 0) * Vector3.forward);
                player.Player.CharacterController.Move(moveDirection * _configProvider.PlayerSpeed * Time.deltaTime);
            }
        }
    }
}