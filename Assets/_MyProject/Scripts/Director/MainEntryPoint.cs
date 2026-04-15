using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using MyProject.Core;
using R3;
using VContainer.Unity;

namespace MyProject.Director
{
    public class MainEntryPoint : IAsyncStartable, ITickable, IDisposable
    {
        readonly SceneCore sceneCore;

        readonly RootDirector rootDirector;
        readonly Dictionary<SceneType, ISceneDirector> sceneDirectors = new();

        readonly CompositeDisposable disposables = new();
        readonly CancellationTokenSource cts = new();

        public MainEntryPoint
        (
            SceneCore sceneCore,
            RootDirector rootDirector,
            TitleSceneDirector titleSceneDirector,
            SelectSceneDirector selectSceneDirector,
            GameSceneDirector gameSceneDirector,
            ResultSceneDirector resultSceneDirector
        )
        {
            this.sceneCore = sceneCore;
            this.rootDirector = rootDirector;
            sceneDirectors.Add(SceneType.Title, titleSceneDirector);
            sceneDirectors.Add(SceneType.Select, selectSceneDirector);
            sceneDirectors.Add(SceneType.Game, gameSceneDirector);
            sceneDirectors.Add(SceneType.Result, resultSceneDirector);
        }

        public async UniTask StartAsync(CancellationToken ct)
        {
            await rootDirector.InitializeAsync(ct);
            await ResetSceneAsync(ct);
        }

        public void Tick()
        {
            rootDirector.Tick();

            var currentScene = sceneCore.CurrentScene.CurrentValue;
            var director = GetSceneDirector(currentScene);
            director.Tick();
        }

        public void Dispose()
        {
            disposables.Dispose();
            cts.Cancel();
            cts.Dispose();
        }

        async UniTask ResetSceneAsync(CancellationToken ct)
        {
            disposables.Clear();

            foreach (var director in sceneDirectors.Values)
            {
                await director.InitializeAsync(ct);
            }

            sceneCore.CurrentScene.Pairwise().Subscribe(pair =>
            {
                var (from, to) = pair;
                HandleSceneTransitionAsync(from, to).Forget();
            }).AddTo(disposables);

            sceneCore.SceneReload.Subscribe(scene =>
            {
                HandleSceneTransitionAsync(scene, scene).Forget();
            }).AddTo(disposables);

            // 初期シーンを起動
            var initialScene = sceneCore.CurrentScene.CurrentValue;
            var initialSceneDirector = GetSceneDirector(initialScene);
            await initialSceneDirector.BeforeEnterAsync(ct);
            await initialSceneDirector.InitialEnterAsync(ct);
        }

        async UniTask HandleSceneTransitionAsync(SceneType from, SceneType to)
        {
            var fromDirector = GetSceneDirector(from);
            var toDirector = GetSceneDirector(to);

            try
            {
                await fromDirector.BeforeExitAsync(cts.Token);
                await toDirector.BeforeEnterAsync(cts.Token);
                await UniTask.WhenAll
                (
                    fromDirector.ExitAsync(cts.Token),
                    toDirector.EnterAsync(cts.Token)
                );
            }
            catch (Exception ex)
            {
                // シーンを初期化
                await ResetSceneAsync(cts.Token);

                throw new InvalidOperationException($"Scene transition failed from {from} to {to}. Scene has been reset.", ex);

            }
            finally
            {
                sceneCore.FinishSceneChange();
            }
        }

        ISceneDirector GetSceneDirector(SceneType sceneType)
        {
            if (sceneDirectors.TryGetValue(sceneType, out var director))
            {
                return director;
            }

            throw new InvalidOperationException($"SceneDirector not found for SceneType: {sceneType}");
        }
    }
}
