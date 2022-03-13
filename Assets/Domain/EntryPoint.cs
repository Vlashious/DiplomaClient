using Domain.Player;
using Domain.Providers;
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
        private UtilCamera _utilCamera;
        [SerializeField]
        private PrefabProvider _prefabProvider;
        [SerializeField]
        private ConfigProvider _configProvider;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(_utilCamera);
            builder.RegisterInstance(_prefabProvider);
            builder.RegisterInstance(_configProvider);
            RegisterSystems(builder);
            builder.UseEntryPoints(Lifetime.Singleton, pointsBuilder => { pointsBuilder.Add<MainWorld>(); });
        }

        private void RegisterSystems(IContainerBuilder builder)
        {
            builder.Register<PlayerSystem>(Lifetime.Singleton);
        }
    }
}