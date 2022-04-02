using System;
using UnityEngine;

namespace Domain.UI
{
    public sealed class HUDProvider : MonoBehaviour
    {
        public event Action ShopClick;
        public event Action InfoClick;

        public ShopProvider ShopProvider;
        public InfoProvider InfoProvider;

        public void OnShopClicked()
        {
            ShopClick?.Invoke();
        }

        public void OnInfoClicked()
        {
            InfoClick?.Invoke();
        }
    }
}