using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MyProject.View;
using MyProject.Model;
using R3;

namespace MyProject.Director
{
    public class GameDirector : ISceneDirector, IDisposable
    {
        public Observable<SceneType> SceneChangeRequest => sceneChangeRequest;
        readonly Subject<SceneType> sceneChangeRequest = new();

        public Observable<Unit> SceneReloadRequest => sceneReloadRequest;
        readonly Subject<Unit> sceneReloadRequest = new();

        readonly GameViewHub gameViewHub;

        readonly CompositeDisposable disposables = new();

        public GameDirector(GameViewHub gameViewHub)
        {
            this.gameViewHub = gameViewHub;
        }

        public async UniTask InitializeAsync(CancellationToken ct)
        {
            gameViewHub.Initialize();
            await UniTask.CompletedTask;
        }

        public async UniTask BeforeEnterAsync(CancellationToken ct)
        {
            await UniTask.CompletedTask;
        }

        public async UniTask EnterAsync(CancellationToken ct)
        {
            await gameViewHub.ShowAsync(ct);
        }

        public async UniTask AfterEnterAsync(CancellationToken ct)
        {
            disposables.Clear();
            gameViewHub.ToSelectButtonClicked
                .Take(1)
                .Subscribe(_ => sceneChangeRequest.OnNext(SceneType.Select))
                .AddTo(disposables);
            await UniTask.CompletedTask;
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
            await gameViewHub.HideAsync(ct);
        }

        public void Dispose()
        {
            disposables.Dispose();
            sceneChangeRequest.Dispose();
            sceneReloadRequest.Dispose();
        }

    }
}
