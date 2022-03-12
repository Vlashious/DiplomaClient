using Unity.Mathematics;

namespace Domain.Common
{
    public struct MovementEvent
    {
        public float3 Direction { get; }

        public MovementEvent(float3 direction)
        {
            Direction = direction;
        }
    }
}