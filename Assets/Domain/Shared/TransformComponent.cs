using System.IO;
using Domain.Network;
using UnityEngine;

namespace Domain.Shared
{
    public struct TransformComponent : ISerializable<TransformComponent>
    {
        public Transform Transform;

        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public byte[] Serialize()
        {
            using var stream = new MemoryStream(sizeof(float) * 3);
            using var binaryWriter = new BinaryWriter(stream);
            binaryWriter.Write(X);
            binaryWriter.Write(Y);
            binaryWriter.Write(Z);

            return stream.ToArray();
        }

        public static TransformComponent Deserialize(byte[] data)
        {
            var transform = new TransformComponent();
            using var stream = new MemoryStream(data);
            using var binaryReader = new BinaryReader(stream);
            transform.X = binaryReader.ReadSingle();
            transform.Y = binaryReader.ReadSingle();
            transform.Z = binaryReader.ReadSingle();
            return transform;
        }
    }
}