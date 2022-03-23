using Leopotam.EcsLite;

namespace Domain.Classes.Mage
{
    public class MageCurseSystem : IEcsRunSystem
    {
        public void Run(EcsSystems systems)
        {
            var world = systems.GetWorld();
        }
    }
}