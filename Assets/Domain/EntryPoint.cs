using Domain.Classes.Mage;
using Domain.Damage;
using Domain.Enemy.Whale;
using Domain.Health;
using Domain.Player;
using Domain.Providers;
using Domain.Selection;
using Domain.Shared;
using Domain.UI;
using Domain.Utils;
using Domain.World;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Domain
{
    public sealed class EntryPoint : LifetimeScope
    {
        [SerializeField]
        private Transform[] _whaleSpawnPoints;
        [SerializeField]
        private UtilCamera _utilCamera;
        [SerializeField]
        private UtilCanvas _utilCanvas;
        [SerializeField]
        private PrefabProvider _prefabProvider;
        [SerializeField]
        private UIProvider _uiProvider;
        [SerializeField]
        private ConfigProvider _configProvider;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(_utilCamera);
            builder.RegisterComponent(_utilCanvas);
            builder.RegisterComponent(_uiProvider);
            builder.RegisterInstance(_prefabProvider);
            builder.RegisterInstance(_configProvider);
            RegisterSystems(builder);
            builder.UseEntryPoints(Lifetime.Singleton, pointsBuilder => { pointsBuilder.Add<MainWorld>(); });
        }

        private void RegisterSystems(IContainerBuilder builder)
        {
            builder.Register<PlayerSystem>(Lifetime.Singleton);
            builder.Register<MageSystem>(Lifetime.Singleton);
            builder.Register<MageBombSystem>(Lifetime.Singleton);
            builder.Register<WhaleSpawnSystem>(Lifetime.Singleton).WithParameter(_whaleSpawnPoints);
            builder.Register<SelectionSystem>(Lifetime.Singleton);
            builder.Register<SelectionViewSystem>(Lifetime.Singleton);
            builder.Register<ProjectileMoveSystem>(Lifetime.Singleton);
            builder.Register<DealDamageSystem>(Lifetime.Singleton);
            builder.Register<HealthSystem>(Lifetime.Singleton);
        }
    }
}