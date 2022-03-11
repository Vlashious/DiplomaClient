using UnityEngine;

namespace Domain.Input
{
    public struct TransformComponent
    {
        public Transform Transform { get; }

        public TransformComponent(Transform transform)
        {
            Transform = transform;
        }
    }
}