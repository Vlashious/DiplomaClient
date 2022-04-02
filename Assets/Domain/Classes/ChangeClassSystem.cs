using Domain.Classes.Mage;
using Domain.Classes.Priest;
using Domain.Classes.Warrior;
using Domain.Player;
using Leopotam.EcsLite;

namespace Domain.Classes
{
    public class ChangeClassSystem : IEcsRunSystem
    {
        public void Run(EcsSystems systems)
        {
            var world = systems.GetWorld();

            foreach (int entity in world.Filter<ChangeClassEvent>().End())
            {
                var eventInfo = world.GetPool<ChangeClassEvent>().Get(entity);

                foreach (int e in world.Filter<MageTag>().End())
                {
                    world.GetPool<MageTag>().Del(e);
                }

                foreach (int e in world.Filter<PriestTag>().End())
                {
                    world.GetPool<PriestTag>().Del(e);
                }

                foreach (int e in world.Filter<WarriorTag>().End())
                {
                    world.GetPool<WarriorTag>().Del(e);
                }

                foreach (int player in world.Filter<PlayerComponent>().End())
                {
                    var magePool = world.GetPool<MageTag>();
                    var priestPool = world.GetPool<PriestTag>();
                    var warriorPool = world.GetPool<WarriorTag>();
                    magePool.Del(player);
                    priestPool.Del(player);
                    warriorPool.Del(player);

                    switch (eventInfo.Class)
                    {
                        case ClassType.Mage:
                            magePool.Add(player);
                            break;
                        case ClassType.Priest:
                            priestPool.Add(player);
                            break;
                        case ClassType.Warrior:
                            warriorPool.Add(player);
                            break;
                    }
                }

                world.DelEntity(entity);
            }
        }
    }
}