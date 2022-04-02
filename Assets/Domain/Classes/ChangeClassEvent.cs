namespace Domain.Classes
{
    public struct ChangeClassEvent
    {
        public ClassType Class;

        public ChangeClassEvent(ClassType @class)
        {
            Class = @class;
        }
    }

    public enum ClassType
    {
        Mage,
        Priest,
        Warrior
    }
}