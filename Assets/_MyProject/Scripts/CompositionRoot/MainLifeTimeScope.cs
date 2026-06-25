using MyProject.View;
using MyProject.Model;
using MyProject.Director;
using MyProject.Infrastructure;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace MyProject.CompositionRoot
{
    public class MainLifeTimeScope : LifetimeScope
    {
        [Header("View")]
        [SerializeField] RootViewHub rootViewHub;
        [SerializeField] TitleViewHub titleViewHub;
        [SerializeField] SelectViewHub selectViewHub;
        [SerializeField] GameViewHub gameViewHub;
        [SerializeField] ResultViewHub resultViewHub;
        [Header("Config")]
        [SerializeField] GameConfigSO gameConfig;

        protected override void Configure(IContainerBuilder builder)
        {
            RegisterModel(builder);
            RegisterView(builder);
            RegisterDirector(builder);
            RegisterInfrastructure(builder);
        }

        void RegisterModel(IContainerBuilder builder)
        {
            builder.RegisterInstance(gameConfig);
        }

        void RegisterView(IContainerBuilder builder)
        {
            builder.RegisterComponent(rootViewHub);
            builder.RegisterComponent(titleViewHub);
            builder.RegisterComponent(selectViewHub);
            builder.RegisterComponent(gameViewHub);
            builder.RegisterComponent(resultViewHub);
        }

        void RegisterDirector(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<MainEntryPoint>(Lifetime.Singleton);
            builder.Register<RootDirector>(Lifetime.Singleton);
            builder.Register<TitleDirector>(Lifetime.Singleton);
            builder.Register<SelectDirector>(Lifetime.Singleton);
            builder.Register<GameDirector>(Lifetime.Singleton);
            builder.Register<ResultDirector>(Lifetime.Singleton);
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
