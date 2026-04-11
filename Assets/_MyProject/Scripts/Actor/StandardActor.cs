using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MyProject.Actor
{
    [RequireComponent(typeof(StandardActorTransitionAnimator))]
    public class StandardActor : ActorBase
    {
        StandardActorTransitionAnimator animator;

        public override void Initialize()
        {
            animator = GetComponent<StandardActorTransitionAnimator>();
            animator.Initialize();

            gameObject.SetActive(false);
        }

        public override async UniTask ShowAsync(CancellationToken ct)
        {
            gameObject.SetActive(true);
            await animator.ShowAsync(ct);
        }

        public override async UniTask HideAsync(CancellationToken ct)
        {
            await animator.HideAsync(ct);
            gameObject.SetActive(false);
        }
    }
}
