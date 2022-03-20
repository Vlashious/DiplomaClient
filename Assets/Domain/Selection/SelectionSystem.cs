using Domain.Player;
using Domain.Shared;
using Domain.Utils;
using Leopotam.EcsLite;
using UnityEngine;

namespace Domain.Selection
{
    public class SelectionSystem : IEcsRunSystem
    {
        private readonly UtilCamera _camera;

        private GameObject _activeSelection;

        public SelectionSystem(UtilCamera camera)
        {
            _camera = camera;
        }

        public void Run(EcsSystems systems)
        {
            var world = systems.GetWorld();

            foreach (int player in world.Filter<PlayerComponent>().End())
            {
                ref var playerComponent = ref world.GetPool<PlayerComponent>().Get(player);

                if (playerComponent.Player.PlayerInput.Player.Select.WasPressedThisFrame())
                {
                    foreach (int selectedEntity in world.Filter<SelectedTag>().End())
                    {
                        world.GetPool<SelectedTag>().Del(selectedEntity);
                    }

                    var pressedPos = playerComponent.Player.PlayerInput.Player.Select.ReadValue<Vector2>();
                    var ray = _camera.Camera.ScreenPointToRay(pressedPos);

                    if (Physics.Raycast(ray, out var hitInfo) &&
                        hitInfo.collider.gameObject.TryGetComponent(out PackedEntity entity) &&
                        entity.Entity.Unpack(world, out var clickedEntity))
                    {
                        world.GetPool<SelectedTag>().Add(clickedEntity);
                    }
                }
            }
        }
    }
}