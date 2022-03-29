using System.IO;
using System.Threading;
using Domain.Classes.Mage;
using Domain.Enemy.Whale;
using Domain.Health;
using Domain.Player;
using Domain.Providers;
using Domain.Shared;
using Leopotam.EcsLite;
using Microsoft.AspNetCore.SignalR.Client;
using Unity.Mathematics;
using UnityEngine;

namespace Domain.Network
{
    public class NetworkingSystem : IEcsInitSystem, IEcsRunSystem, IEcsDestroySystem
    {
        private readonly PrefabProvider _prefabProvider;
        private readonly SynchronizeMap _synchronizeMap;
        private readonly CancellationTokenSource _cancellationToken = new();
        private HubConnection _connection;
        private EcsWorld _world;

        public NetworkingSystem(PrefabProvider prefabProvider, SynchronizeMap synchronizeMap)
        {
            _prefabProvider = prefabProvider;
            _synchronizeMap = synchronizeMap;
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
            _connection.On<byte[]>("SpawnMageProjectile", OnSpawnMageProjectile);
            _connection.On<byte[]>("SpawnWhale", OnSpawnWhale);
            _connection.On<byte[]>("UpdateHealth", OnUpdateHealth);

            await _connection.StartAsync();
            Debug.Log("Connected as player!");
        }

        private void UpdatePlayerPosition(byte[] data)
        {
            using var stream = new MemoryStream(data);
            using var reader = new BinaryReader(stream);
            var serverId = reader.ReadInt32();
            var x = reader.ReadSingle();
            var y = reader.ReadSingle();
            var z = reader.ReadSingle();
            var rx = reader.ReadSingle();
            var ry = reader.ReadSingle();
            var rz = reader.ReadSingle();
            var position = new float3(x, y, z);
            var rotation = new float3(rx, ry, rz);

            foreach (int entity in _world.Filter<TransformComponent>().End())
            {
                var innerId = _synchronizeMap.GetInnerId(serverId);

                if (innerId == serverId)
                {
                    ref var transform = ref _world.GetPool<TransformComponent>().Get(innerId);
                    transform.Transform.position = position;
                    transform.Transform.rotation = Quaternion.Euler(rotation);
                    break;
                }
            }
        }

        private void OnDestroyPlayer(byte[] data)
        {
            using var stream = new MemoryStream(data);
            using var reader = new BinaryReader(stream);
            var serverId = reader.ReadInt32();

            foreach (int entity in _world.Filter<TransformComponent>().End())
            {
                var innerId = _synchronizeMap.GetInnerId(serverId);

                if (innerId == serverId)
                {
                    var transform = _world.GetPool<TransformComponent>().Get(innerId);
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
        }

        private void OnSpawnNetworkPlayer(byte[] data)
        {
            using var stream = new MemoryStream(data);
            using var reader = new BinaryReader(stream);
            var serverId = reader.ReadInt32();
            var x = reader.ReadSingle();
            var y = reader.ReadSingle();
            var z = reader.ReadSingle();
            var position = new float3(x, y, z);
            var networkPlayer = Object.Instantiate(_prefabProvider.NetworkPlayer);
            var playerEntity = _world.NewEntity();
            _synchronizeMap.Add(playerEntity, serverId);
            ref var transform = ref _world.GetPool<TransformComponent>().Add(playerEntity);
            transform.Transform = networkPlayer.Transform;
            transform.Transform.position = position;
        }

        public void Run(EcsSystems systems)
        {
            var world = systems.GetWorld();

            foreach (int entity in world.Filter<MageFirstAbilitySpawnEvent>().End())
            {
                var projectile = world.GetPool<MageFirstAbilitySpawnEvent>().Get(entity);
                using var ms = new MemoryStream();
                using var wr = new BinaryWriter(ms);
                wr.Write(projectile.TargetId);
                wr.Write(projectile.SpawnPos.x);
                wr.Write(projectile.SpawnPos.y);
                wr.Write(projectile.SpawnPos.z);
                _connection.SendAsync("SpawnMageProjectile", ms.ToArray());
                world.DelEntity(entity);
            }

            foreach (int entity in world.Filter<EntityPositionChangedEvent>().End())
            {
                var data = world.GetPool<EntityPositionChangedEvent>().Get(entity);
                using var stream = new MemoryStream();
                using var writer = new BinaryWriter(stream);
                writer.Write(data.Id);
                writer.Write(data.NewPosition.x);
                writer.Write(data.NewPosition.y);
                writer.Write(data.NewPosition.z);
                writer.Write(data.NewRotation.x);
                writer.Write(data.NewRotation.y);
                writer.Write(data.NewRotation.z);
                _connection.SendAsync("SendPlayerData", stream.ToArray());
                world.DelEntity(entity);
            }
        }

        private void OnSpawnMageProjectile(byte[] data)
        {
            using var ms = new MemoryStream(data);
            using var rd = new BinaryReader(ms);
            var targetServerId = rd.ReadInt32();
            var startX = rd.ReadSingle();
            var startY = rd.ReadSingle();
            var startZ = rd.ReadSingle();
            var speed = rd.ReadSingle();

            var projectile = _world.NewEntity();
            var visuals = Object.Instantiate(_prefabProvider.Fireball, new float3(startX, startY, startZ), Quaternion.identity);
            _world.GetPool<Projectile.Projectile>().Add(projectile) = new Projectile.Projectile(targetServerId, speed);
            _world.GetPool<TransformComponent>().Add(projectile) = new TransformComponent(visuals.transform);
        }

        private void OnSpawnWhale(byte[] data)
        {
            using var ms = new MemoryStream(data);
            using var rd = new BinaryReader(ms);
            var id = rd.ReadInt32();
            var x = rd.ReadSingle();
            var y = rd.ReadSingle();
            var z = rd.ReadSingle();
            var health = rd.ReadInt32();
            var spawnEvent = _world.NewEntity();
            _world.GetPool<SpawnWhaleEvent>().Add(spawnEvent) = new SpawnWhaleEvent(new float3(x, y, z), id, health);
        }

        private void OnUpdateHealth(byte[] data)
        {
            using var ms = new MemoryStream(data);
            using var br = new BinaryReader(ms);
            var serverId = br.ReadInt32();
            var newHealthValue = br.ReadInt32();
            var healthUpdate = _world.NewEntity();
            _world.GetPool<HealthUpdateEvent>().Add(healthUpdate) = new HealthUpdateEvent(serverId, newHealthValue);
        }

        public async void Destroy(EcsSystems systems)
        {
            _cancellationToken.Cancel();
            await _connection.DisposeAsync();
        }
    }
}