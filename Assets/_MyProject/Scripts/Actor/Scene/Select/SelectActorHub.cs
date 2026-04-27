using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace MyProject.Actor
{
    [RequireComponent(typeof(ActorAnimationTimeline))]
    public class SelectActorHub : SceneActorHubBase
    {
        ActorAnimationTimeline animationTimeline;
        SelectActions selectActions;

        [Inject]
        public void Construct(SelectActions selectActions)
        {
            this.selectActions = selectActions;
        }

        public override void Initialize()
        {
            animationTimeline = GetComponent<ActorAnimationTimeline>();

            selectActions.Disable();
            animationTimeline.Initialize();
            gameObject.SetActive(false);
        }

        public override async UniTask InitialShowAsync(CancellationToken ct)
        {
            gameObject.SetActive(true);
            await animationTimeline.InitialShowAsync(ct);
            selectActions.Enable();
        }

        public override async UniTask ShowAsync(CancellationToken ct)
        {
            gameObject.SetActive(true);
            await animationTimeline.ShowAsync(ct);
            selectActions.Enable();
        }

        public override async UniTask HideAsync(CancellationToken ct)
        {
            selectActions.Disable();
            await animationTimeline.HideAsync(ct);
            gameObject.SetActive(false);
        }
    }
}
