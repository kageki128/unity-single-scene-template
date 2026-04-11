using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MyProject.Actor;
using MyProject.Core;
using R3;

namespace MyProject.Director
{
    public class GameSceneDirector : ISceneDirector, IDisposable
    {
        readonly SceneCore sceneCore;
        readonly GameActorHub gameActorHub;

        readonly CompositeDisposable disposables = new();

        public GameSceneDirector
        (
            SceneCore sceneCore,
            GameActorHub gameActorHub
        )
        {
            this.sceneCore = sceneCore;
            this.gameActorHub = gameActorHub;
        }

        public void Initialize()
        {
            gameActorHub.Initialize();
        }

        public async UniTask BeforeEnterAsync(CancellationToken ct)
        {
            await UniTask.CompletedTask;
        }

        public async UniTask InitialEnterAsync(CancellationToken ct)
        {
            await gameActorHub.InitialShowAsync(ct);
            HandleEnter();
        }

        public async UniTask EnterAsync(CancellationToken ct)
        {
            await gameActorHub.ShowAsync(ct);
            HandleEnter();
        }

        public void Tick()
        {
        }

        public async UniTask BeforeExitAsync(CancellationToken ct)
        {
            disposables.Clear();
            await UniTask.CompletedTask;
        }

        public async UniTask ExitAsync(CancellationToken ct)
        {
            await gameActorHub.HideAsync(ct);
        }

        public void Dispose()
        {
            disposables.Dispose();
        }

        void HandleEnter()
        {
            disposables.Clear();
            gameActorHub.ToSelectButtonClicked
                .Take(1)
                .Subscribe(_ => sceneCore.RequestSceneChange(SceneType.Select))
                .AddTo(disposables);
        }
    }
}
