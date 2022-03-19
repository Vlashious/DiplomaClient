using Domain.Providers;
using Domain.Shared;
using Domain.UI;
using Domain.Utils;
using Leopotam.EcsLite;
using UnityEngine;

namespace Domain.Selection
{
    public class SelectionViewSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly PrefabProvider _prefabProvider;
        private readonly UIProvider _uiProvider;
        private readonly UtilCanvas _utilCanvas;

        private GameObject _selectionView;
        private EcsWorld _world;

        public SelectionViewSystem(PrefabProvider prefabProvider, UIProvider uiProvider, UtilCanvas utilCanvas)
        {
            _prefabProvider = prefabProvider;
            _uiProvider = uiProvider;
            _utilCanvas = utilCanvas;
        }

        public void Init(EcsSystems systems)
        {
            _world = systems.GetWorld();
            _selectionView = Object.Instantiate(_prefabProvider.Selection);
            _selectionView.SetActive(false);
        }

        public void Run(EcsSystems systems)
        {
            UpdateViewInWorld();
            UpdateViewInUI();
        }

        private void UpdateViewInWorld()
        {
            var filter = _world.Filter<SelectedTag>().Inc<TransformComponent>().End();
            var isSelectionVisible = filter.GetEntitiesCount() > 0;
            _selectionView.SetActive(isSelectionVisible);

            if (isSelectionVisible)
            {
                foreach (int selectedEntity in filter)
                {
                    TransformComponent transform = _world.GetPool<TransformComponent>().Get(selectedEntity);
                    _selectionView.transform.position = transform.Transform.position + Vector3.up * 0.01f;
                }
            }
        }

        private void UpdateViewInUI()
        {
            var filter = _world.Filter<SelectedTag>().Inc<HealthComponent>().Inc<NameComponent>().End();
            var isSelectionVisible = filter.GetEntitiesCount() > 0;
            _uiProvider.CreatureInspectorProvider.gameObject.SetActive(isSelectionVisible);

            if (isSelectionVisible)
            {
                foreach (int selectedEntity in filter)
                {
                    HealthComponent healthComponent = _world.GetPool<HealthComponent>().Get(selectedEntity);
                    _uiProvider.CreatureInspectorProvider.SetValue(healthComponent.Health, healthComponent.MaxHealth);
                    NameComponent nameComponent = _world.GetPool<NameComponent>().Get(selectedEntity);
                    _uiProvider.CreatureInspectorProvider.Name.SetText(nameComponent.Name);
                }
            }
        }
    }
}