using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using Domain.Player;
using Domain.Providers;
using Domain.Shared;
using Leopotam.EcsLite;
using Microsoft.AspNetCore.SignalR.Client;
using Unity.Mathematics;
using UnityEngine;

namespace Domain.Network
{
    public class NetworkingSystem : IEcsInitSystem, IEcsDestroySystem
    {
        private readonly PrefabProvider _prefabProvider;
        private readonly CancellationTokenSource _cancellationToken = new();
        private HubConnection _connection;
        private EcsWorld _world;
        private int _thisId;

        public NetworkingSystem(PrefabProvider prefabProvider)
        {
            _prefabProvider = prefabProvider;
        }

        public async void Init(EcsSystems systems)
        {
            _world = systems.GetWorld();

            _connection = new HubConnectionBuilder()
                         .WithUrl("http://localhost:5176/world")
                         .Build();

            _connection.On<byte[]>("SpawnPlayer", OnSpawnPlayer);
            _connection.On<byte[]>("SpawnNetworkPlayer", OnSpawnNetworkPlayer);
            _connection.On<byte[]>("UpdatePlayerPosition", UpdatePlayerPosition);
            _connection.On<byte[]>("DestroyPlayer", OnDestroyPlayer);

            await _connection.StartAsync();
            UniTask.RunOnThreadPool(SendPlayerData).Forget();
            Debug.Log("Connected as player!");
        }

        private async UniTask SendPlayerData()
        {
            while (!_cancellationToken.IsCancellationRequested)
            {
                await UniTask.Delay(10);

                foreach (int entity in _world.Filter<PlayerComponent>().Inc<Synchronize>().Inc<TransformComponent>().End())
                {
                    var id = _world.GetPool<Synchronize>().Get(entity).Id;
                    var pos = _world.GetPool<TransformComponent>().Get(entity);
                    using var stream = new MemoryStream();
                    using var writer = new BinaryWriter(stream);
                    writer.Write(id);
                    writer.Write(pos.Transform.position.x);
                    writer.Write(pos.Transform.position.y);
                    writer.Write(pos.Transform.position.z);
                    await _connection.SendAsync("SendPlayerData", stream.ToArray());
                }
            }
        }

        private void UpdatePlayerPosition(byte[] data)
        {
            using var stream = new MemoryStream(data);
            using var reader = new BinaryReader(stream);
            var id = reader.ReadInt32();
            var x = reader.ReadSingle();
            var y = reader.ReadSingle();
            var z = reader.ReadSingle();
            var position = new float3(x, y, z);

            foreach (int entity in _world.Filter<Synchronize>().Inc<TransformComponent>().End())
            {
                var entityId = _world.GetPool<Synchronize>().Get(entity);

                if (id != _thisId && entityId.Id == id)
                {
                    ref var transform = ref _world.GetPool<TransformComponent>().Get(entity);
                    transform.Transform.position = position;
                    break;
                }
            }
        }

        private void OnDestroyPlayer(byte[] data)
        {
            using var stream = new MemoryStream(data);
            using var reader = new BinaryReader(stream);
            var id = reader.ReadInt32();

            foreach (int entity in _world.Filter<Synchronize>().Inc<TransformComponent>().End())
            {
                var entityId = _world.GetPool<Synchronize>().Get(entity);

                if (entityId.Id == id)
                {
                    var transform = _world.GetPool<TransformComponent>().Get(entity);
                    Object.Destroy(transform.Transform.gameObject);
                    _world.DelEntity(entity);
                }
            }
        }

        private void OnSpawnPlayer(byte[] data)
        {
            using var stream = new MemoryStream(data);
            using var reader = new BinaryReader(stream);
            var id = reader.ReadInt32();
            var x = reader.ReadSingle();
            var y = reader.ReadSingle();
            var z = reader.ReadSingle();
            var position = new float3(x, y, z);
            var playerSpawnEvent = _world.NewEntity();
            _world.GetPool<SpawnPlayerEvent>().Add(playerSpawnEvent) = new SpawnPlayerEvent {Position = position, SpawnWithId = id};
            _thisId = id;
        }

        private void OnSpawnNetworkPlayer(byte[] data)
        {
            using var stream = new MemoryStream(data);
            using var reader = new BinaryReader(stream);
            var id = reader.ReadInt32();
            var x = reader.ReadSingle();
            var y = reader.ReadSingle();
            var z = reader.ReadSingle();
            var position = new float3(x, y, z);
            var networkPlayer = Object.Instantiate(_prefabProvider.NetworkPlayer);
            var playerEntity = _world.NewEntity();
            ref var player = ref _world.GetPool<Synchronize>().Add(playerEntity);
            player.Id = id;
            ref var transform = ref _world.GetPool<TransformComponent>().Add(playerEntity);
            transform.Transform = networkPlayer.Transform;
            transform.Transform.position = position;
        }

        public async void Destroy(EcsSystems systems)
        {
            _cancellationToken.Cancel();
            await _connection.DisposeAsync();
        }
    }
}