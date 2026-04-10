using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MyProject.Actor
{
    /// <summary>
    /// Actorの基底クラス。
    /// 全てのアクターは基本的にこのクラスを継承する。
    /// </summary>
    public abstract class ActorBase : MonoBehaviour
    {
        /// <summary>
        /// 初期化処理を行う。
        /// </summary>
        public virtual void Initialize()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// ゲームの最初のシーンとして表示されるときの表示処理。
        /// 多くの場合、すぐに表示されることが望ましいだろう。
        /// </summary>
        public virtual UniTask InitialShowAsync(CancellationToken ct)
        {
            gameObject.SetActive(true);
            return UniTask.CompletedTask;
        }

        /// <summary>
        /// 表示処理。
        /// </summary>
        public virtual UniTask ShowAsync(CancellationToken ct)
        {
            gameObject.SetActive(true);
            return UniTask.CompletedTask;
        }

        /// <summary>
        /// 非表示処理。
        /// </summary>
        public virtual UniTask HideAsync(CancellationToken ct)
        {
            gameObject.SetActive(false);
            return UniTask.CompletedTask;
        }
    }
}
