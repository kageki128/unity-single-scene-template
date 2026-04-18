# UniTask Project Usage

このプロジェクト（`single-scene-template`）における、UniTask実装パターンの要点。

## 1. Director層でのルート制御

- `MainEntryPoint` は `IAsyncStartable.StartAsync(CancellationToken ct)` を起点に初期化を実行する。
- シーン遷移は `HandleSceneTransitionAsync(...).Forget()` でイベント境界から起動している。
- 遷移内部では `UniTask.WhenAll(...)` で Exit/Enter を並列実行している。

対象:
- `Assets/_MyProject/Scripts/Director/MainEntryPoint.cs`

## 2. Actor層での並列実行と待機

- `RootActorHub` は複数Actorの `InitialShowAsync(ct)` を `UniTask.WhenAll` で同時実行している。
- `ActorAnimationTimeline` は `UniTask.Delay(..., cancellationToken: ct)` と `WhenAll` を組み合わせ、タイムライン再生を合成している。

対象:
- `Assets/_MyProject/Scripts/Actor/RootActorHub.cs`
- `Assets/_MyProject/Scripts/Actor/HelperComponent/Animation/ActorAnimationTimeline.cs`

## 3. Infrastructure層での外部非同期ラップ

- Addressablesの `AsyncOperationHandle` は `.ToUniTask(cancellationToken: ct)` へ統一している。
- 入口で `ct.ThrowIfCancellationRequested()` を呼び、不要処理を早期停止している。

対象:
- `Assets/_MyProject/Scripts/Infrastructure/Utils/AddressableLoader.cs`

## 4. 同期処理をUniTask契約に合わせる

- 非同期I/Fに対し同期完了パスは `UniTask.CompletedTask` / `UniTask.FromResult(...)` で返している。

対象:
- `Assets/_MyProject/Scripts/Infrastructure/SaveData/PlayerPrefs/PlayerPrefsSaveDataRepository.cs`
- `Assets/_MyProject/Scripts/Infrastructure/Ranking/Unityroom/UnityroomRankingRegisterer.cs`

## 5. 今後の統一ルール案

- `Forget()` はイベント境界でのみ使用し、業務フロー内では `await` 優先にする。
- `CancellationToken` の命名を `ct` で統一し、必ず末尾引数に置く。
- `async` だが `await` 実質不要なメソッドは、`async` を外して `UniTask.CompletedTask` 返却へ簡略化する。

