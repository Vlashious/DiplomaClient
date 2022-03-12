using Cinemachine;
using UnityEngine;

namespace Domain.Player
{
    public sealed class PlayerProvider : MonoBehaviour
    {
        public Transform Transform;
        public CharacterController CharacterController;
        public Animator Animator;
        public CinemachineFreeLook FreeLookCamera;
    }
}