using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MyProject.Actor
{
    [RequireComponent(typeof(StandardActorAnimator))]
    public class StandardActor : ActorBase
    {
        StandardActorAnimator animator;

        public override void Initialize()
        {
            animator = GetComponent<StandardActorAnimator>();
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