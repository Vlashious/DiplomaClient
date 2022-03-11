using Leopotam.EcsLite;
using UnityEngine;

namespace Domain.MessageOnClickFeature
{
    public sealed class MessageOutputSystem : IEcsRunSystem
    {
        public void Run(EcsSystems systems)
        {
            var world = systems.GetWorld();
            var filter = world.Filter<MessageComponent>().End();
            var messages = world.GetPool<MessageComponent>();

            foreach (int entity in filter)
            {
                ref var msg = ref messages.Get(entity);
                Debug.Log(msg.Message);
                messages.Del(entity);
            }
        }
    }
}