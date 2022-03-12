using Leopotam.EcsLite;
using UnityEngine;

namespace Domain.Player
{
    public sealed class PlayerGravitySystem : IEcsRunSystem
    {
        private float _gravity;

        public void Run(EcsSystems systems)
        {
            EcsWorld world = systems.GetWorld();

            foreach (int entity in world.Filter<PlayerComponent>().End())
            {
                ref PlayerComponent player = ref world.GetPool<PlayerComponent>().Get(entity);
                _gravity = -9.81f * Time.deltaTime;

                if (player.Player.CharacterController.isGrounded)
                {
                    _gravity = 0;
                }

                player.Player.CharacterController.Move(Vector3.up * _gravity);
            }
        }
    }
}