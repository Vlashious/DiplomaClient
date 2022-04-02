﻿using System;
using System.IO;
using Domain.Network;
using Domain.Player;
using Domain.Shared;
using Domain.UI;
using Domain.Utils;
using Leopotam.EcsLite;
using UnityEngine;

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

        private bool _isSelecting = false;
        private Action _abiltyToSpawn;

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
                    _abiltyToSpawn = SpawnFirstAbility;
                }

                if (_inputSystem.Player.SecondAbility.WasPressedThisFrame())
                {
                    _abiltyToSpawn = SpawnSecondAbility;
                }

                if (_inputSystem.Player.SpecialAbility.WasPressedThisFrame())
                {
                    _abiltyToSpawn = SpawnSpecialAbility;
                }

                TryActivateAbility(out _);
            }
        }

        private bool TryActivateAbility(out int clickedEntity)
        {
            if (_abiltyToSpawn is not null &&
                _inputSystem.Player.Select.WasPerformedThisFrame())
            {
                var input = _inputSystem.Player.Select.ReadValue<Vector2>();
                var ray = _camera.Camera.ScreenPointToRay(input);

                if (Physics.Raycast(ray, out var hitInfo) &&
                    hitInfo.transform.TryGetComponent(out PackedEntity packedEntity) &&
                    packedEntity.Entity.Unpack(_world, out clickedEntity))
                {
                    _world.GetPool<Selection>().Add(clickedEntity);
                    _abiltyToSpawn.Invoke();
                    _world.GetPool<Selection>().Del(clickedEntity);

                    _abiltyToSpawn = default;
                    return true;
                }
            }

            clickedEntity = default;
            return false;
        }

        private void SpawnFirstAbility()
        {
            foreach (int entity in _world.Filter<TransformComponent>().Inc<Selection>().End())
            {
                using var ms = new MemoryStream();
                using var wr = new BinaryWriter(ms);
                wr.Write(_synchronizeMap[entity]);
                wr.Write(_player.Transform.position.x);
                wr.Write(_player.Transform.position.y);
                wr.Write(_player.Transform.position.z);
                var networkPacket = _world.NewEntity();

                _world.GetPool<NetworkPacket>().Add(networkPacket) =
                    new NetworkPacket("SpawnMageProjectile", ms.ToArray());
            }
        }

        private void SpawnSecondAbility()
        {
            foreach (int entity in _world.Filter<TransformComponent>().Inc<Selection>().End())
            {
                using var ms = new MemoryStream();
                using var wr = new BinaryWriter(ms);
                wr.Write(_synchronizeMap[entity]);
                var networkPacket = _world.NewEntity();

                _world.GetPool<NetworkPacket>().Add(networkPacket) =
                    new NetworkPacket("SpawnMageBomb", ms.ToArray());
            }
        }

        private void SpawnSpecialAbility() { }
    }
}