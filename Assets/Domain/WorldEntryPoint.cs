using Domain.Classes;
using Domain.Classes.Mage;
using Domain.Classes.Priest;
using Domain.Enemy.Whale;
using Domain.Health;
using Domain.Network;
using Domain.Player;
using Domain.Projectile;
using Domain.Providers;
using Domain.Shared;
using Domain.UI;
using Domain.Utils;
using Domain.World;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Domain
{
    public sealed class WorldEntryPoint : LifetimeScope
    {
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
            Application.targetFrameRate = 60;
            builder.RegisterComponent(_utilCamera);
            builder.RegisterComponent(_utilCanvas);
            builder.RegisterComponent(_uiProvider);
            builder.RegisterInstance(_prefabProvider);
            builder.RegisterInstance(_configProvider);
            RegisterSystems(builder);
            RegisterUI(builder);

            builder.UseEntryPoints(Lifetime.Singleton, pointsBuilder =>
            {
                pointsBuilder.Add<MainWorld>().As<MainWorld>();
                pointsBuilder.Add<HUDController>();
            });
        }

        private void RegisterUI(IContainerBuilder builder)
        {
            builder.Register<ShopProvider>(Lifetime.Transient);
            builder.Register<InfoProvider>(Lifetime.Transient);
        }

        private void RegisterSystems(IContainerBuilder builder)
        {
            builder.Register<SynchronizeMap>(Lifetime.Singleton);
            builder.Register<PlayerSystem>(Lifetime.Singleton);
            builder.Register<PlayerSpawnSystem>(Lifetime.Singleton);
            builder.Register<MageSystem>(Lifetime.Singleton);
            builder.Register<MageBombSystem>(Lifetime.Singleton);
            builder.Register<PriestSystem>(Lifetime.Singleton);
            builder.Register<ProjectileMoveSystem>(Lifetime.Singleton);
            builder.Register<HealthSystem>(Lifetime.Singleton);
            builder.Register<CreatureInspectorSystem>(Lifetime.Singleton);
            builder.Register<NetworkingSystem>(Lifetime.Singleton);
            builder.Register<WhaleSpawnSystem>(Lifetime.Singleton);
            builder.Register<ChangeClassSystem>(Lifetime.Singleton);
        }
    }
}