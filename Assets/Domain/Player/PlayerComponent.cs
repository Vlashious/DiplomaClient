namespace Domain.Player
{
    public struct PlayerComponent
    {
        public PlayerProvider Player { get; }

        public PlayerComponent(PlayerProvider player)
        {
            Player = player;
        }
    }
}