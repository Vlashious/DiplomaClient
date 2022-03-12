using Unity.Mathematics;

namespace Domain.Player
{
    public struct PlayerMovementEvent
    {
        public float3 Direction { get; }

        public PlayerMovementEvent(float3 direction)
        {
            Direction = direction;
        }
    }
}