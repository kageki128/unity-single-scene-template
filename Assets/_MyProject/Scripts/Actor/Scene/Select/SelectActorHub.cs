using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MyProject.Actor
{
    [RequireComponent(typeof(ActorAnimationTimeline))]
    public class SelectActorHub : SceneActorHubBase
    {
        readonly SelectActionsObserver selectActionsObserver = new();

        ActorAnimationTimeline animationTimeline;

        public override void Initialize()
        {
            animationTimeline = GetComponent<ActorAnimationTimeline>();

            selectActionsObserver.Disable();
            animationTimeline.Initialize();
            gameObject.SetActive(false);
        }

        public override async UniTask InitialShowAsync(CancellationToken ct)
        {
            gameObject.SetActive(true);
            await animationTimeline.InitialShowAsync(ct);
            selectActionsObserver.Enable();
        }

        public override async UniTask ShowAsync(CancellationToken ct)
        {
            gameObject.SetActive(true);
            await animationTimeline.ShowAsync(ct);
            selectActionsObserver.Enable();
        }

        public override async UniTask HideAsync(CancellationToken ct)
        {
            selectActionsObserver.Disable();
            await animationTimeline.HideAsync(ct);
            gameObject.SetActive(false);
        }

        void OnDestroy()
        {
            selectActionsObserver.Dispose();
        }
    }
}
