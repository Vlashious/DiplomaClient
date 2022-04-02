using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;

namespace Inputs
{
    public sealed class ButtonInput : OnScreenControl, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField]
        [InputControl]
        private string _path;
        protected override string controlPathInternal
        {
            get => _path;
            set => _path = value;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            SendValueToControl(1f);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            SendValueToControl(0f);
        }
    }
}