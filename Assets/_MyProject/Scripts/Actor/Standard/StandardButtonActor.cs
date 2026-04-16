using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

namespace MyProject.Actor
{
    [RequireComponent(typeof(PointerEventObserver))]
    [RequireComponent(typeof(ButtonAnimator))]
    [RequireComponent(typeof(StandardActorTransitionAnimator))]
    public class StandardButtonActor : ActorBase
    {
        public Observable<Unit> Clicked => pointerEventObserver.Clicked.Select(_ => Unit.Default);

        PointerEventObserver pointerEventObserver;
        ButtonAnimator buttonAnimator;
        StandardActorTransitionAnimator transitionAnimator;

        public override void Initialize()
        {
            pointerEventObserver = GetComponent<PointerEventObserver>();
            buttonAnimator = GetComponent<ButtonAnimator>();
            transitionAnimator = GetComponent<StandardActorTransitionAnimator>();

            transitionAnimator.Initialize();

            gameObject.SetActive(false);
        }

        public override async UniTask ShowAsync(CancellationToken ct)
        {
            gameObject.SetActive(true);
            await transitionAnimator.ShowAsync(ct);
            buttonAnimator.Play();
        }

        public override async UniTask HideAsync(CancellationToken ct)
        {
            buttonAnimator.Stop();
            await transitionAnimator.HideAsync(ct);
            gameObject.SetActive(false);
        }
    }
}
