using Domain.Providers;
using Domain.World;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Domain
{
    public sealed class EntryPoint : LifetimeScope
    {
        [SerializeField]
        private PrefabProvider _prefabProvider;
        [SerializeField]
        private ConfigProvider _configProvider;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(_prefabProvider);
            builder.RegisterComponent(_configProvider);
            builder.UseEntryPoints(Lifetime.Singleton, pointsBuilder => { pointsBuilder.Add<MainWorld>(); });
        }
    }
}