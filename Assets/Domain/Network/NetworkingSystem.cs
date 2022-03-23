using System;
using System.Net.Http;
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
        private Guid _guid = Guid.NewGuid();
        private EcsWorld _world;

        public NetworkingSystem(PrefabProvider prefabProvider)
        {
            _prefabProvider = prefabProvider;
        }

        public async void Init(EcsSystems systems)
        {
            _world = systems.GetWorld();

            _connection = new HubConnectionBuilder()
                         .WithUrl("http://localhost:5176/transformHub", options =>
                          {
                              options.HttpMessageHandlerFactory = (msg) =>
                              {
                                  if (msg is HttpClientHandler handler)
                                  {
                                      handler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;
                                  }

                                  return msg;
                              };
                          })
                         .Build();

            _connection.On<string, byte[]>("SendTransformUpdate", OnTransformUpdateReceived);
            _connection.On<string>("OnPlayerConnected", OnPlayerConnected);

            await _connection.StartAsync();
            await _connection.SendAsync("OnPlayerConnected", _guid.ToString());
            UniTask.RunOnThreadPool(Synchronize).Forget();
            Debug.Log("Connected to transform hub!");
        }

        private async UniTask Synchronize()
        {
            while (true)
            {
                await UniTask.Delay(TimeSpan.FromMilliseconds(30));

                foreach (var entity in _world.Filter<TransformComponent>().Inc<Synchronize>().End())
                {
                    var transform = _world.GetPool<TransformComponent>().Get(entity);
                    var position = transform.Transform.position;
                    transform.X = position.x;
                    transform.Y = position.y;
                    transform.Z = position.z;
                    var data = transform.Serialize();
                    await _connection.SendAsync("SendTransformUpdate", _guid.ToString(), data);
                }
            }
        }

        private void OnTransformUpdateReceived(string userId, byte[] data)
        {
            Debug.Log($"{userId}: {TransformComponent.Deserialize(data)}");

            foreach (int entity in _world.Filter<NetworkPlayer>().Inc<TransformComponent>().End())
            {
                ref var transform = ref _world.GetPool<TransformComponent>().Get(entity);
                ref var player = ref _world.GetPool<NetworkPlayer>().Get(entity);

                if (player.Id == userId)
                {
                    var deserialized = TransformComponent.Deserialize(data);
                    transform.Transform.position = new float3(deserialized.X, deserialized.Y, deserialized.Z);
                }
            }
        }

        private void OnPlayerConnected(string userId)
        {
            var networkPlayer = Object.Instantiate(_prefabProvider.NetworkPlayer);
            var playerEntity = _world.NewEntity();
            ref var player = ref _world.GetPool<NetworkPlayer>().Add(playerEntity);
            player.Id = userId;
            ref var transform = ref _world.GetPool<TransformComponent>().Add(playerEntity);
            transform.Transform = networkPlayer.Transform;
        }

        public async void Dispose()
        {
            await _connection.DisposeAsync();
        }
    }
}