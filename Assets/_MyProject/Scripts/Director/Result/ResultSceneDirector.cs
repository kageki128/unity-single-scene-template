using System.Threading;
using Cysharp.Threading.Tasks;
using MyProject.Actor;

namespace MyProject.Director
{
    public class ResultSceneDirector : ISceneDirector
    {
        readonly ResultActorHub resultActorHub;

        public ResultSceneDirector(ResultActorHub resultActorHub)
        {
            this.resultActorHub = resultActorHub;
        }

        public void Initialize()
        {
            resultActorHub.Initialize();
        }

        public UniTask InitialEnterAsync(CancellationToken ct)
        {
            return resultActorHub.InitialShowAsync(ct);
        }

        public UniTask BeforeEnterAsync(CancellationToken ct)
        {
            return UniTask.CompletedTask;
        }

        public UniTask EnterAsync(CancellationToken ct)
        {
            return resultActorHub.ShowAsync(ct);
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
            return resultActorHub.HideAsync(ct);
        }
    }
}
