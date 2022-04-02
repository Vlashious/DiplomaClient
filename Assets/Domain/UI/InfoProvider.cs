using System;
using UnityEngine;

namespace Domain.UI
{
    public sealed class InfoProvider : MonoBehaviour
    {
        public event Action MageClick;
        public event Action PriestClick;
        public event Action WarriorClick;
        public event Action Close;

        public void OnMageClicked()
        {
            MageClick?.Invoke();
        }

        public void OnPriestClicked()
        {
            PriestClick?.Invoke();
        }

        public void OnWarriorClicked()
        {
            WarriorClick?.Invoke();
        }

        public void OnCloseClicked()
        {
            Close?.Invoke();
        }
    }
}