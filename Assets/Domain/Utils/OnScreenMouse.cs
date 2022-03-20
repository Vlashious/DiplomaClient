using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;

namespace Domain.Utils
{
    public sealed class OnScreenMouse : OnScreenControl, IDragHandler
    {
        [SerializeField, InputControl(layout = "Vector2")]
        private string _controlPath;
        protected override string controlPathInternal
        {
            get => _controlPath;
            set => _controlPath = value;
        }
        private bool _hasDrag;
        private Vector2 _delta;

        private void LateUpdate()
        {
            if (_hasDrag)
            {
                SendValueToControl(_delta);
                _hasDrag = false;
            }
            else
            {
                SendValueToControl(new Vector2(0, 0));
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            _hasDrag = true;
            _delta = eventData.delta;
        }
    }
}