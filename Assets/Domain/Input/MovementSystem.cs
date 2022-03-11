using Domain.Providers;
using Leopotam.EcsLite;
using UnityEngine;

namespace Domain.Input
{
    public sealed class MovementSystem : IEcsRunSystem
    {
        private readonly ConfigProvider _configProvider;

        public MovementSystem(ConfigProvider configProvider)
        {
            _configProvider = configProvider;
        }

        public void Run(EcsSystems systems)
        {
            EcsWorld world = systems.GetWorld();
            MovePlayer(world);
        }

        private void MovePlayer(EcsWorld world)
        {
            EcsFilter filter = world.Filter<MovementInputEvent>().Inc<TransformComponent>().End();
            EcsPool<MovementInputEvent> movementPool = world.GetPool<MovementInputEvent>();
            EcsPool<TransformComponent> transformPool = world.GetPool<TransformComponent>();

            foreach (int entity in filter)
            {
                ref var movement = ref movementPool.Get(entity);
                ref var transform = ref transformPool.Get(entity);

                transform.Transform.Translate(movement.Direction * _configProvider.PlayerSpeed * Time.deltaTime);
                movementPool.Del(entity);
            }
        }
    }
}