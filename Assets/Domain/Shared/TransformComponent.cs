using System.IO;
using Domain.Network;
using Unity.Mathematics;
using UnityEngine;

namespace Domain.Shared
{
    public struct TransformComponent : ISerializable<TransformComponent>
    {
        public Transform Transform;

        public byte[] Serialize()
        {
            using var stream = new MemoryStream(sizeof(float) * 3);
            using var binaryWriter = new BinaryWriter(stream);
            binaryWriter.Write(Transform.position.x);
            binaryWriter.Write(Transform.position.y);
            binaryWriter.Write(Transform.position.z);
            binaryWriter.Write(Transform.rotation.eulerAngles.x);
            binaryWriter.Write(Transform.rotation.eulerAngles.y);
            binaryWriter.Write(Transform.rotation.eulerAngles.z);

            return stream.ToArray();
        }

        public static (float3 pos, float3 rot) Deserialize(byte[] data)
        {
            using var stream = new MemoryStream(data);
            using var br = new BinaryReader(stream);
            var position = new float3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
            var rotation = new float3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
            return (position, rotation);
        }
    }
}