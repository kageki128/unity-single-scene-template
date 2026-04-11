# Project Actor Integration

## 既存アーキテクチャでの責務分担

- `Director`
  - 画面遷移をオーケストレーションする。
  - Actorの `InitialShowAsync/ShowAsync/HideAsync` を呼ぶだけに留める。
- `SceneActorHubBase`
  - 表示状態 (`SetActive`) と `ActorAnimationTimeline` 連携を担当する。
- `ActorAnimationTimeline`
  - 複数Actorの開始タイミング管理を担当する。
  - 各Actor内部のTween詳細は持たない。
- 各 `ActorBase` 派生
  - 実際のTween設定と演出ロジックを保持する。
  - `SerializableMotionSettings` を `SerializeField` で持つ。

## Tween設定を置く場所の指針

- 原則: 各Actorクラスに置く。
  - 理由: 見た目とパラメータの責務が一致するため。
- `ActorAnimationTimeline` には開始秒のみ置く。
  - 理由: スケジューリング責務に集中させるため。

## 実装時の流れ

1. 対象Actorに `SerializableMotionSettings` フィールドを追加する。
2. 既定値を `Ease.OutCubic` で初期化する。
3. `InitialShowAsync/ShowAsync/HideAsync` を `LMotion.Create(settings).BindTo...().ToUniTask(ct)` で実装する。
4. 複数Tweenが必要なら、同時は `UniTask.WhenAll`、順次は `LSequence` を使う。
5. `SceneActorHubBase` と `ActorAnimationTimeline` の責務を侵食しないか確認する。

## チェックリスト

- Actor側のTweenがInspector編集可能か。
- 既定Easeが `OutCubic` になっているか。
- `CancellationToken` が待機に渡されているか。
- Directorに演出ロジックを書いていないか。
