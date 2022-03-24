using System;
using Cysharp.Threading.Tasks;
using Domain.Providers;
using Domain.Shared;
using Leopotam.EcsLite;
using Microsoft.AspNetCore.SignalR.Client;
using Unity.Mathematics;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Domain.Network
{
    public class NetworkingSystem : IEcsInitSystem, IDisposable
    {
        private readonly PrefabProvider _prefabProvider;
        private HubConnection _connection;
        private readonly Guid _guid = Guid.NewGuid();
        private EcsWorld _world;

        public NetworkingSystem(PrefabProvider prefabProvider)
        {
            _prefabProvider = prefabProvider;
        }

        public async void Init(EcsSystems systems)
        {
            _world = systems.GetWorld();

            _connection = new HubConnectionBuilder()
                         .WithUrl("http://localhost:5176/transformHub")
                         .Build();

            _connection.On<Guid[]>("RetreiveAllPlayers", OnAllPlayersRetreived);
            _connection.On<Guid, byte[]>("SendTransformUpdate", OnTransformUpdateReceived);
            _connection.On<Guid>("OnPlayerConnected", OnPlayerConnected);
            _connection.On<Guid>("OnPlayerDisconnected", OnPlayerDisconnected);

            await _connection.StartAsync();
            await _connection.SendAsync("RetreiveAllPlayers");

            await _connection.SendAsync("OnPlayerConnected", _guid);
            UniTask.RunOnThreadPool(Synchronize).Forget();
            Debug.Log("Connected to transform hub!");
        }

        private async UniTask Synchronize()
        {
            while (true)
            {
                await UniTask.Delay(TimeSpan.FromMilliseconds(50));

                foreach (var entity in _world.Filter<TransformComponent>().Inc<Synchronize>().End())
                {
                    var transform = _world.GetPool<TransformComponent>().Get(entity);
                    var data = transform.Serialize();
                    await _connection.SendAsync("SendTransformUpdate", _guid, data);
                }
            }
        }

        private void OnTransformUpdateReceived(Guid userId, byte[] data)
        {
            foreach (int entity in _world.Filter<NetworkPlayer>().Inc<TransformComponent>().End())
            {
                ref var transform = ref _world.GetPool<TransformComponent>().Get(entity);
                ref var player = ref _world.GetPool<NetworkPlayer>().Get(entity);

                if (player.Id == userId)
                {
                    (float3 pos, float3 rot) = TransformComponent.Deserialize(data);
                    transform.Transform.position = pos;
                    transform.Transform.rotation = Quaternion.Euler(rot.xyz);
                }
            }
        }

        private void OnAllPlayersRetreived(Guid[] allPlayers)
        {
            foreach (Guid player in allPlayers)
            {
                OnPlayerConnected(player);
            }
        }

        private void OnPlayerConnected(Guid userId)
        {
            var networkPlayer = Object.Instantiate(_prefabProvider.NetworkPlayer);
            var playerEntity = _world.NewEntity();
            ref var player = ref _world.GetPool<NetworkPlayer>().Add(playerEntity);
            player.Id = userId;
            ref var transform = ref _world.GetPool<TransformComponent>().Add(playerEntity);
            transform.Transform = networkPlayer.Transform;
        }

        private void OnPlayerDisconnected(Guid userId)
        {
            foreach (int entity in _world.Filter<NetworkPlayer>().Inc<TransformComponent>().End())
            {
                ref var transform = ref _world.GetPool<TransformComponent>().Get(entity);
                ref var player = ref _world.GetPool<NetworkPlayer>().Get(entity);

                if (player.Id == userId)
                {
                    Object.Destroy(transform.Transform.gameObject);
                    _world.DelEntity(entity);
                }
            }
        }

        public async void Dispose()
        {
            await _connection.DisposeAsync();
        }
    }
}