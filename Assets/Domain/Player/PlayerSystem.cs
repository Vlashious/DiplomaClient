using Domain.Health;
using Domain.Providers;
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

        public PlayerSystem(ConfigProvider configProvider, UtilCamera camera, UIProvider uiProvider)
        {
            _configProvider = configProvider;
            _camera = camera;
            _uiProvider = uiProvider;
        }

        public void Run(EcsSystems systems)
        {
            var world = systems.GetWorld();

            foreach (int player in world.Filter<PlayerComponent>().End())
            {
                var playerProvider = world.GetPool<PlayerComponent>().Get(player);
                CheckMove(playerProvider.Player);
                UpdatePlayerInspector(world);
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

            player.Transform.rotation = Quaternion.Euler(player.Transform.rotation.eulerAngles.x,
                _camera.transform.rotation.eulerAngles.y, player.Transform.rotation.eulerAngles.z);
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