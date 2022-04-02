using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;

namespace Inputs
{
    public sealed class DragInput : OnScreenControl, IDragHandler
    {
        [InputControl]
        [SerializeField]
        private string _path;
        protected override string controlPathInternal
        {
            get => _path;
            set => _path = value;
        }

        public void OnDrag(PointerEventData eventData)
        {
            SendValueToControl(eventData.delta);
        }
    }
}