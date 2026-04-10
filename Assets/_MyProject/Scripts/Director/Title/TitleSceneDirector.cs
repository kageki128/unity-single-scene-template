using System.Threading;
using Cysharp.Threading.Tasks;

namespace MyProject.Director
{
    public class TitleSceneDirector : ISceneDirector
    {
        public UniTask InitializeAsync(CancellationToken cts)
        {
            return UniTask.CompletedTask;
        }

        public UniTask BeforeEnterAsync(CancellationToken cts)
        {
            return UniTask.CompletedTask;
        }

        public UniTask EnterAsync(CancellationToken cts)
        {
            return UniTask.CompletedTask;
        }

        public UniTask InitialEnterAsync(CancellationToken cts)
        {
            return UniTask.CompletedTask;
        }

        public void Tick()
        {
        }

        public UniTask BeforeExitAsync(CancellationToken cts)
        {
            return UniTask.CompletedTask;
        }

        public UniTask ExitAsync(CancellationToken cts)
        {
            return UniTask.CompletedTask;
        }
    }
}
