using Domain.World;
using VContainer;
using VContainer.Unity;

namespace Domain
{
    public sealed class EntryPoint : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.UseEntryPoints(Lifetime.Singleton, pointsBuilder => { pointsBuilder.Add<MainWorld>(); });
        }
    }
}