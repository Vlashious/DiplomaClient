using System;
using Domain.Classes;
using Domain.Classes.Mage;
using Domain.Classes.Priest;
using Domain.Enemy.Whale;
using Domain.Health;
using Domain.Network;
using Domain.Player;
using Domain.Projectile;
using Domain.Shared;
using Leopotam.EcsLite;
using VContainer;
using VContainer.Unity;

namespace Domain.World
{
    public sealed class MainWorld : IInitializable, ITickable, IDisposable
    {
        private readonly IObjectResolver _resolver;
        public EcsWorld World { get; private set; }
        private EcsSystems _ecsSystems;

        public MainWorld(IObjectResolver resolver)
        {
            _resolver = resolver;
        }

        public void Initialize()
        {
            World = new EcsWorld();
            _ecsSystems = new EcsSystems(World);

            _ecsSystems
               .Add(_resolver.Resolve<PlayerSystem>())
               .Add(_resolver.Resolve<PlayerSpawnSystem>())
               .Add(_resolver.Resolve<MageSystem>())
               .Add(_resolver.Resolve<MageBombSystem>())
               .Add(_resolver.Resolve<PriestSystem>())
               .Add(_resolver.Resolve<WhaleSpawnSystem>())
               .Add(_resolver.Resolve<ProjectileMoveSystem>())
               .Add(_resolver.Resolve<HealthSystem>())
               .Add(_resolver.Resolve<CreatureInspectorSystem>())
               .Add(_resolver.Resolve<NetworkingSystem>())
               .Add(_resolver.Resolve<ChangeClassSystem>())
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

            if (World is not null)
            {
                World.Destroy();
                World = null;
            }
        }
    }
}