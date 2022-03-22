using Domain.Player;
using Domain.Providers;
using Domain.Selection;
using Domain.Shared;
using Domain.UI;
using Leopotam.EcsLite;
using UnityEngine;

namespace Domain.Classes.Mage
{
    public class MageSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly UIProvider _uiProvider;
        private readonly PrefabProvider _prefabProvider;
        private EcsWorld _world;
        private PlayerInputs _inputSystem;
        private PlayerProvider _player;

        public MageSystem(UIProvider uiProvider, PrefabProvider prefabProvider)
        {
            _uiProvider = uiProvider;
            _prefabProvider = prefabProvider;
        }

        public void Init(EcsSystems systems)
        {
            _world = systems.GetWorld();

            foreach (int player in _world.Filter<PlayerComponent>().Inc<MageTag>().End())
            {
                _player = _world.GetPool<PlayerComponent>().Get(player).Player;
                _inputSystem = _world.GetPool<PlayerComponent>().Get(player).Player.PlayerInput;

                _uiProvider.FirstAbility.Name.SetText("Fireball");
                _uiProvider.SecondAbility.Name.SetText("Bomb");
                _uiProvider.SpecialAbility.Name.SetText("Curse");
            }
        }

        public void Run(EcsSystems systems)
        {
            foreach (int player in _world.Filter<PlayerComponent>().Inc<MageTag>().End())
            {
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
                    Debug.Log("Mage spceial ability");
                }
            }
        }

        private void SpawnFirstAbility()
        {
            foreach (int entity in _world.Filter<TransformComponent>().Inc<SelectedTag>().End())
            {
                var target = _world.GetPool<TransformComponent>().Get(entity);
                var fireball = _world.NewEntity();
                var go = Object.Instantiate(_prefabProvider.Fireball, _player.Transform.position, Quaternion.identity);
                _world.GetPool<TransformComponent>().Add(fireball).Transform = go.transform;
                ref var projectile = ref _world.GetPool<DamageProjectile>().Add(fireball);
                projectile.Speed = 50;
                projectile.Target = target.Transform;
                projectile.Damage = 10;
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
    }
}