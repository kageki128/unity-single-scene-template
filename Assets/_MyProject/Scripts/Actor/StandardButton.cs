using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

namespace MyProject.Actor
{
    [RequireComponent(typeof(PointerEventObserver))]
    public class StandardButton : ActorBase
    {
        public Observable<Unit> Clicked => pointerEventObserver.Clicked.Select(_ => Unit.Default);

        PointerEventObserver pointerEventObserver;

        public override void Initialize()
        {
            pointerEventObserver = GetComponent<PointerEventObserver>();
            gameObject.SetActive(false);

            Clicked.Subscribe(_ => HandleClicked()).AddTo(this);
        }

        public override async UniTask ShowAsync(CancellationToken ct)
        {
            gameObject.SetActive(true);
        }

        public override async UniTask HideAsync(CancellationToken ct)
        {
            gameObject.SetActive(false);
        }

        void HandleClicked()
        {
            Debug.Log("Button clicked!");
        }
    }
}