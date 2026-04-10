using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using MyProject.Actor;

namespace MyProject.Director
{
    public class GameSceneDirector : ISceneDirector
    {
        readonly GameActorHub gameActorHub;

        public GameSceneDirector(GameActorHub gameActorHub)
        {
            this.gameActorHub = gameActorHub;
        }

        public void Initialize()
        {
            gameActorHub.Initialize();
        }

        public async UniTask InitialEnterAsync(CancellationToken ct)
        {
            await gameActorHub.InitialShowAsync(ct);
        }

        public async UniTask BeforeEnterAsync(CancellationToken ct)
        {
            await UniTask.CompletedTask;
        }

        public async UniTask EnterAsync(CancellationToken ct)
        {
            await gameActorHub.ShowAsync(ct);
        }

        public void Tick()
        {
        }

        public async UniTask BeforeExitAsync(CancellationToken ct)
        {
            await UniTask.CompletedTask;
        }

        public async UniTask ExitAsync(CancellationToken ct)
        {
            await gameActorHub.HideAsync(ct);
        }
    }
}
