using Unity.Mathematics;

namespace Domain.Classes.Mage
{
    public struct MageFirstAbilitySpawnEvent
    {
        public int TargetId;
        public float3 SpawnPos;

        public MageFirstAbilitySpawnEvent(int targetId, float3 spawnPos)
        {
            TargetId = targetId;
            SpawnPos = spawnPos;
        }
    }
}