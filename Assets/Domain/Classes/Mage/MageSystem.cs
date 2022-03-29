using Domain.Network;
using Domain.Player;
using Domain.Providers;
using Domain.Selection;
using Domain.Shared;
using Domain.UI;
using Leopotam.EcsLite;

namespace Domain.Classes.Mage
{
    public class MageSystem : IEcsRunSystem
    {
        private readonly UIProvider _uiProvider;
        private readonly PrefabProvider _prefabProvider;
        private readonly SynchronizeMap _synchronizeMap;
        private EcsWorld _world;
        private PlayerInputs _inputSystem;
        private PlayerProvider _player;

        public MageSystem(UIProvider uiProvider, PrefabProvider prefabProvider, SynchronizeMap synchronizeMap)
        {
            _uiProvider = uiProvider;
            _prefabProvider = prefabProvider;
            _synchronizeMap = synchronizeMap;
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
            foreach (int entity in _world.Filter<TransformComponent>().Inc<SelectedTag>().End())
            {
                var projectileEntity = _world.NewEntity();

                _world.GetPool<MageFirstAbilitySpawnEvent>().Add(projectileEntity) =
                    new MageFirstAbilitySpawnEvent(_synchronizeMap[entity], _player.Transform.position);
            }
        }

        private void SpawnSecondAbility()
        {
            foreach (int entity in _world.Filter<TransformComponent>().Inc<SelectedTag>().End())
            {
                var bombPool = _world.GetPool<MageBomb>();

                if (bombPool.Has(entity)) { }
                else
                {
                    ref var bomb = ref _world.GetPool<MageBomb>().Add(entity);
                    bomb.Duration = bomb.MaxDuration = 5;
                    bomb.Damage = 100;
                }
            }
        }

        private void SpawnSpecialAbility() { }
    }
}