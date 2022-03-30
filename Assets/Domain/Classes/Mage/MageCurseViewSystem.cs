using Leopotam.EcsLite;

namespace Domain.Classes.Mage
{
    public class MageCurseViewSystem : IEcsRunSystem
    {
        public void Run(EcsSystems systems)
        {
            var world = systems.GetWorld();
        }
    }
}