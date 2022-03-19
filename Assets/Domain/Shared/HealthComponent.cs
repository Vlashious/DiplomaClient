namespace Domain.Shared
{
    public struct HealthComponent
    {
        public HealthComponent(int maxHealth)
        {
            MaxHealth = maxHealth;
            Health = maxHealth;
        }

        public int MaxHealth { get; }
        public int Health { get; set; }
    }
}