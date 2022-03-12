using System;
using Domain.Player;
using Domain.Providers;
using Domain.Utils;
using Leopotam.EcsLite;
using VContainer.Unity;

namespace Domain.World
{
    public sealed class MainWorld : IInitializable, ITickable, IDisposable
    {
        private readonly PrefabProvider _prefabProvider;
        private readonly ConfigProvider _configProvider;
        private readonly UtilCamera _utilCamera;
        private EcsWorld _ecsWorld;
        private EcsSystems _ecsSystems;

        public MainWorld(PrefabProvider prefabProvider, ConfigProvider configProvider, UtilCamera utilCamera)
        {
            _prefabProvider = prefabProvider;
            _configProvider = configProvider;
            _utilCamera = utilCamera;
        }

        public void Initialize()
        {
            _ecsWorld = new EcsWorld();
            _ecsSystems = new EcsSystems(_ecsWorld);

            _ecsSystems.Add(new PlayerSystem(_prefabProvider, _configProvider, _utilCamera))
                       .Add(new CameraRotationSystem())
                       .Add(new PlayerJumpSystem())
                       .Add(new PlayerGravitySystem())
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