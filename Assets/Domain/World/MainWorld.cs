using System;
using Domain.Input;
using Domain.Player;
using Domain.Providers;
using Leopotam.EcsLite;
using VContainer.Unity;

namespace Domain.World
{
    public sealed class MainWorld : IInitializable, ITickable, IDisposable
    {
        private readonly PrefabProvider _prefabProvider;
        private readonly ConfigProvider _configProvider;
        private EcsWorld _ecsWorld;
        private EcsSystems _ecsSystems;

        public MainWorld(PrefabProvider prefabProvider, ConfigProvider configProvider)
        {
            _prefabProvider = prefabProvider;
            _configProvider = configProvider;
        }

        public void Initialize()
        {
            _ecsWorld = new EcsWorld();
            _ecsSystems = new EcsSystems(_ecsWorld);

            _ecsSystems.Add(new PlayerSpawnSystem(_prefabProvider))
                       .Add(new PlayerMovementEventSpawnSystem())
                       .Add(new MovementSystem(_configProvider))
                       .Init();
        }

        public void Tick()
        {
            _ecsSystems?.Run();
        }

        public void Dispose()
        {
            if (_ecsSystems is not null)
            {
                _ecsSystems.Destroy();
                _ecsSystems = null;
            }

            if (_ecsWorld is not null)
            {
                _ecsWorld.Destroy();
                _ecsWorld = null;
            }
        }
    }
}