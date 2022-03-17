﻿using System;
using Domain.Player;
using Domain.Selection;
using Leopotam.EcsLite;
using VContainer;
using VContainer.Unity;

namespace Domain.World
{
    public sealed class MainWorld : IInitializable, ITickable, IDisposable
    {
        private readonly IObjectResolver _resolver;
        private EcsWorld _ecsWorld;
        private EcsSystems _ecsSystems;

        public MainWorld(IObjectResolver resolver)
        {
            _resolver = resolver;
        }

        public void Initialize()
        {
            _ecsWorld = new EcsWorld();
            _ecsSystems = new EcsSystems(_ecsWorld);

            _ecsSystems.Add(_resolver.Resolve<PlayerSystem>())
                       .Add(_resolver.Resolve<SelectionSystem>())
                       .Add(_resolver.Resolve<SelectionViewSystem>())
                       .Init();
        }

        public void Tick()
        {
            _ecsSystems?.Run();
        }

        public void Dispose()
        {
            if (_ecsSystems is not null)
            {
                _ecsSystems.Destroy();
                _ecsSystems = null;
            }

            if (_ecsWorld is not null)
            {
                _ecsWorld.Destroy();
                _ecsWorld = null;
            }
        }
    }
}