using Leopotam.EcsLite;
using UnityEngine;

namespace Domain.Shared
{
    public class ProjectileMoveSystem : IEcsRunSystem
    {
        public void Run(EcsSystems systems)
        {
            var world = systems.GetWorld();

            foreach (int entity in world.Filter<TransformComponent>().Inc<ProjectileTargetTag>().End())
            {
                var target = world.GetPool<ProjectileTargetTag>().Get(entity);
                var projectile = world.GetPool<TransformComponent>().Get(entity).Transform;
                var moveDir = Vector3.MoveTowards(projectile.position, target.Transform.position, target.Speed * Time.deltaTime);
                projectile.transform.position = moveDir;
            }
        }
    }
}