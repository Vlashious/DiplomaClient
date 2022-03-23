using Domain.Classes.Mage;
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
    public sealed class PlayerSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly PrefabProvider _prefabProvider;
        private readonly ConfigProvider _configProvider;
        private readonly UtilCamera _camera;
        private readonly UIProvider _uiProvider;

        private EcsWorld _world;
        private PlayerProvider _player;

        public PlayerSystem(PrefabProvider prefabProvider, ConfigProvider configProvider, UtilCamera camera, UIProvider uiProvider)
        {
            _prefabProvider = prefabProvider;
            _configProvider = configProvider;
            _camera = camera;
            _uiProvider = uiProvider;
        }

        public void Init(EcsSystems systems)
        {
            _player = Object.Instantiate(_prefabProvider.Player);
            _world = systems.GetWorld();
            var playerEntity = _world.NewEntity();
            ref PlayerComponent playerComponent = ref _world.GetPool<PlayerComponent>().Add(playerEntity);
            playerComponent = new PlayerComponent(_player);
            ref TransformComponent transformComponent = ref _world.GetPool<TransformComponent>().Add(playerEntity);
            transformComponent.Transform = _player.Transform;
            _player.gameObject.AddComponent<PackedEntity>().Entity = _world.PackEntity(playerEntity);
            ref var playerHealth = ref _world.GetPool<HealthComponent>().Add(playerEntity);
            playerHealth = new HealthComponent(_configProvider.BasePlayerHealth);
            ref var playerName = ref _world.GetPool<NameComponent>().Add(playerEntity);
            playerName = new NameComponent("Me");
            _world.GetPool<MageTag>().Add(playerEntity);
            _world.GetPool<Synchronize>().Add(playerEntity);
        }

        public void Run(EcsSystems systems)
        {
            CheckMove();
            UpdatePlayerInspector();
        }

        private void CheckMove()
        {
            Vector2 movementInput = _player.PlayerInput.Player.Move.ReadValue<Vector2>();

            bool isMoving = math.lengthsq(movementInput) > 0.1f;

            _player.Animator.SetBool("IsMoving", isMoving);

            if (!isMoving)
            {
                return;
            }

            float3 moveDirection = _camera.transform.forward * movementInput.y + _camera.transform.right * movementInput.x;
            moveDirection.y = 0f;

            moveDirection = math.normalizesafe(moveDirection);
            _player.CharacterController.Move(moveDirection * _configProvider.PlayerSpeed * Time.deltaTime);

            _player.Transform.rotation = Quaternion.Euler(_player.Transform.rotation.eulerAngles.x,
                _camera.transform.rotation.eulerAngles.y, _player.Transform.rotation.eulerAngles.z);
        }

        private void UpdatePlayerInspector()
        {
            _uiProvider.PlayerInspectorProvider.Name.SetText("Player");

            foreach (int player in _world.Filter<PlayerComponent>().Inc<HealthComponent>().End())
            {
                var health = _world.GetPool<HealthComponent>().Get(player);
                _uiProvider.PlayerInspectorProvider.SetValue(health.Health, health.MaxHealth);
            }
        }
    }
}