using Domain.Damage;
using Leopotam.EcsLite;
using Unity.Mathematics;
using UnityEngine;

namespace Domain.Shared
{
    public class ProjectileMoveSystem : IEcsRunSystem
    {
        public void Run(EcsSystems systems)
        {
            var world = systems.GetWorld();

            foreach (int entity in world.Filter<TransformComponent>().Inc<DamageProjectile>().End())
            {
                var damageProjectile = world.GetPool<DamageProjectile>().Get(entity);
                var transform = world.GetPool<TransformComponent>().Get(entity).Transform;

                var moveDir = Vector3.MoveTowards(transform.position, damageProjectile.Target.position,
                    damageProjectile.Speed * Time.deltaTime);
                transform.transform.position = moveDir;

                if (math.distancesq(moveDir, damageProjectile.Target.position) < 0.1f)
                {
                    world.DelEntity(entity);
                    Object.Destroy(transform.gameObject, 0.5f);

                    if (damageProjectile.Target.TryGetComponent(out PackedEntity packedEntity) &&
                        packedEntity.Entity.Unpack(world, out var targetEntity))
                    {
                        ref var damage = ref world.GetPool<DealDamageComponent>().Add(targetEntity);
                        damage = new DealDamageComponent(damageProjectile.Damage);
                    }
                }
            }
        }
    }
}