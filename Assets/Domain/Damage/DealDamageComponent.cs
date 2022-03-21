namespace Domain.Damage
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