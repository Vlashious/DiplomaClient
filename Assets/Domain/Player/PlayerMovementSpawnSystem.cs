using Leopotam.EcsLite;
using Unity.Mathematics;
using UnityEngine;

namespace Domain.Player
{
    public sealed class PlayerMovementSpawnSystem : IEcsRunSystem
    {
        public void Run(EcsSystems systems)
        {
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            float verticalInput = Input.GetAxisRaw("Vertical");
            var direction = new float3(horizontalInput, 0, verticalInput);
            direction = math.normalizesafe(direction);

            if (math.lengthsq(direction) < 0.1)
            {
                return;
            }

            EcsWorld world = systems.GetWorld();

            foreach (int entity in world.Filter<PlayerComponent>().End())
            {
                ref PlayerMovementEvent movementEvent = ref world.GetPool<PlayerMovementEvent>().Add(entity);
                movementEvent = new PlayerMovementEvent(direction);
            }
        }
    }
}