using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MyProject.Actor
{
    [RequireComponent(typeof(ActorAnimationTimeline))]
    public class ResultActorHub : SceneActorHubBase
    {
        ResultActionsObserver resultActionsObserver;
        ActorAnimationTimeline animationTimeline;

        public override void Initialize()
        {
            resultActionsObserver ??= new ResultActionsObserver();
            animationTimeline = GetComponent<ActorAnimationTimeline>();

            resultActionsObserver.Disable();
            animationTimeline.Initialize();
            gameObject.SetActive(false);
        }

        public override async UniTask ShowAsync(CancellationToken ct)
        {
            gameObject.SetActive(true);
            await animationTimeline.ShowAsync(ct);
            resultActionsObserver.Enable();
        }

        public override async UniTask HideAsync(CancellationToken ct)
        {
            resultActionsObserver.Disable();
            await animationTimeline.HideAsync(ct);
            gameObject.SetActive(false);
        }

        void OnDestroy()
        {
            resultActionsObserver?.Dispose();
        }
    }
}
