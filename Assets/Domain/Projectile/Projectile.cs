namespace Domain.Projectile
{
    public struct Projectile
    {
        public int TargetServerId;
        public float Speed;

        public Projectile(int targetServerId, float speed)
        {
            TargetServerId = targetServerId;
            Speed = speed;
        }
    }
}