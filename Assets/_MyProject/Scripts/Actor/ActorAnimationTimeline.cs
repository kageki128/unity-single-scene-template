using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MyProject.Actor
{
    /// <summary>
    /// 対象のActorとアニメーション再生タイミングの組の集合から、アニメーションのタイムラインを作成するクラス。
    /// </summary>
    public class ActorAnimationTimeline : MonoBehaviour
    {
        [Serializable]
        public class TimedActorAnimation
        {
            public ActorBase Actor => actor;
            [SerializeField] ActorBase actor;

            public float StartSeconds => startSeconds;
            [SerializeField, Min(0f)] float startSeconds;
        }

        [Header("Initial Show Timeline (空の場合はShow Timelineを再生)")]
        [SerializeField] List<TimedActorAnimation> initialShowAnimations = new();

        [Header("Show Timeline")]
        [SerializeField] List<TimedActorAnimation> showAnimations = new();

        [Header("Hide Timeline")]
        [SerializeField] List<TimedActorAnimation> hideAnimations = new();

        /// <summary>
        /// Initial Show Timelineを再生する。
        /// </summary>
        public UniTask PlayInitialShowTimelineAsync(CancellationToken ct)
        {
            // 空ならShow Timelineを再生する
            if (initialShowAnimations.Count == 0)
            {
                return PlayShowTimelineAsync(ct);
            }

            return PlayTimelineAsync(initialShowAnimations, PlayInitialShowAsync, ct);
        }

        /// <summary>
        /// Show Timelineを再生する。
        /// </summary>
        public UniTask PlayShowTimelineAsync(CancellationToken ct)
        {
            return PlayTimelineAsync(showAnimations, PlayShowAsync, ct);
        }

        /// <summary>
        /// Hide Timelineを再生する。
        /// </summary>
        public UniTask PlayHideTimelineAsync(CancellationToken ct)
        {
            return PlayTimelineAsync(hideAnimations, PlayHideAsync, ct);
        }

        UniTask PlayTimelineAsync
        (
            List<TimedActorAnimation> timeline,
            Func<ActorBase, CancellationToken, UniTask> playAsync,
            CancellationToken ct
        )
        {
            if (timeline.Count == 0)
            {
                return UniTask.CompletedTask;
            }

            var playTasks = new List<UniTask>(timeline.Count);

            foreach (var timedAnimation in timeline)
            {
                playTasks.Add(PlayTimedAnimationAsync(timedAnimation, playAsync, ct));
            }

            return UniTask.WhenAll(playTasks);
        }

        async UniTask PlayTimedAnimationAsync
        (
            TimedActorAnimation timedAnimation,
            Func<ActorBase, CancellationToken, UniTask> playAsync,
            CancellationToken ct
        )
        {
            ct.ThrowIfCancellationRequested();

            if (timedAnimation.Actor == null)
            {
                Debug.LogWarning(
                    $"[{nameof(ActorAnimationTimeline)}] Actor is not assigned. startSeconds: {timedAnimation.StartSeconds}",
                    this);
                return;
            }

            var startSeconds = Mathf.Max(0f, timedAnimation.StartSeconds);
            if (startSeconds > 0f)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(startSeconds), cancellationToken: ct);
            }

            await playAsync(timedAnimation.Actor, ct);
        }

        static UniTask PlayInitialShowAsync(ActorBase actor, CancellationToken ct)
        {
            return actor.InitialShowAsync(ct);
        }

        static UniTask PlayShowAsync(ActorBase actor, CancellationToken ct)
        {
            return actor.ShowAsync(ct);
        }

        static UniTask PlayHideAsync(ActorBase actor, CancellationToken ct)
        {
            return actor.HideAsync(ct);
        }
    }
}
