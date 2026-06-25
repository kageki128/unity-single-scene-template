using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MyProject.Actor;
using MyProject.Core;
using R3;

namespace MyProject.Director
{
    public class ResultDirector : ISceneDirector, IDisposable
    {
        public Observable<SceneType> SceneChangeRequest => sceneChangeRequest;
        readonly Subject<SceneType> sceneChangeRequest = new();

        public Observable<Unit> SceneReloadRequest => sceneReloadRequest;
        readonly Subject<Unit> sceneReloadRequest = new();

        readonly ResultActorHub resultActorHub;

        readonly CompositeDisposable disposables = new();

        public ResultDirector(ResultActorHub resultActorHub)
        {
            this.resultActorHub = resultActorHub;
        }

        public async UniTask InitializeAsync(CancellationToken ct)
        {
            resultActorHub.Initialize();
            await UniTask.CompletedTask;
        }

        public async UniTask BeforeEnterAsync(CancellationToken ct)
        {
            await UniTask.CompletedTask;
        }

        public async UniTask EnterAsync(CancellationToken ct)
        {
            await resultActorHub.ShowAsync(ct);
        }

        public async UniTask AfterEnterAsync(CancellationToken ct)
        {
            disposables.Clear();
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
            await resultActorHub.HideAsync(ct);
        }

        public void Dispose()
        {
            disposables.Dispose();
            sceneChangeRequest.Dispose();
            sceneReloadRequest.Dispose();
        }

    }
}
