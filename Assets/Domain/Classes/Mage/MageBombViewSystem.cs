using Leopotam.EcsLite;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Domain.Classes.Mage
{
    public class MageBombSystem : IEcsRunSystem
    {
        public void Run(EcsSystems systems)
        {
            var world = systems.GetWorld();

            foreach (int entity in world.Filter<MageBomb>().End())
            {
                ref var bomb = ref world.GetPool<MageBomb>().Get(entity);

                bomb.Duration -= Time.deltaTime;
                bomb.EffectProvider.Duration.SetText(bomb.Duration.ToString());
                bomb.EffectProvider.Image.fillAmount = bomb.Duration / bomb.MaxDuration;
                bomb.EffectProvider.Name.SetText("Bomb");

                if (bomb.Duration <= 0)
                {
                    Object.Destroy(bomb.EffectProvider.gameObject);
                    world.DelEntity(entity);
                }
            }
        }
    }
}