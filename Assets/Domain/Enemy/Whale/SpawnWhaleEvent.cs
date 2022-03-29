using Unity.Mathematics;

namespace Domain.Enemy.Whale
{
    public struct SpawnWhaleEvent
    {
        public int Id;
        public float3 Position;
        public int Health;

        public SpawnWhaleEvent(float3 position, int id, int health)
        {
            Position = position;
            Id = id;
            Health = health;
        }
    }
}