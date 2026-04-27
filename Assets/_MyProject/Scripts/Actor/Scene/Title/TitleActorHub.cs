using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace MyProject.Actor
{
    [RequireComponent(typeof(ActorAnimationTimeline))]
    public class TitleActorHub : SceneActorHubBase
    {
        ActorAnimationTimeline animationTimeline;
        TitleActionsObserver titleActions;

        [Inject]
        public void Construct(TitleActionsObserver titleActions)
        {
            this.titleActions = titleActions;
        }

        public override void Initialize()
        {
            animationTimeline = GetComponent<ActorAnimationTimeline>();

            titleActions.Disable();
            animationTimeline.Initialize();
            gameObject.SetActive(false);
        }

        public override async UniTask InitialShowAsync(CancellationToken ct)
        {
            gameObject.SetActive(true);
            await animationTimeline.InitialShowAsync(ct);
            titleActions.Enable();
        }

        public override async UniTask ShowAsync(CancellationToken ct)
        {
            gameObject.SetActive(true);
            await animationTimeline.ShowAsync(ct);
            titleActions.Enable();
        }

        public override async UniTask HideAsync(CancellationToken ct)
        {
            titleActions.Disable();
            await animationTimeline.HideAsync(ct);
            gameObject.SetActive(false);
        }
    }
}
