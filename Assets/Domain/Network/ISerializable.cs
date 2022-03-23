namespace Domain.Network
{
    public interface ISerializable<T>
    {
        public byte[] Serialize();

        public static T Deserialize(byte[] data)
        {
            return default;
        }
    }
}