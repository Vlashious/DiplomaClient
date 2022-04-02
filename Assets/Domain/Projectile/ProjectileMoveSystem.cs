using Domain.Network;
using Domain.Shared;
using Leopotam.EcsLite;
using Unity.Mathematics;
using UnityEngine;

namespace Domain.Projectile
{
    public class ProjectileMoveSystem : IEcsRunSystem
    {
        private readonly SynchronizeMap _synchronizeMap;

        public ProjectileMoveSystem(SynchronizeMap synchronizeMap)
        {
            _synchronizeMap = synchronizeMap;
        }

        public void Run(EcsSystems systems)
        {
            var world = systems.GetWorld();

            foreach (int entity in world.Filter<Projectile>().Inc<TransformComponent>().End())
            {
                var damageProjectile = world.GetPool<Projectile>().Get(entity);
                var transformPool = world.GetPool<TransformComponent>();
                var projectileTransform = transformPool.Get(entity);

                var innerId = _synchronizeMap.GetInnerId(damageProjectile.TargetServerId);

                if (transformPool.Has(innerId))
                {
                    var targetPos = transformPool.Get(innerId).Transform;

                    var moveDir = (targetPos.position - projectileTransform.Transform.position).normalized;
                    moveDir = moveDir * damageProjectile.Speed * Time.deltaTime;
                    projectileTransform.Transform.transform.position += moveDir;

                    if (math.distancesq(projectileTransform.Transform.transform.position, targetPos.position) < 0.1f)
                    {
                        world.DelEntity(entity);
                        Object.Destroy(projectileTransform.Transform.gameObject, 1f);
                    }
                }
            }
        }
    }
}