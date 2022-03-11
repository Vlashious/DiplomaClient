namespace Domain.MessageOnClickFeature
{
    public struct MessageComponent
    {
        public MessageComponent(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }
}