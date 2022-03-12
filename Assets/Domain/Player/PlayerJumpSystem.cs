using Domain.Common;
using Leopotam.EcsLite;
using UnityEngine;

namespace Domain.Player
{
    public sealed class PlayerJumpSystem : IEcsRunSystem
    {
        public void Run(EcsSystems systems)
        {
            bool hasJumped = Input.GetKeyDown(KeyCode.Space);
            var world = systems.GetWorld();

            foreach (int entity in world.Filter<PlayerComponent>().End())
            {
                ref PlayerComponent player = ref world.GetPool<PlayerComponent>().Get(entity);

                if (hasJumped)
                {
                    player.Player.Animator.SetBool("IsInAir", true);
                    player.Player.CharacterController.Move(Vector3.up * 2);
                    world.GetPool<InAirTag>().Add(entity);
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