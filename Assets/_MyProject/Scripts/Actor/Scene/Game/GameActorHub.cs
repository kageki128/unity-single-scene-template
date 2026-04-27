using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using VContainer;

namespace MyProject.Actor
{
    [RequireComponent(typeof(ActorAnimationTimeline))]
    public class GameActorHub : SceneActorHubBase
    {
        public Observable<Unit> ToSelectButtonClicked => toSelectButton.Clicked;

        [SerializeField] StandardButtonActor toSelectButton;

        ActorAnimationTimeline animationTimeline;
        GameActions gameActions;

        [Inject]
        public void Construct(GameActions gameActions)
        {
            this.gameActions = gameActions;
        }

        public override void Initialize()
        {
            animationTimeline = GetComponent<ActorAnimationTimeline>();

            gameActions.Disable();
            animationTimeline.Initialize();
            gameObject.SetActive(false);
        }

        public override async UniTask InitialShowAsync(CancellationToken ct)
        {
            gameObject.SetActive(true);
            await animationTimeline.InitialShowAsync(ct);
            gameActions.Enable();
        }

        public override async UniTask ShowAsync(CancellationToken ct)
        {
            gameObject.SetActive(true);
            await animationTimeline.ShowAsync(ct);
            gameActions.Enable();
        }

        public override async UniTask HideAsync(CancellationToken ct)
        {
            gameActions.Disable();
            await animationTimeline.HideAsync(ct);
            gameObject.SetActive(false);
        }
    }
}
