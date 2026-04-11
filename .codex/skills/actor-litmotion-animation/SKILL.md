---
name: actor-litmotion-animation
description: UnityプロジェクトでActorBase派生クラスの表示/非表示アニメーションをLitMotionで実装・改修するためのskill。Director→SceneActorHubBase→ActorAnimationTimeline構造のプロジェクトで、Inspector編集可能なSerializableMotionSettingsを使い、Ease.OutCubicを既定値としてInitialShowAsync/ShowAsync/HideAsyncを実装するときに使用する。
---

# Actor LitMotion Animation

## Overview

ActorアニメーションをLitMotionで一貫実装する。  
デフォルトイージングを`Ease.OutCubic`に固定しつつ、アニメーション構成パラメータをInspectorから調整可能な形で実装する。

## Workflow

1. Actorの責務を確認する。
- `Director`はオーケストレーションのみ担当する。
- `SceneActorHubBase`は表示/非表示の入口を担当する。
- 個々のActorが実際のTween設定を持つ。

2. Tween設定を`SerializeField`で定義する。
- `SerializableMotionSettings<TValue, TOptions>`を使う。
- `StartValue/EndValue/Duration/Ease/Delay/Loops/LoopType/Scheduler`をInspectorで編集可能にする。
- 既定値は`Ease.OutCubic`にする。

3. `InitialShowAsync/ShowAsync/HideAsync`を実装する。
- `LMotion.Create(settings).BindTo...().ToUniTask(ct)`を標準形にする。
- `CancellationToken ct`を必須で受け、待機に渡す。

4. 複数Tweenの構成方法を選ぶ。
- 同時再生は`UniTask.WhenAll`を使う。
- 順次再生は`LSequence.Create()`を使う。

5. 最後にセルフチェックする。
- OutCubic既定になっているか。
- Inspector編集可能な設計か。
- `ToUniTask(ct)`でキャンセル連携できているか。

## Implementation Rules

- ActorのTween設定は`SerializableMotionSettings<TValue, TOptions>`を基本採用する。
- `Ease`未指定のまま使わず、既定値を`Ease.OutCubic`に設定する。
- `InitialShowAsync/ShowAsync/HideAsync`は`LMotion.Create(settings).BindTo...().ToUniTask(ct)`で実装する。
- メソッド再入時は「現在実行中の同種Tweenを`TryCancel()`で中断し、先頭から再生し直す」を基本挙動にする。
- `CancellationToken`を渡せない待機処理を新規導入しない。
- 同時再生は`UniTask.WhenAll`、順次再生は`LSequence`を使い分ける。
- Unityのビルド作業、csproj作業、meta生成は行わない。

## Actor Template (Inspector編集 + OutCubic既定)

```csharp
using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;

namespace MyProject.Actor
{
    public sealed class ExamplePopupActor : ActorBase
    {
        MotionHandle showHandle;
        MotionHandle hideHandle;

        [Header("Show")]
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

        [Header("Hide")]
        [SerializeField] SerializableMotionSettings<float, NoOptions> hideScale = new()
        {
            StartValue = 1f,
            EndValue = 0f,
            Duration = 0.12f,
            Ease = Ease.OutCubic,
            DelayType = DelayType.FirstLoop,
            LoopType = LoopType.Restart,
            Loops = 1,
            SkipValuesDuringDelay = true,
            ImmediateBind = true
        };

        void Reset()
        {
            showScale = showScale with { Ease = Ease.OutCubic };
            hideScale = hideScale with { Ease = Ease.OutCubic };
        }

        public override UniTask InitialShowAsync(CancellationToken ct)
        {
            gameObject.SetActive(true);
            return PlayShowAsync(ct);
        }

        public override UniTask ShowAsync(CancellationToken ct)
        {
            gameObject.SetActive(true);
            return PlayShowAsync(ct);
        }

        public override async UniTask HideAsync(CancellationToken ct)
        {
            hideHandle.TryCancel();
            hideHandle = LMotion.Create(hideScale)
                .BindToLocalScaleXYZ(transform)
                .AddTo(this);

            await hideHandle.ToUniTask(CancelBehavior.Cancel, false, ct);
            gameObject.SetActive(false);
        }

        UniTask PlayShowAsync(CancellationToken ct)
        {
            showHandle.TryCancel();
            showHandle = LMotion.Create(showScale)
                .BindToLocalScaleXYZ(transform)
                .AddTo(this);

            return showHandle.ToUniTask(CancelBehavior.Cancel, false, ct);
        }
    }
}
```

## Multi Motion Patterns

### Parallel (`UniTask.WhenAll`)

```csharp
await UniTask.WhenAll(
    LMotion.Create(fadeInSettings)
        .BindToCanvasGroupAlpha(canvasGroup)
        .ToUniTask(ct),
    LMotion.Create(moveUpSettings)
        .BindToLocalPositionY(transform)
        .ToUniTask(ct)
);
```

### Sequential (`LSequence`)

```csharp
await LSequence.Create()
    .Append(LMotion.Create(showScaleSettings).BindToLocalScaleXYZ(transform))
    .Append(LMotion.Create(fadeInSettings).BindToCanvasGroupAlpha(canvasGroup))
    .Run()
    .ToUniTask(ct);
```

`LSequence`には「再生中Handle」「無限ループTween」を追加しない。例外が発生する。

## Inspectorで調整するパラメータ

- `StartValue`
- `EndValue`
- `Duration`
- `Ease` (既定: `OutCubic`)
- `Delay`
- `DelayType`
- `Loops`
- `LoopType`
- `Scheduler`

## 禁止パターン

- Inspector編集可能にすべき値を、メソッド内ハードコードのみで固定する。
- `Ease`を未指定のままにしてLinear既定を流入させる。
- `ToUniTask(ct)`を使わず、`CancellationToken`を待機に連携しない。
- 再入しうる再生メソッドで、既存Tweenを止めずに重ねて再生する。

## Quick Prompt Checks

- `ActorのShowを作る`
  期待: `SerializableMotionSettings` + `Ease.OutCubic` + `ToUniTask(ct)`を含む。
- `Hideを作る`
  期待: 非表示前にTween待機し、完了後`SetActive(false)`する。
- `複数Actorを順次表示する`
  期待: `LSequence`か`ActorAnimationTimeline`前提の順次構成を示す。
- `再生中にShowが再び呼ばれた`
  期待: 既存Tweenを`TryCancel()`して、先頭から再生し直す。

## References

- 詳細は`references/litmotion-settings-pattern.md`を参照する。
- プロジェクト統合方針は`references/project-actor-integration.md`を参照する。
