namespace Domain.Shared
{
    public struct NameComponent
    {
        public NameComponent(string name)
        {
            Name = name;
        }
        public string Name { get; set; }
    }
}