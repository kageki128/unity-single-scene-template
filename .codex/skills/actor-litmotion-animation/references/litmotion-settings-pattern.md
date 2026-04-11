# LitMotion Settings Pattern

## なぜ `SerializableMotionSettings` を使うか

- `SerializableMotionSettings<TValue, TOptions>` はInspector編集可能な `MotionSettings`。
- ActorごとのTweenパラメータをコードから分離し、演出調整をノーコードで進められる。
- `LMotion.Create(settings)` で `StartValue/EndValue/Duration/Ease/Delay/Loops/LoopType/Scheduler` を一括適用できる。

## OutCubic を既定値にする実装パターン

### パターンA: フィールド初期化で既定値を注入する

```csharp
[SerializeField] SerializableMotionSettings<float, NoOptions> showScale = new()
{
    StartValue = 0f,
    EndValue = 1f,
    Duration = 0.2f,
    Ease = Ease.OutCubic,
    DelayType = DelayType.FirstLoop,
    LoopType = LoopType.Restart,
    Loops = 1,
    SkipValuesDuringDelay = true,
    ImmediateBind = true
};
```

この方式を標準とする。  
理由: 既定値が明示され、Inspectorで上書き可能だから。

### パターンB: `Reset` で既定値を再注入する

```csharp
void Reset()
{
    showScale = showScale with { Ease = Ease.OutCubic };
}
```

`Reset` は「初期値再構築」に使う。  
実行時に常時上書きするとInspector編集の意味がなくなるため、`OnValidate`で毎回強制しない。

## `ToUniTask` のキャンセル挙動

`ToUniTask` は `CancellationToken` を受け取れる。標準は `ToUniTask(ct)` を使う。

- `await handle.ToUniTask(ct)`
  - tokenキャンセル時: Motionを`Cancel`して待機をキャンセルする。
- `await handle.ToUniTask(CancelBehavior.Complete, ct)`
  - tokenキャンセル時: Motionを`Complete`して待機をキャンセルする。
- `await handle.ToUniTask(CancelBehavior.Cancel, false, ct)`
  - Motion側が`Cancel()`されても待機をキャンセルしない。

Actorの `Show/Hide` は通常 `ToUniTask(ct)` を使う。  
画面遷移中断時に不要なTweenを引きずらないため。

## 再入時のリスタート再生パターン

「再生メソッドが再び呼ばれたら、中断して先頭から再生」が必要な場合は、`MotionHandle` をフィールドで保持して差し替える。

```csharp
MotionHandle showHandle;

UniTask PlayShowAsync(CancellationToken ct)
{
    showHandle.TryCancel();
    showHandle = LMotion.Create(showSettings)
        .BindToLocalScaleXYZ(transform)
        .AddTo(this);

    return showHandle.ToUniTask(CancelBehavior.Cancel, false, ct);
}
```

- `TryCancel()` で前回再生を止める。
- 新しいHandleを再代入して再生し直す。
- `cancelAwaitOnMotionCanceled` に `false` を渡すことで、再入による`Cancel`を例外扱いしない。

## Sequence運用メモ

- `LSequence` に追加できるのは「未再生Handle」のみ。
- 再生中Handleや無限ループTweenを追加すると例外になる。
- 順次演出が必要な場合だけ `LSequence` を使い、単純な同時再生は `UniTask.WhenAll` を優先する。
