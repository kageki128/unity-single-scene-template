using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MyProject.Actor;
using R3;

namespace MyProject.Director
{
    public class ResultSceneDirector : ISceneDirector, IDisposable
    {
        readonly ResultActorHub resultActorHub;

        readonly CompositeDisposable disposables = new();

        public ResultSceneDirector(ResultActorHub resultActorHub)
        {
            this.resultActorHub = resultActorHub;
        }

        public void Initialize()
        {
            resultActorHub.Initialize();
        }

        public async UniTask BeforeEnterAsync(CancellationToken ct)
        {
            await UniTask.CompletedTask;
        }

        public async UniTask InitialEnterAsync(CancellationToken ct)
        {
            await resultActorHub.InitialShowAsync(ct);
            HandleEnter();
        }
        
        public async UniTask EnterAsync(CancellationToken ct)
        {
            await resultActorHub.ShowAsync(ct);
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
            await resultActorHub.HideAsync(ct);
        }

        public void Dispose()
        {
            disposables.Dispose();
        }

        void HandleEnter()
        {
            disposables.Clear();
        }
    }
}
