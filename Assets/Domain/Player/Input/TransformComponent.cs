using UnityEngine;

namespace Domain.Player.Input
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