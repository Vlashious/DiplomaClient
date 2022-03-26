using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Domain.Network.Commands;
using Domain.Player;
using Domain.Providers;
using Domain.Shared;
using Leopotam.EcsLite;
using Microsoft.AspNetCore.SignalR.Client;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Domain.Network
{
    public class NetworkingSystem : IEcsInitSystem, IEcsRunSystem, IEcsDestroySystem
    {
        private readonly Guid _thisId = Guid.NewGuid();
        private readonly Queue<INetworkCommand> _commands = new();

        private readonly PrefabProvider _prefabProvider;
        private HubConnection _connection;
        private EcsWorld _world;

        public NetworkingSystem(PrefabProvider prefabProvider)
        {
            _prefabProvider = prefabProvider;
        }

        public async void Init(EcsSystems systems)
        {
            _world = systems.GetWorld();

            _connection = new HubConnectionBuilder()
                         .WithUrl("http://localhost:5176/worldHub")
                         .Build();

            _connection.On<Guid, byte[]>("SendTransformUpdate", OnTransformUpdateReceived);
            _connection.On<Guid>("OtherPlayerConnect", OnOtherPlayerConnected);
            _connection.On<Guid, Guid[]>("ThisPlayerConnect", OnThisPlayerConnected);
            _connection.On<Guid>("OnPlayerDisconnected", OnPlayerDisconnected);

            await _connection.StartAsync();

            await _connection.SendAsync("ThisPlayerConnect", _thisId, Array.Empty<Guid>());
            Debug.Log("Connected as player!");
        }

        private void OnTransformUpdateReceived(Guid userId, byte[] data)
        {
            if (userId != _thisId)
            {
                _commands.Enqueue(new UpdatePositionCommand(_world, userId, data));
            }
        }

        private void OnOtherPlayerConnected(Guid userId)
        {
            var networkPlayer = Object.Instantiate(_prefabProvider.NetworkPlayer);
            var playerEntity = _world.NewEntity();
            ref var player = ref _world.GetPool<Synchronize>().Add(playerEntity);
            player.Id = userId;
            ref var transform = ref _world.GetPool<TransformComponent>().Add(playerEntity);
            transform.Transform = networkPlayer.Transform;
        }

        private void OnThisPlayerConnected(Guid userId, Guid[] otherPlayers)
        {
            _commands.Enqueue(new SpawnMainPlayerCommand(_world, userId));

            foreach (Guid otherPlayer in otherPlayers)
            {
                OnOtherPlayerConnected(otherPlayer);
            }
        }

        private void OnPlayerDisconnected(Guid userId)
        {
            foreach (int entity in _world.Filter<Synchronize>().Inc<TransformComponent>().End())
            {
                ref var transform = ref _world.GetPool<TransformComponent>().Get(entity);
                ref var player = ref _world.GetPool<Synchronize>().Get(entity);

                if (player.Id == userId)
                {
                    Object.Destroy(transform.Transform.gameObject);
                    _world.DelEntity(entity);
                }
            }
        }

        public async void Run(EcsSystems systems)
        {
            while (_commands.TryDequeue(out var command))
            {
                command.Do();
            }

            foreach (int syncEntity in _world.Filter<TransformComponent>().Inc<Synchronize>().Inc<PlayerComponent>().End())
            {
                var trasnform = _world.GetPool<TransformComponent>().Get(syncEntity);
                var id = _world.GetPool<Synchronize>().Get(syncEntity);

                await _connection.SendAsync("SendTransformUpdate", id.Id, trasnform.Serialize());
            }
        }

        public async void Destroy(EcsSystems systems)
        {
            await _connection.DisposeAsync();
        }
    }
}