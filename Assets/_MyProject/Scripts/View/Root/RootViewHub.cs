using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

namespace MyProject.View
{
    public class RootViewHub : MonoBehaviour
    {
        [SerializeField] StandardSliderView audioSlider;
        [SerializeField] AudioButtonView audioButton;

        readonly CompositeDisposable disposables = new();

        public async UniTask InitializeAsync(CancellationToken ct)
        {
            gameObject.SetActive(true);

            InitializeViews();
            BindAudioViews();
            await ShowAsync(ct);
        }

        UniTask ShowAsync(CancellationToken ct)
        {
            return UniTask.WhenAll
            (
                audioSlider.ShowAsync(ct),
                audioButton.ShowAsync(ct)
            );
        }

        void InitializeViews()
        {
            audioSlider.Initialize();
            audioButton.Initialize();
        }

        void BindAudioViews()
        {
            disposables.Clear();

            var audioPlayer = AudioPlayerView.Instance;
            var volume = audioPlayer.BgmVolume.CurrentValue;

            audioSlider.SetValue(volume);
            audioButton.SetVolume(volume);
            audioPlayer.SetBgmVolume(volume);
            audioPlayer.SetSeVolume(volume);

            audioSlider.ValueChanged
                .Subscribe(value =>
                {
                    audioPlayer.SetBgmVolume(value);
                    audioPlayer.SetSeVolume(value);
                })
                .AddTo(disposables);
            audioSlider.HandleDoubleClicked
                .Subscribe(_ =>
                {
                    audioPlayer.ResetBgmVolume();
                    audioPlayer.ResetSeVolume();
                })
                .AddTo(disposables);
            audioButton.VolumeRequested
                .Subscribe(value =>
                {
                    audioPlayer.SetBgmVolume(value);
                    audioPlayer.SetSeVolume(value);
                })
                .AddTo(disposables);

            audioPlayer.BgmVolume
                .Subscribe(value =>
                {
                    audioSlider.SetValue(value);
                    audioButton.SetVolume(value);
                })
                .AddTo(disposables);
        }

        void OnDestroy()
        {
            disposables.Dispose();
        }
    }
}
