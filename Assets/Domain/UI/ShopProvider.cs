using System;
using UnityEngine;

namespace Domain.UI
{
    public sealed class ShopProvider : MonoBehaviour
    {
        public event Action Close;

        public void OnCloseClicked()
        {
            Close?.Invoke();
        }
    }
}