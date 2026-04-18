# R3 Project Usage

このプロジェクト（`single-scene-template`）における、R3実装パターンの要点。

## 1. Core層の状態公開パターン

- `ReactiveProperty<T>` を内部に持ち、公開は `ReadOnlyReactiveProperty<T>` に限定している。
- 例:
  - `SceneCore.CurrentScene`
  - `GameSessionCore.State`
  - `ScoreCore.Value`
  - `AudioPlayer.BgmVolume` / `AudioPlayer.SeVolume`

対象:
- `Assets/_MyProject/Scripts/Core/Scene/SceneCore.cs`
- `Assets/_MyProject/Scripts/Core/GameSession/GameSessionCore.cs`
- `Assets/_MyProject/Scripts/Core/GameSession/ScoreCore.cs`
- `Assets/_MyProject/Scripts/Actor/Standard/AudioPlayer.cs`

## 2. Core/Actorのイベント通知パターン

- 外部公開は `Observable<T>` にし、内部は `Subject<T>` から `OnNext` している。
- 例:
  - `SceneCore.SceneReload`
  - `PointerClickObserver.Clicked`
  - `PointerEventObserver` の各ポインターイベント
  - `StandardSliderActor.ValueChanged`

対象:
- `Assets/_MyProject/Scripts/Core/Scene/SceneCore.cs`
- `Assets/_MyProject/Scripts/Actor/HelperComponent/Event/PointerClickObserver.cs`
- `Assets/_MyProject/Scripts/Actor/HelperComponent/Event/PointerEventObserver.cs`
- `Assets/_MyProject/Scripts/Actor/Standard/StandardSliderActor.cs`

## 3. 購読寿命管理パターン（Director）

- Directorは `CompositeDisposable` をフィールドで保持し、シーン入退場で `Clear()` して再購読している。
- `Dispose()` で最終解放する形を統一している。
- 単発イベントには `Take(1)` を使って多重遷移を防いでいる。

対象:
- `Assets/_MyProject/Scripts/Director/MainEntryPoint.cs`
- `Assets/_MyProject/Scripts/Director/Game/GameSceneDirector.cs`
- `Assets/_MyProject/Scripts/Director/Title/TitleSceneDirector.cs`
- `Assets/_MyProject/Scripts/Director/Select/SelectSceneDirector.cs`
- `Assets/_MyProject/Scripts/Director/Result/ResultSceneDirector.cs`

## 4. 双方向バインドに近い接続（Actor Hub）

- UI入力（Sliderイベント）を `Subscribe` でドメイン更新へ流し、同時にドメイン状態購読でUIへ反映している。
- 双方向の両側を同一 `CompositeDisposable` に集約して寿命管理している。

対象:
- `Assets/_MyProject/Scripts/Actor/RootActorHub.cs`

## 5. 破棄時の終端処理

- `Subject<T>` を持つ `MonoBehaviour` は `OnDestroy()` で `OnCompleted()` と `Dispose()` を明示的に呼んでいる。
- これにより下流購読の終了契約が明確になる。

対象:
- `Assets/_MyProject/Scripts/Actor/HelperComponent/Event/PointerClickObserver.cs`
- `Assets/_MyProject/Scripts/Actor/HelperComponent/Event/PointerEventObserver.cs`
- `Assets/_MyProject/Scripts/Actor/Standard/StandardSliderActor.cs`

## 6. 現状の補足

- `ObservableCollections.R3` は依存に含まれるが、`Assets/_MyProject/Scripts` 配下では現時点で未使用。

## 7. 導入時の推奨パターン（このプロジェクト向け）

- Coreで `ObservableList<T>` などを private で保持し、公開は `IObservableCollection<T>` に限定する。
- Directorで `ObserveAdd` / `ObserveRemove` を購読し、画面遷移ごとに `CompositeDisposable` を `Clear()` する。
- Actorで要素Viewを持つ場合は `CreateView` で変換し、`OnDestroy()` で `Dispose()` してリークを防ぐ。
- 一覧UIが範囲更新を使う可能性がある場合、`ToNotifyCollectionChangedSlim()` ではなく通常の `ToNotifyCollectionChanged()` を優先する。
