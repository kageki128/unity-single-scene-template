using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MyProject.Actor;
using R3;

namespace MyProject.Director
{
    public class SelectSceneDirector : ISceneDirector, IDisposable
    {
        readonly SelectActorHub selectActorHub;

        readonly CompositeDisposable disposables = new();

        public SelectSceneDirector(SelectActorHub selectActorHub)
        {
            this.selectActorHub = selectActorHub;
        }

        public void Initialize()
        {
            selectActorHub.Initialize();
        }

        public async UniTask BeforeEnterAsync(CancellationToken ct)
        {
            await UniTask.CompletedTask;
        }

        public async UniTask InitialEnterAsync(CancellationToken ct)
        {
            await selectActorHub.InitialShowAsync(ct);
            HandleEnter();
        }

        public async UniTask EnterAsync(CancellationToken ct)
        {
            await selectActorHub.ShowAsync(ct);
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
            await selectActorHub.HideAsync(ct);
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
