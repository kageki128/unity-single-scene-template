using UnityEngine;

namespace MyProject.View
{
    /// <summary>
    /// RootViewの基底クラス。
    /// ゲーム中常に存在するViewがこのクラスを継承する。
    /// </summary>
    public abstract class RootViewBase : MonoBehaviour
    {
        /// <summary>
        /// 初期化処理
        /// </summary>
        public abstract void Initialize();
    }
}
