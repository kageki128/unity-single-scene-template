using System.Threading;
using Cysharp.Threading.Tasks;

namespace MyProject.View
{
    public abstract class SceneViewHubBase : ViewBase
    {
        public abstract override void Initialize();
        public abstract override UniTask ShowAsync(CancellationToken ct);
        public abstract override UniTask HideAsync(CancellationToken ct);
    }
}
