using System;
using System.Threading;
using R3;

namespace MyProject.Core
{
    public class SceneCore : IDisposable
    {
        public ReadOnlyReactiveProperty<SceneType> CurrentScene => currentScene;
        readonly ReactiveProperty<SceneType> currentScene = new();

        public Observable<SceneType> SceneReload => sceneReload;
        readonly Subject<SceneType> sceneReload = new();

        readonly SceneType initialScene;
        readonly SemaphoreSlim sceneChangeSemaphore = new(1, 1);

        public SceneCore(GameConfigSO gameConfig)
        {
            initialScene = gameConfig.InitialSceneType;
            currentScene.Value = initialScene;
        }

        public void Dispose()
        {
            currentScene.Dispose();
            sceneReload.Dispose();
            sceneChangeSemaphore.Dispose();
        }

        /// <summary>
        /// シーンチェンジをリクエストする。
        /// シーンチェンジ中は、外からFinishされるまでシーンチェンジ中と見なされロックされる。
        /// ロック中はリクエストは棄却される。
        /// 現在のシーンへのリクエストの場合はシーンがリロードされる。
        /// </summary>
        public void RequestSceneChange(SceneType sceneType)
        {
            // シーンチェンジ中ならリクエストを棄却
            if (!sceneChangeSemaphore.Wait(0))
            {
                throw new InvalidOperationException("Scene change was requested while another scene change is in progress.");
            }

            // 現在のシーンと同じならリロード
            if (currentScene.Value == sceneType)
            {
                sceneReload.OnNext(sceneType);
                return;
            }

            currentScene.Value = sceneType;
        }

        /// <summary>
        /// 進行中のシーンチェンジを完了する。
        /// ロックは解除される。
        /// </summary>
        public void FinishSceneChange()
        {
            // 進行中のシーンチェンジが無ければエラー
            if (sceneChangeSemaphore.CurrentCount == 1)
            {
                throw new InvalidOperationException("No scene change is in progress.");
            }

            sceneChangeSemaphore.Release();
        }
    }
}