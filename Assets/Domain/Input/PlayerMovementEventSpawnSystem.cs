using Domain.Player;
using Leopotam.EcsLite;
using Unity.Mathematics;

namespace Domain.Input
{
    public sealed class PlayerMovementEventSpawnSystem : IEcsRunSystem
    {
        public void Run(EcsSystems systems)
        {
            var horizontalInput = UnityEngine.Input.GetAxis("Horizontal");
            var verticalInput = UnityEngine.Input.GetAxis("Vertical");
            float3 direction = new float3(horizontalInput, 0, verticalInput);

            if (math.lengthsq(direction) == 0)
            {
                return;
            }

            var world = systems.GetWorld();
            var movementEventPool = world.GetPool<MovementInputEvent>();
            var filter = world.Filter<PlayerTag>().Inc<TransformComponent>().End();

            foreach (int entity in filter)
            {
                ref MovementInputEvent movementEvent = ref movementEventPool.Add(entity);
                movementEvent = new MovementInputEvent(math.normalize(direction));
            }
        }
    }
}