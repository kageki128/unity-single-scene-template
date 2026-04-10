using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MyProject.Actor
{
    [RequireComponent(typeof(ActorAnimationTimeline))]
    public class TitleActorHub : ActorBase
    {
        ActorAnimationTimeline animationTimeline;

        public override void Initialize()
        {
            gameObject.SetActive(true);

            animationTimeline = GetComponent<ActorAnimationTimeline>();
        }

        public override async UniTask InitialShowAsync(CancellationToken ct)
        {
            await animationTimeline.PlayInitialShowTimelineAsync(ct);
        }

        public override async UniTask ShowAsync(CancellationToken ct)
        {
            await animationTimeline.PlayShowTimelineAsync(ct);
        }

        public override async UniTask HideAsync(CancellationToken ct)
        {
            await animationTimeline.PlayHideTimelineAsync(ct);
        }
    }
}
