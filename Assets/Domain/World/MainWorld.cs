using System;
using Domain.Classes.Mage;
using Domain.Enemy.Whale;
using Domain.Health;
using Domain.Network;
using Domain.Player;
using Domain.Projectile;
using Domain.Selection;
using Domain.Shared;
using Leopotam.EcsLite;
using VContainer;
using VContainer.Unity;

namespace Domain.World
{
    public sealed class MainWorld : IInitializable, ITickable, IDisposable
    {
        private readonly IObjectResolver _resolver;
        private EcsWorld _ecsWorld;
        private EcsSystems _ecsSystems;

        public MainWorld(IObjectResolver resolver)
        {
            _resolver = resolver;
        }

        public void Initialize()
        {
            _ecsWorld = new EcsWorld();
            _ecsSystems = new EcsSystems(_ecsWorld);

            _ecsSystems
               .Add(_resolver.Resolve<PlayerSystem>())
               .Add(_resolver.Resolve<PlayerSpawnSystem>())
               .Add(_resolver.Resolve<MageSystem>())
               .Add(_resolver.Resolve<MageBombSystem>())
               .Add(_resolver.Resolve<SelectionSystem>())
               .Add(_resolver.Resolve<SelectionViewSystem>())
               .Add(_resolver.Resolve<WhaleSpawnSystem>())
               .Add(_resolver.Resolve<ProjectileMoveSystem>())
               .Add(_resolver.Resolve<HealthSystem>())
               .Add(_resolver.Resolve<CreatureInspectorSystem>())
               .Add(_resolver.Resolve<NetworkingSystem>())
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