using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

namespace MyProject.View
{
    [RequireComponent(typeof(ViewAnimationTimeline))]
    public class GameViewHub : SceneViewHubBase
    {
        public Observable<Unit> ToSelectButtonClicked => toSelectButton.Clicked;

        [SerializeField] StandardButtonView toSelectButton;

        GameActionsObserver gameActionsObserver;
        ViewAnimationTimeline animationTimeline;

        public override void Initialize()
        {
            gameActionsObserver ??= new GameActionsObserver();
            animationTimeline = GetComponent<ViewAnimationTimeline>();

            gameActionsObserver.Disable();
            animationTimeline.Initialize();
            gameObject.SetActive(false);
        }

        public override async UniTask ShowAsync(CancellationToken ct)
        {
            gameObject.SetActive(true);
            await animationTimeline.ShowAsync(ct);
            gameActionsObserver.Enable();
        }

        public override async UniTask HideAsync(CancellationToken ct)
        {
            gameActionsObserver.Disable();
            await animationTimeline.HideAsync(ct);
            gameObject.SetActive(false);
        }

        void OnDestroy()
        {
            gameActionsObserver?.Dispose();
        }
    }
}
