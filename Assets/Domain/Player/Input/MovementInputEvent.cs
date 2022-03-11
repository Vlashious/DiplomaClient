using Unity.Mathematics;

namespace Domain.Player.Input
{
    public struct MovementInputEvent
    {
        public MovementInputEvent(float3 direction)
        {
            Direction = direction;
        }

        public float3 Direction { get; }
    }
}