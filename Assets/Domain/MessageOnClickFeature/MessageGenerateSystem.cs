using Leopotam.EcsLite;
using UnityEngine;

namespace Domain.MessageOnClickFeature
{
    public sealed class MessageGenerateSystem : IEcsRunSystem
    {
        public void Run(EcsSystems systems)
        {
            if (Input.GetMouseButtonUp(0))
            {
                var world = systems.GetWorld();
                var entity = world.NewEntity();
                var pool = world.GetPool<MessageComponent>();
                ref var msg = ref pool.Add(entity);
                msg = new MessageComponent($"Hello world! {Input.mousePosition}");
            }
        }
    }
}