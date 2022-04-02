using System;
using VContainer;
using VContainer.Unity;
using Object = UnityEngine.Object;

namespace Domain.UI
{
    public class HUDController : IInitializable, IDisposable
    {
        private readonly UIProvider _uiProvider;
        private readonly IObjectResolver _resolver;

        public HUDController(UIProvider uiProvider, IObjectResolver resolver)
        {
            _uiProvider = uiProvider;
            _resolver = resolver;
        }

        public void Initialize()
        {
            _uiProvider.HUDProvider.ShopClick += OnShopClicked;
            _uiProvider.HUDProvider.InfoClick += OnInfoClicked;
        }

        private void OnShopClicked()
        {
            var shop = _resolver.Instantiate(_uiProvider.HUDProvider.ShopProvider, _uiProvider.transform);
            shop.Close += OnCloseClicked;

            void OnCloseClicked()
            {
                Object.Destroy(shop.gameObject);
            }
        }

        private void OnInfoClicked()
        {
            _resolver.Resolve<InfoProvider>();
        }

        public void Dispose()
        {
            _uiProvider.HUDProvider.ShopClick -= OnShopClicked;
            _uiProvider.HUDProvider.InfoClick -= OnInfoClicked;
        }
    }
}