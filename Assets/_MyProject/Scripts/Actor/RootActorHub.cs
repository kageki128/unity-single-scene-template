using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MyProject.Actor
{
    public class RootActorHub : MonoBehaviour
    {
        [SerializeField] ScrollBackgroundActor scrollBackgroundActor;

        public async UniTask InitializeAsync(CancellationToken ct)
        {
            gameObject.SetActive(true);

            Initialzie();
            await InitialShowAsync(ct);
        }

        void Initialzie()
        {
            scrollBackgroundActor.Initialize();
        }

        async UniTask InitialShowAsync(CancellationToken ct)
        {
            await scrollBackgroundActor.InitialShowAsync(ct);
        }
    }
}
