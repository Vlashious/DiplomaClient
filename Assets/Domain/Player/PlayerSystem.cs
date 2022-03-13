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

        private EcsWorld _world;
        private PlayerProvider _player;

        public PlayerSystem(PrefabProvider prefabProvider, ConfigProvider configProvider)
        {
            _prefabProvider = prefabProvider;
            _configProvider = configProvider;
        }

        public void Init(EcsSystems systems)
        {
            _player = Object.Instantiate(_prefabProvider.Player);
            _world = systems.GetWorld();
            var playerEntity = _world.NewEntity();
            ref PlayerComponent playerComponent = ref _world.GetPool<PlayerComponent>().Add(playerEntity);
            playerComponent = new PlayerComponent(_player);
        }

        public void Run(EcsSystems systems)
        {
            CheckRotate();
            CheckMove();
        }

        private void CheckRotate()
        {
            if (_player.PlayerInput.Player.Look.WasPerformedThisFrame())
            {
                Vector2 rotationEvent = _player.PlayerInput.Player.Look.ReadValue<Vector2>();
                _player.Transform.Rotate(0, rotationEvent.x, 0);
            }
        }

        private void CheckMove()
        {
            Vector2 movementEvent = _player.PlayerInput.Player.Move.ReadValue<Vector2>();

            bool isMoving = math.lengthsq(movementEvent) > 0.1f;

            _player.Animator.SetBool("IsMoving", isMoving);

            if (!isMoving)
            {
                return;
            }

            float3 moveDirection =
                _player.Transform.TransformDirection(math.normalizesafe(new float3(movementEvent.x, 0, movementEvent.y)));
            _player.CharacterController.Move(moveDirection * _configProvider.PlayerSpeed * Time.deltaTime);
            Debug.Log(_player.Transform.position);
        }
    }
}