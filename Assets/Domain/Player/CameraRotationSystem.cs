using Leopotam.EcsLite;

namespace Domain.Player
{
    public sealed class CameraRotationSystem : IEcsRunSystem
    {
        public void Run(EcsSystems systems)
        {
            var world = systems.GetWorld();

            foreach (int entity in world.Filter<PlayerComponent>().End())
            {
                ref var player = ref world.GetPool<PlayerComponent>().Get(entity);

                // rotate camera while holding rmb or swiping for mobile
            }
        }
    }
}