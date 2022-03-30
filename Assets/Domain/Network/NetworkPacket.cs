namespace Domain.Network
{
    public struct NetworkPacket
    {
        public readonly string Method;
        public readonly byte[] Data;

        public NetworkPacket(string method, byte[] data)
        {
            Method = method;
            Data = data;
        }
    }
}