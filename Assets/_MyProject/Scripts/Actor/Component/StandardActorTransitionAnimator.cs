using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using UnityEngine;
using UnityEngine.UI;

namespace MyProject.Actor
{
    [DisallowMultipleComponent]
    public class StandardActorTransitionAnimator : MonoBehaviour
    {

        [Serializable]
        class MoveSettings
        {
            public float Distance => distance;
            [SerializeField, Min(0f)] float distance = 3f;

            public float AngleDegrees => angleDegrees;
            [SerializeField] float angleDegrees = 0f;

            public Ease Ease => ease;
            [SerializeField] Ease ease = Ease.OutCubic;
        }

        [Serializable]
        class FadeSettings
        {
            public bool IsFade => isFade;
            [SerializeField] bool isFade = true;

            public Ease Ease => ease;
            [SerializeField] Ease ease = Ease.InCubic;
        }

        [Serializable]
        class ScaleSettings
        {
            public float Multiplier => multiplier;
            [SerializeField, Min(0f)] float multiplier = 1f;

            public Ease Ease => ease;
            [SerializeField] Ease ease = Ease.InCubic;
        }

        [Serializable]
        class PhaseSettings
        {
            public float DurationSeconds => durationSeconds;
            [SerializeField, Min(0f)] float durationSeconds = 0.3f;

            public bool UseCanvasGroupForFade => useCanvasGroupForFade;
            [SerializeField] bool useCanvasGroupForFade = true;

            public MoveSettings Move => move;
            [SerializeField] MoveSettings move = new();

            public FadeSettings Fade => fade;
            [SerializeField] FadeSettings fade = new();

            public ScaleSettings Scale => scale;
            [SerializeField] ScaleSettings scale = new();
        }

        [Header("Initial Show")]
        [SerializeField] PhaseSettings initialShowSettings = new();

        [Header("Show")]
        [SerializeField] PhaseSettings showSettings = new();

        [Header("Hide")]
        [SerializeField] PhaseSettings hideSettings = new();

        FadeTarget selfCanvasGroupFadeTarget;
        readonly List<FadeTarget> childFadeTargets = new();

        RectTransform rectTransform;
        bool usesAnchoredPosition;
        Vector3 basePosition;
        Vector3 baseScale;

        MotionHandle moveHandle;
        MotionHandle fadeHandle;
        MotionHandle scaleHandle;

        public void Initialize()
        {
            var selfCanvasGroup = GetComponent<CanvasGroup>();
            selfCanvasGroupFadeTarget = CreateCanvasGroupFadeTarget(selfCanvasGroup);

            rectTransform = transform as RectTransform;
            usesAnchoredPosition = rectTransform != null;
            basePosition = usesAnchoredPosition ? rectTransform.anchoredPosition3D : transform.localPosition;
            baseScale = transform.localScale;

            CacheChildFadeTargets();
            CancelRunningMotions();
        }

        public UniTask InitialShowAsync(CancellationToken ct) =>
             PlayPhaseAsync(initialShowSettings, PhaseType.Show, ct);

        public UniTask ShowAsync(CancellationToken ct) =>
             PlayPhaseAsync(showSettings, PhaseType.Show, ct);

        public UniTask HideAsync(CancellationToken ct) =>
             PlayPhaseAsync(hideSettings, PhaseType.Hide, ct);

        

        UniTask PlayPhaseAsync(PhaseSettings settings, PhaseType phaseType, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            CancelRunningMotions();

            var duration = settings.DurationSeconds;
            var tasks = new List<UniTask>();

            moveHandle = CreateMoveMotion(settings.Move, phaseType, duration).AddTo(this);
            tasks.Add(moveHandle.ToUniTask(CancelBehavior.Cancel, false, ct));

            scaleHandle = CreateScaleMotion(settings.Scale, phaseType, duration).AddTo(this);
            tasks.Add(scaleHandle.ToUniTask(CancelBehavior.Cancel, false, ct));

            if (settings.Fade.IsFade)
            {
                bool useCanvasGroup = settings.UseCanvasGroupForFade && selfCanvasGroupFadeTarget.IsAlive();

                fadeHandle = CreateFadeMotion(useCanvasGroup, settings.Fade, phaseType, duration).AddTo(this);
                tasks.Add(fadeHandle.ToUniTask(CancelBehavior.Cancel, false, ct));
            }

            return UniTask.WhenAll(tasks);
        }

        MotionHandle CreateMoveMotion(MoveSettings settings, PhaseType phaseType, float duration)
        {
            var radian = settings.AngleDegrees * Mathf.Deg2Rad;
            var direction = new Vector3(Mathf.Cos(radian), Mathf.Sin(radian), 0f);
            var offset = direction * settings.Distance;

            var from = phaseType == PhaseType.Hide ? basePosition : basePosition + offset;
            var to = phaseType == PhaseType.Hide ? basePosition + offset : basePosition;

            return LMotion.Create(from, to, duration)
                .WithEase(settings.Ease)
                .Bind(SetCurrentPosition);
        }

        void SetCurrentPosition(Vector3 value)
        {
            if (usesAnchoredPosition)
            {
                rectTransform.anchoredPosition3D = value;
                return;
            }

            transform.localPosition = value;
        }

        MotionHandle CreateFadeMotion(bool useCanvasGroup, FadeSettings settings, PhaseType phaseType, float duration)
        {
            var fadeTargets = useCanvasGroup ? new List<FadeTarget> { selfCanvasGroupFadeTarget } : childFadeTargets;
            return LMotion.Create(0f, 1f, duration)
                .WithEase(settings.Ease)
                .Bind(progress => ApplyFadeProgress(fadeTargets, progress, phaseType));
        }

        MotionHandle CreateScaleMotion(ScaleSettings settings, PhaseType phaseType, float duration)
        {
            var startScale = phaseType == PhaseType.Hide ? baseScale : baseScale * settings.Multiplier;
            var targetScale = phaseType == PhaseType.Hide ? baseScale * settings.Multiplier : baseScale;

            return LMotion.Create(startScale, targetScale, duration)
                .WithEase(settings.Ease)
                .Bind(value => transform.localScale = value);
        }

        void CancelRunningMotions()
        {
            moveHandle.TryCancel();
            fadeHandle.TryCancel();
            scaleHandle.TryCancel();
        }

        void CacheChildFadeTargets()
        {
            childFadeTargets.Clear();

            var graphics = GetComponentsInChildren<Graphic>(true);
            foreach (var graphic in graphics)
            {
                childFadeTargets.Add(CreateGraphicFadeTarget(graphic));
            }

            var spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
            foreach (var spriteRenderer in spriteRenderers)
            {
                childFadeTargets.Add(CreateSpriteFadeTarget(spriteRenderer));
            }
        }

        void ApplyFadeProgress(IReadOnlyList<FadeTarget> fadeTargets, float progress, PhaseType phaseType)
        {
            foreach (var target in fadeTargets)
            {
                var from = phaseType == PhaseType.Hide ? target.BaseAlpha : 0f;
                var to = phaseType == PhaseType.Hide ? 0f : target.BaseAlpha;

                var value = Mathf.Lerp(from, to, progress);
                target.ApplyAlpha(value);
            }
        }

        static FadeTarget CreateCanvasGroupFadeTarget(CanvasGroup target)
        {
            if (target == null)
            {
                return CreateInvalidFadeTarget();
            }

            return new FadeTarget(
                () => target != null,
                value => target.alpha = value,
                target.alpha);
        }

        static FadeTarget CreateGraphicFadeTarget(Graphic target)
        {
            if (target == null)
            {
                return CreateInvalidFadeTarget();
            }

            return new FadeTarget(
                () => target != null,
                value =>
                {
                    var color = target.color;
                    color.a = value;
                    target.color = color;
                },
                target.color.a);
        }

        static FadeTarget CreateSpriteFadeTarget(SpriteRenderer target)
        {
            if (target == null)
            {
                return CreateInvalidFadeTarget();
            }

            return new FadeTarget(
                () => target != null,
                value =>
                {
                    var color = target.color;
                    color.a = value;
                    target.color = color;
                },
                target.color.a);
        }

        static FadeTarget CreateInvalidFadeTarget()
        {
            return new FadeTarget(() => false, _ => { }, 0f);
        }

        readonly struct FadeTarget
        {
            public Func<bool> IsAlive { get; }
            public float BaseAlpha { get; }

            readonly Action<float> writeAlpha;
            
            public FadeTarget(Func<bool> isAlive, Action<float> writeAlpha, float baseAlpha)
            {
                IsAlive = isAlive;
                BaseAlpha = baseAlpha;
                this.writeAlpha = writeAlpha;
            }

            public void ApplyAlpha(float alpha)
            {
                if (!IsAlive())
                {
                    return;
                }

                writeAlpha(alpha);
            }
        }

        enum PhaseType
        {
            Show,
            Hide
        }
    }
}
