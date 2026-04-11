using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MyProject.Actor
{
    [RequireComponent(typeof(ActorAnimationTimeline))]
    public abstract class SceneActorHubBase : ActorBase
    {
        protected ActorAnimationTimeline animationTimeline;

        public override void Initialize()
        {
            animationTimeline = GetComponent<ActorAnimationTimeline>();
            animationTimeline.Initialize();

            gameObject.SetActive(false);
        }

        public override async UniTask InitialShowAsync(CancellationToken ct)
        {
            gameObject.SetActive(true);
            await animationTimeline.InitialShowAsync(ct);
        }

        public override async UniTask ShowAsync(CancellationToken ct)
        {
            gameObject.SetActive(true);
            await animationTimeline.ShowAsync(ct);
        }

        public override async UniTask HideAsync(CancellationToken ct)
        {
            await animationTimeline.HideAsync(ct);
            gameObject.SetActive(false);
        }
    }
}