namespace Domain.Player
{
    public struct PlayerComponent
    {
        public PlayerComponent(PlayerProvider player)
        {
            Player = player;
        }

        public PlayerProvider Player { get; }
    }
}