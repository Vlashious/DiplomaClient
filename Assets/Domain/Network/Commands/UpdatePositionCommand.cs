using System;
using Domain.Shared;
using Leopotam.EcsLite;
using Unity.Mathematics;
using UnityEngine;

namespace Domain.Network.Commands
{
    public sealed class UpdatePositionCommand : INetworkCommand
    {
        private readonly EcsWorld _world;
        private readonly Guid _userId;
        private readonly byte[] _data;

        public UpdatePositionCommand(EcsWorld world, Guid userId, byte[] data)
        {
            _world = world;
            _userId = userId;
            _data = data;
        }

        public void Do()
        {
            foreach (int entity in _world.Filter<Synchronize>().Inc<TransformComponent>().End())
            {
                ref var transform = ref _world.GetPool<TransformComponent>().Get(entity);
                ref var id = ref _world.GetPool<Synchronize>().Get(entity);

                if (id.Id == _userId)
                {
                    (float3 pos, float3 rot) = TransformComponent.Deserialize(_data);
                    transform.Transform.position = pos;
                    transform.Transform.rotation = Quaternion.Euler(rot.xyz);
                }
            }
        }
    }
}