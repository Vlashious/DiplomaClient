using Domain.Health;
using Domain.Network;
using Domain.Providers;
using Domain.Shared;
using Domain.UI;
using Domain.Utils;
using Leopotam.EcsLite;
using Unity.Mathematics;
using UnityEngine;

namespace Domain.Player
{
    public sealed class PlayerSystem : IEcsRunSystem
    {
        private readonly ConfigProvider _configProvider;
        private readonly UtilCamera _camera;
        private readonly UIProvider _uiProvider;
        private readonly SynchronizeMap _synchronizeMap;
        private EcsWorld _world;
        private int _id;

        public PlayerSystem(ConfigProvider configProvider, UtilCamera camera, UIProvider uiProvider, SynchronizeMap synchronizeMap)
        {
            _configProvider = configProvider;
            _camera = camera;
            _uiProvider = uiProvider;
            _synchronizeMap = synchronizeMap;
        }

        public void Run(EcsSystems systems)
        {
            _world = systems.GetWorld();

            foreach (int player in _world.Filter<PlayerComponent>().End())
            {
                _id = _synchronizeMap[player];
                var playerProvider = _world.GetPool<PlayerComponent>().Get(player);
                CheckMove(playerProvider.Player);
                UpdatePlayerInspector(_world);
            }
        }

        private void CheckMove(PlayerProvider player)
        {
            Vector2 movementInput = player.PlayerInput.Player.Move.ReadValue<Vector2>();

            bool isMoving = math.lengthsq(movementInput) > 0.1f;

            player.Animator.SetBool("IsMoving", isMoving);

            if (!isMoving)
            {
                return;
            }

            float3 moveDirection = _camera.transform.forward * movementInput.y + _camera.transform.right * movementInput.x;
            moveDirection.y = 0f;

            moveDirection = math.normalizesafe(moveDirection);
            player.CharacterController.Move(moveDirection * _configProvider.PlayerSpeed * Time.deltaTime);

            var rotation = player.Transform.rotation;

            rotation = Quaternion.Euler(rotation.eulerAngles.x,
                _camera.transform.rotation.eulerAngles.y, rotation.eulerAngles.z);
            player.Transform.rotation = rotation;

            var updateEvent = _world.NewEntity();

            _world.GetPool<EntityPositionChangedEvent>().Add(updateEvent) =
                new EntityPositionChangedEvent(player.Transform.position, rotation.eulerAngles, _id);
        }

        private void UpdatePlayerInspector(EcsWorld world)
        {
            _uiProvider.PlayerInspectorProvider.Name.SetText("Player");

            foreach (int player in world.Filter<PlayerComponent>().Inc<HealthComponent>().End())
            {
                var health = world.GetPool<HealthComponent>().Get(player);
                _uiProvider.PlayerInspectorProvider.SetValue(health.Health, health.MaxHealth);
            }
        }
    }
}