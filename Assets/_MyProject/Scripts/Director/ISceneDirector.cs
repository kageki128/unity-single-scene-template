using System.Threading;
using Cysharp.Threading.Tasks;

namespace MyProject.Director
{
    public interface ISceneDirector
    {
        /// <summary>
        /// シーンの初期化処理を行う。
        /// </summary>
        UniTask InitializeAsync(CancellationToken cts);
        /// <summary>
        /// 全シーン初期化後、最初のシーンに遷移するときに呼び出される。
        /// アニメーションはせず、すぐにこのシーンが始まるようにする。
        /// </summary>
        UniTask InitialEnterAsync(CancellationToken cts);
        /// <summary>
        /// シーンチェンジでこのシーンに遷移する前に呼び出される。
        /// 遷移元のBeforeExitAsyncの後に呼び出される。
        /// </summary>
        UniTask BeforeEnterAsync(CancellationToken cts);
        /// <summary>
        /// シーンチェンジでこのシーンに遷移するときに呼び出される。
        /// 遷移元のExitAsyncと同時に呼び出される。
        /// </summary>
        UniTask EnterAsync(CancellationToken cts);
        /// <summary>
        /// このシーンが選ばれている間、毎フレーム呼び出される。
        /// </summary>
        void Tick();
        /// <summary>
        /// シーンチェンジでこのシーンから遷移する前に呼び出される。
        /// 遷移先のBeforeEnterAsyncよりも先に呼び出される。
        /// </summary>
        UniTask BeforeExitAsync(CancellationToken cts);
        /// <summary>
        /// シーンチェンジでこのシーンから遷移するときに呼び出される。
        /// 遷移先のEnterAsyncと同時に呼び出される。
        /// </summary>
        UniTask ExitAsync(CancellationToken cts);
    }
}
