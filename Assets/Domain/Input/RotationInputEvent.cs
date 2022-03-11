using Unity.Mathematics;

namespace Domain.Input
{
    public struct RotationInputEvent
    {
        public RotationInputEvent(float3 direction)
        {
            Direction = direction;
        }
        public float3 Direction { get; }
    }
}