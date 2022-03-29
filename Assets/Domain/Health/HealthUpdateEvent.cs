namespace Domain.Health
{
    public struct HealthUpdateEvent
    {
        public int EntityId;
        public int NewHealth;
        public HealthUpdateEvent(int entityId, int newHealth)
        {
            EntityId = entityId;
            NewHealth = newHealth;
        }
    }
}