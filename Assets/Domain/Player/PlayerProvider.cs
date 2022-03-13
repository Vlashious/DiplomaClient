using UnityEngine;

namespace Domain.Player
{
    public sealed class PlayerProvider : MonoBehaviour
    {
        public Transform Transform;
        public CharacterController CharacterController;
        public Animator Animator;
        public PlayerInputs PlayerInput;

        private void Awake()
        {
            PlayerInput = new();
            PlayerInput.Enable();
        }

        private void OnEnable()
        {
            PlayerInput.Enable();
        }

        private void OnDisable()
        {
            PlayerInput.Disable();
        }
    }
}