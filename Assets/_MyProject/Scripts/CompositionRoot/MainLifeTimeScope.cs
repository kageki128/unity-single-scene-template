using MyProject.Core;
using MyProject.Director;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace MyProject.CompositionRoot
{
    public class MainLifeTimeScope : LifetimeScope
    {
        [Header("Actor")]
        [Header("Config")]
        [SerializeField] GameConfigSO gameConfig;

        protected override void Configure(IContainerBuilder builder)
        {
            RegisterCore(builder);
            RegisterActor(builder);
            RegisterDirector(builder);
            RegisterInfrastructure(builder);
        }

        void RegisterCore(IContainerBuilder builder)
        {
            builder.Register<SceneCore>(Lifetime.Singleton);
        }

        void RegisterActor(IContainerBuilder builder)
        {
            
        }

        void RegisterDirector(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<MainEntryPoint>(Lifetime.Singleton);
            builder.Register<TitleSceneDirector>(Lifetime.Singleton);
            builder.Register<SelectSceneDirector>(Lifetime.Singleton);
            builder.Register<GameSceneDirector>(Lifetime.Singleton);
            builder.Register<ResultSceneDirector>(Lifetime.Singleton);
        }

        void RegisterInfrastructure(IContainerBuilder builder)
        {
            
        }
    }
}