using System;
using Domain.Player;
using Leopotam.EcsLite;

namespace Domain.Network.Commands
{
    public sealed class SpawnMainPlayerCommand : INetworkCommand
    {
        private readonly EcsWorld _world;
        private readonly Guid _id;

        public SpawnMainPlayerCommand(EcsWorld world, Guid id)
        {
            _world = world;
            _id = id;
        }

        public void Do()
        {
            var playerSpawnEvent = _world.NewEntity();
            _world.GetPool<SpawnPlayerEvent>().Add(playerSpawnEvent).SpawnWithId = _id;
        }
    }
}