﻿using System.IO;
using Domain.Network;
using Domain.Player;
using Domain.Shared;
using Domain.UI;
using Leopotam.EcsLite;

namespace Domain.Classes.Priest
{
    public class PriestSystem : IEcsRunSystem
    {
        private readonly UIProvider _uiProvider;
        private readonly SynchronizeMap _synchronizeMap;
        private EcsWorld _world;
        private PlayerInputs _inputSystem;
        private PlayerProvider _player;

        public PriestSystem(UIProvider uiProvider, SynchronizeMap synchronizeMap)
        {
            _uiProvider = uiProvider;
            _synchronizeMap = synchronizeMap;
        }

        public void Run(EcsSystems systems)
        {
            _world = systems.GetWorld();

            foreach (int player in _world.Filter<PlayerComponent>().Inc<PriestTag>().End())
            {
                _player = _world.GetPool<PlayerComponent>().Get(player).Player;
                _inputSystem = _world.GetPool<PlayerComponent>().Get(player).Player.PlayerInput;

                _uiProvider.FirstAbility.Name.SetText("Heal");
                _uiProvider.SecondAbility.Name.SetText("Blessing");
                _uiProvider.SpecialAbility.Name.SetText("Resurrect");

                if (_inputSystem.Player.FirstAbility.WasPressedThisFrame())
                {
                    SpawnFirstAbility();
                }

                if (_inputSystem.Player.SecondAbility.WasPressedThisFrame())
                {
                    SpawnSecondAbility();
                }

                if (_inputSystem.Player.SpecialAbility.WasPressedThisFrame())
                {
                    SpawnSpecialAbility();
                }
            }
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
                    new NetworkPacket("PriestHeal", ms.ToArray());
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
                    new NetworkPacket("PriestBlessing", ms.ToArray());
            }
        }

        private void SpawnSpecialAbility() { }
    }
}