using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MyProject.Actor
{
    [RequireComponent(typeof(ActorAnimationTimeline))]
    public class ResultActorHub : ActorBase
    {
        ActorAnimationTimeline animationTimeline;

        public override void Initialize()
        {
            animationTimeline = GetComponent<ActorAnimationTimeline>();
            gameObject.SetActive(false);
        }

        public override async UniTask InitialShowAsync(CancellationToken ct)
        {
            gameObject.SetActive(true);
            await animationTimeline.PlayInitialShowTimelineAsync(ct);
        }

        public override async UniTask ShowAsync(CancellationToken ct)
        {
            gameObject.SetActive(true);
            await animationTimeline.PlayShowTimelineAsync(ct);
        }

        public override async UniTask HideAsync(CancellationToken ct)
        {
            await animationTimeline.PlayHideTimelineAsync(ct);
            gameObject.SetActive(false);
        }
    }
}
