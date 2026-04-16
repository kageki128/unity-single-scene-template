using MyProject.Actor;
using MyProject.Core;
using MyProject.Director;
using MyProject.Infrastructure;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace MyProject.CompositionRoot
{
    public class MainLifeTimeScope : LifetimeScope
    {
        [Header("Actor")]
        [SerializeField] RootActorHub rootActorHub;
        [SerializeField] TitleActorHub titleActorHub;
        [SerializeField] SelectActorHub selectActorHub;
        [SerializeField] GameActorHub gameActorHub;
        [SerializeField] ResultActorHub resultActorHub;
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
            builder.RegisterInstance(gameConfig);
        }

        void RegisterActor(IContainerBuilder builder)
        {
            builder.RegisterInstance(rootActorHub);
            builder.RegisterInstance(titleActorHub);
            builder.RegisterInstance(selectActorHub);
            builder.RegisterInstance(gameActorHub);
            builder.RegisterInstance(resultActorHub);
        }

        void RegisterDirector(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<MainEntryPoint>(Lifetime.Singleton);
            builder.Register<RootDirector>(Lifetime.Singleton);
            builder.Register<TitleSceneDirector>(Lifetime.Singleton);
            builder.Register<SelectSceneDirector>(Lifetime.Singleton);
            builder.Register<GameSceneDirector>(Lifetime.Singleton);
            builder.Register<ResultSceneDirector>(Lifetime.Singleton);
        }

        void RegisterInfrastructure(IContainerBuilder builder)
        {
            builder.Register<PlayerPrefsSaveDataRepository>(Lifetime.Singleton)
                .As<ISaveDataRepository>();
            builder.Register<UnityroomRankingRegisterer>(Lifetime.Singleton)
                .As<IRankingRegisterer>();
        }
    }
}
