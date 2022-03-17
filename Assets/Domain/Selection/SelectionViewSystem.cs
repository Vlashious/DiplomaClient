using Domain.Providers;
using Domain.Utils;
using Leopotam.EcsLite;
using UnityEngine;

namespace Domain.Selection
{
    public class SelectionViewSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly PrefabProvider _prefabProvider;

        private GameObject _selectionView;

        public SelectionViewSystem(PrefabProvider prefabProvider)
        {
            _prefabProvider = prefabProvider;
        }

        public void Init(EcsSystems systems)
        {
            _selectionView = Object.Instantiate(_prefabProvider.Selection);
            _selectionView.SetActive(false);
        }

        public void Run(EcsSystems systems)
        {
            var world = systems.GetWorld();

            var filter = world.Filter<SelectedTag>().Inc<TransformComponent>().End();
            var isSelectionVisible = filter.GetEntitiesCount() > 0;
            _selectionView.SetActive(isSelectionVisible);

            if (isSelectionVisible)
            {
                foreach (int selectedEntity in filter)
                {
                    TransformComponent transform = world.GetPool<TransformComponent>().Get(selectedEntity);
                    _selectionView.transform.position = transform.Transform.position;
                }
            }
        }
    }
}