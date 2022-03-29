using Unity.Mathematics;

namespace Domain.Shared
{
    public struct EntityPositionChangedEvent
    {
        public int Id;
        public float3 NewPosition;
        public float3 NewRotation;

        public EntityPositionChangedEvent(float3 newPosition, float3 newRotation, int id)
        {
            NewPosition = newPosition;
            NewRotation = newRotation;
            Id = id;
        }
    }
}