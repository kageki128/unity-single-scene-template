using System.Threading;
using Cysharp.Threading.Tasks;
using MyProject.Actor;

namespace MyProject.Director
{
    public class SelectSceneDirector : ISceneDirector
    {
        readonly SelectActorHub selectActorHub;

        public SelectSceneDirector(SelectActorHub selectActorHub)
        {
            this.selectActorHub = selectActorHub;
        }

        public void Initialize()
        {
            selectActorHub.Initialize();
        }

        public UniTask InitialEnterAsync(CancellationToken ct)
        {
            return selectActorHub.InitialShowAsync(ct);
        }

        public UniTask BeforeEnterAsync(CancellationToken ct)
        {
            return UniTask.CompletedTask;
        }

        public UniTask EnterAsync(CancellationToken ct)
        {
            return selectActorHub.ShowAsync(ct);
        }

        public void Tick()
        {
        }

        public UniTask BeforeExitAsync(CancellationToken ct)
        {
            return UniTask.CompletedTask;
        }

        public UniTask ExitAsync(CancellationToken ct)
        {
            return selectActorHub.HideAsync(ct);
        }
    }
}
