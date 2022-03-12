using Unity.Mathematics;

namespace Domain.Player
{
    public struct PlayerComponent
    {
        public PlayerProvider Player { get; }
        public float3 MovementDirection { get; set; }

        public PlayerComponent(PlayerProvider player)
        {
            Player = player;
            MovementDirection = default;
        }
    }
}