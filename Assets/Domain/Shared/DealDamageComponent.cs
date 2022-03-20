namespace Domain.Shared
{
    public struct DealDamageComponent
    {
        public DealDamageComponent(int damage)
        {
            Damage = damage;
        }

        public int Damage { get; }
    }
}