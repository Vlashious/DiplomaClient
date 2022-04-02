using System;
using Domain.Classes;
using Domain.World;
using VContainer;
using VContainer.Unity;
using Object = UnityEngine.Object;

namespace Domain.UI
{
    public class HUDController : IInitializable, IDisposable
    {
        private readonly UIProvider _uiProvider;
        private readonly IObjectResolver _resolver;
        private readonly MainWorld _mainWorld;

        public HUDController(UIProvider uiProvider, IObjectResolver resolver, MainWorld mainWorld)
        {
            _uiProvider = uiProvider;
            _resolver = resolver;
            _mainWorld = mainWorld;
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
            var info = _resolver.Instantiate(_uiProvider.HUDProvider.InfoProvider, _uiProvider.transform);
            info.Close += OnCloseClicked;
            info.PriestClick += OnPriestClicked;
            info.MageClick += OnMageClicked;
            info.WarriorClick += OnWarriorClicked;

            void OnCloseClicked()
            {
                Object.Destroy(info.gameObject);
            }

            void OnMageClicked()
            {
                var changeClassEvent = _mainWorld.World.NewEntity();
                _mainWorld.World.GetPool<ChangeClassEvent>().Add(changeClassEvent) = new ChangeClassEvent(ClassType.Mage);
            }

            void OnPriestClicked()
            {
                var changeClassEvent = _mainWorld.World.NewEntity();
                _mainWorld.World.GetPool<ChangeClassEvent>().Add(changeClassEvent) = new ChangeClassEvent(ClassType.Priest);
            }

            void OnWarriorClicked()
            {
                var changeClassEvent = _mainWorld.World.NewEntity();
                _mainWorld.World.GetPool<ChangeClassEvent>().Add(changeClassEvent) = new ChangeClassEvent(ClassType.Warrior);
            }
        }

        public void Dispose()
        {
            _uiProvider.HUDProvider.ShopClick -= OnShopClicked;
            _uiProvider.HUDProvider.InfoClick -= OnInfoClicked;
        }
    }
}