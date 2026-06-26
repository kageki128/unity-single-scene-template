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

        readonly GameSessionModel gameSessionModel;
        readonly GameViewHub gameViewHub;

        readonly CompositeDisposable disposables = new();
        readonly CancellationTokenSource cts = new();

        public GameDirector(GameSessionModel gameSessionModel, GameViewHub gameViewHub)
        {
            this.gameSessionModel = gameSessionModel;
            this.gameViewHub = gameViewHub;
        }

        public async UniTask InitializeAsync(CancellationToken ct)
        {
            gameSessionModel.Initialize();
            gameViewHub.Initialize();
            await UniTask.CompletedTask;
        }

        public async UniTask BeforeEnterAsync(CancellationToken ct)
        {
            gameSessionModel.Initialize();
            SubscribeModel();
            await UniTask.CompletedTask;
        }

        public async UniTask EnterAsync(CancellationToken ct)
        {
            await gameViewHub.ShowAsync(ct);
        }

        public async UniTask AfterEnterAsync(CancellationToken ct)
        {
            disposables.Clear();
            SubscribeView();
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
            cts.Cancel();
            cts.Dispose();
            disposables.Dispose();
            sceneChangeRequest.OnCompleted();
            sceneChangeRequest.Dispose();
            sceneReloadRequest.OnCompleted();
            sceneReloadRequest.Dispose();
        }

        void SubscribeModel()
        {
            gameSessionModel.Finished
                .Take(1)
                .Subscribe(_ => HandleGameFinishedAsync(cts.Token).Forget())
                .AddTo(disposables);
        }

        void SubscribeView()
        {
            gameViewHub.ToSelectButtonClicked
                .Take(1)
                .Subscribe(_ => sceneChangeRequest.OnNext(SceneType.Select))
                .AddTo(disposables);
        }

        async UniTask HandleGameFinishedAsync(CancellationToken ct)
        {
            await gameSessionModel.SaveAsync(ct);
            sceneChangeRequest.OnNext(SceneType.Result);
        }
    }
}
