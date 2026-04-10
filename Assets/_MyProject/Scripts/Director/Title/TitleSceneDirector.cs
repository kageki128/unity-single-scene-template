using System.Threading;
using Cysharp.Threading.Tasks;
using MyProject.Actor;

namespace MyProject.Director
{
    public class TitleSceneDirector : ISceneDirector
    {
        readonly TitleActorHub titleActorHub;

        public TitleSceneDirector(TitleActorHub titleActorHub)
        {
            this.titleActorHub = titleActorHub;
        }

        public void Initialize()
        {
            titleActorHub.Initialize();
        }

        public UniTask InitialEnterAsync(CancellationToken ct)
        {
            return titleActorHub.InitialShowAsync(ct);
        }

        public UniTask BeforeEnterAsync(CancellationToken ct)
        {
            return UniTask.CompletedTask;
        }

        public UniTask EnterAsync(CancellationToken ct)
        {
            return titleActorHub.ShowAsync(ct);
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
            return titleActorHub.HideAsync(ct);
        }
    }
}
