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
            _connection.On<byte[]>("SpawnMageBomb", OnSpawnMageBomb);
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

                if (innerId == entity)
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

                if (innerId == entity)
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
            transform.Transform = networkPlayer.PlayerProvider.Transform;
            transform.Transform.position = position;

            _world.GetPool<CreatureInspector>().Add(playerEntity) = new CreatureInspector
            {
                CreatureInspectorProvider = networkPlayer.Inspector
            };
        }

        public void Run(EcsSystems systems)
        {
            var world = systems.GetWorld();

            foreach (var entity in world.Filter<NetworkPacket>().End())
            {
                var packet = world.GetPool<NetworkPacket>().Get(entity);
                _connection.SendAsync(packet.Method, packet.Data);
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

        private void OnSpawnMageBomb(byte[] data)
        {
            using var ms = new MemoryStream(data);
            using var rd = new BinaryReader(ms);
            var targetServerId = rd.ReadInt32();
            var duration = rd.ReadSingle();

            var inspectorPool = _world.GetPool<CreatureInspector>();
            var innerId = _synchronizeMap.GetInnerId(targetServerId);

            if (inspectorPool.Has(innerId))
            {
                var bomb = _world.NewEntity();
                var inspector = inspectorPool.Get(innerId);

                var effectProvider = Object.Instantiate(inspector.CreatureInspectorProvider.UIEffectProviderPrefab,
                    inspector.CreatureInspectorProvider.EffectsRoot);
                _world.GetPool<MageBomb>().Add(bomb) = new MageBomb(duration, targetServerId, effectProvider);
            }
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