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
        private readonly UtilCamera _camera;

        private EcsWorld _world;
        private PlayerProvider _player;

        public PlayerSystem(PrefabProvider prefabProvider, ConfigProvider configProvider, UtilCamera camera)
        {
            _prefabProvider = prefabProvider;
            _configProvider = configProvider;
            _camera = camera;
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
            CheckMove();
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
            Debug.Log(_player.Transform.position);
        }
    }
}