using Domain.Common;
using Domain.Providers;
using Leopotam.EcsLite;
using Unity.Mathematics;
using UnityEngine;

namespace Domain.Player
{
    public sealed class PlayerJumpSystem : IEcsRunSystem
    {
        private readonly ConfigProvider _configProvider;

        private float _yDirection;

        public PlayerJumpSystem(ConfigProvider configProvider)
        {
            _configProvider = configProvider;
        }

        public void Run(EcsSystems systems)
        {
            EcsWorld world = systems.GetWorld();

            foreach (int entity in world.Filter<PlayerComponent>().End())
            {
                float3 direction = new float3();
                ref PlayerComponent player = ref world.GetPool<PlayerComponent>().Get(entity);
                bool hasJumped = player.Player.PlayerInput.PlayerMovement.Jump.WasPressedThisFrame();

                if (!world.GetPool<InAirTag>().Has(entity) && hasJumped)
                {
                    player.Player.Animator.SetBool("IsInAir", true);
                    _yDirection = _configProvider.PlayerJumpHeight;
                    world.GetPool<InAirTag>().Add(entity);
                }

                if (world.GetPool<InAirTag>().Has(entity))
                {
                    _yDirection -= _configProvider.Gravity * Time.deltaTime;
                    direction.y = _yDirection;
                    player.Player.Animator.SetFloat("JumpDirection", _yDirection);
                    player.Player.CharacterController.Move(direction * _configProvider.PlayerSpeed * Time.deltaTime);
                }

                if (world.GetPool<InAirTag>().Has(entity) && player.Player.CharacterController.isGrounded)
                {
                    world.GetPool<InAirTag>().Del(entity);
                    player.Player.Animator.SetBool("IsInAir", false);
                }
            }
        }
    }
}