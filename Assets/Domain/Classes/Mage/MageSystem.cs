using System.IO;
using Domain.Network;
using Domain.Player;
using Domain.Shared;
using Domain.UI;
using Domain.Utils;
using Leopotam.EcsLite;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Domain.Classes.Mage
{
    public class MageSystem : IEcsRunSystem
    {
        private readonly UIProvider _uiProvider;
        private readonly SynchronizeMap _synchronizeMap;
        private readonly UtilCamera _camera;

        private EcsWorld _world;
        private PlayerInputs _inputSystem;
        private PlayerProvider _player;
        private MageAbility _selectedAbility;

        public MageSystem(UIProvider uiProvider, SynchronizeMap synchronizeMap, UtilCamera camera)
        {
            _uiProvider = uiProvider;
            _synchronizeMap = synchronizeMap;
            _camera = camera;
        }

        public void Run(EcsSystems systems)
        {
            _world = systems.GetWorld();

            foreach (int player in _world.Filter<PlayerComponent>().Inc<MageTag>().End())
            {
                _player = _world.GetPool<PlayerComponent>().Get(player).Player;
                _inputSystem = _world.GetPool<PlayerComponent>().Get(player).Player.PlayerInput;

                _uiProvider.FirstAbility.Name.SetText("Fireball");
                _uiProvider.SecondAbility.Name.SetText("Bomb");
                _uiProvider.SpecialAbility.Name.SetText("Curse");

                if (_inputSystem.Player.FirstAbility.WasPressedThisFrame())
                {
                    _selectedAbility = MageAbility.Fireball;
                }

                if (_inputSystem.Player.SecondAbility.WasPressedThisFrame())
                {
                    _selectedAbility = MageAbility.Bomb;
                }

                if (_inputSystem.Player.SpecialAbility.WasPressedThisFrame())
                {
                    _selectedAbility = MageAbility.Curse;
                }

                TryActivateAbility();
            }
        }

        private void TryActivateAbility()
        {
            if (!EventSystem.current.IsPointerOverGameObject() &&
                _inputSystem.Player.Select.IsPressed() &&
                _selectedAbility is not MageAbility.None)
            {
                var input = _inputSystem.Player.Select.ReadValue<Vector2>();
                var ray = _camera.Camera.ScreenPointToRay(input);

                if (Physics.Raycast(ray, out var hitInfo))
                {
                    var clickedEntity = -1;

                    var hasFoundEntity = hitInfo.transform.gameObject.TryGetComponent(out PackedEntity packedEntity) &&
                                         packedEntity.Entity.Unpack(_world, out clickedEntity);

                    switch (_selectedAbility)
                    {
                        case MageAbility.Fireball when hasFoundEntity:
                            SpawnFirstAbility(clickedEntity);
                            break;
                        case MageAbility.Bomb when hasFoundEntity:
                            SpawnSecondAbility(clickedEntity);
                            break;
                        case MageAbility.Curse:
                            SpawnSpecialAbility(hitInfo.point);
                            break;
                    }
                }

                _selectedAbility = MageAbility.None;
            }
        }

        private void SpawnFirstAbility(int clickedEntity)
        {
            using var ms = new MemoryStream();
            using var wr = new BinaryWriter(ms);
            wr.Write(_synchronizeMap[clickedEntity]);
            wr.Write(_player.Transform.position.x);
            wr.Write(_player.Transform.position.y);
            wr.Write(_player.Transform.position.z);
            var networkPacket = _world.NewEntity();

            _world.GetPool<NetworkPacket>().Add(networkPacket) =
                new NetworkPacket("SpawnMageProjectile", ms.ToArray());
        }

        private void SpawnSecondAbility(int clickedEntity)
        {
            using var ms = new MemoryStream();
            using var wr = new BinaryWriter(ms);
            wr.Write(_synchronizeMap[clickedEntity]);
            var networkPacket = _world.NewEntity();

            _world.GetPool<NetworkPacket>().Add(networkPacket) =
                new NetworkPacket("SpawnMageBomb", ms.ToArray());
        }

        private void SpawnSpecialAbility(float3 clickPosition)
        {
            using var ms = new MemoryStream();
            using var wr = new BinaryWriter(ms);
            wr.Write(clickPosition.x);
            wr.Write(clickPosition.y);
            wr.Write(clickPosition.z);
            var networkPacket = _world.NewEntity();

            _world.GetPool<NetworkPacket>().Add(networkPacket) = new NetworkPacket("SpawnMageCurse", ms.ToArray());
        }
    }

    public enum MageAbility
    {
        None,
        Fireball,
        Bomb,
        Curse
    }
}