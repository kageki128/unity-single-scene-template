using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

namespace MyProject.Actor
{
    public class RootActorHub : MonoBehaviour
    {
        [SerializeField] ScrollBackgroundActor scrollBackgroundActor;
        [SerializeField] StandardSliderActor bgmSliderActor;
        [SerializeField] StandardSliderActor seSliderActor;

        readonly CompositeDisposable disposables = new();

        public async UniTask InitializeAsync(CancellationToken ct)
        {
            gameObject.SetActive(true);

            InitializeActors();
            BindVolumeSliders();
            await InitialShowAsync(ct);
        }

        void InitializeActors()
        {
            scrollBackgroundActor.Initialize();
            bgmSliderActor.Initialize();
            seSliderActor.Initialize();
        }

        UniTask InitialShowAsync(CancellationToken ct)
        {
            return UniTask.WhenAll
            (
                scrollBackgroundActor.InitialShowAsync(ct),
                bgmSliderActor.InitialShowAsync(ct),
                seSliderActor.InitialShowAsync(ct)
            );
        }

        void BindVolumeSliders()
        {
            disposables.Clear();

            var audioPlayer = AudioPlayer.Instance;

            bgmSliderActor.SetValueWithoutNotify(audioPlayer.BgmVolume.CurrentValue);
            seSliderActor.SetValueWithoutNotify(audioPlayer.SeVolume.CurrentValue);

            bgmSliderActor.ValueChanged
                .Subscribe(audioPlayer.SetBgmVolume)
                .AddTo(disposables);
            seSliderActor.ValueChanged
                .Subscribe(audioPlayer.SetSeVolume)
                .AddTo(disposables);
            bgmSliderActor.HandleDoubleClicked
                .Subscribe(_ => audioPlayer.ResetBgmVolume())
                .AddTo(disposables);
            seSliderActor.HandleDoubleClicked
                .Subscribe(_ => audioPlayer.ResetSeVolume())
                .AddTo(disposables);

            audioPlayer.BgmVolume
                .Subscribe(bgmSliderActor.SetValueWithoutNotify)
                .AddTo(disposables);
            audioPlayer.SeVolume
                .Subscribe(seSliderActor.SetValueWithoutNotify)
                .AddTo(disposables);
        }

        void OnDestroy()
        {
            disposables.Dispose();
        }
    }
}
