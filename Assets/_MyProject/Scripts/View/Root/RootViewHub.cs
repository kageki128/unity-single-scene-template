using System.Collections.Generic;
using System.Linq;
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

        readonly List<ViewBase> views = new();

        public async UniTask InitializeAsync(CancellationToken ct)
        {
            gameObject.SetActive(true);

            RegisterViews();
            foreach (var view in views)
            {
                view.Initialize();
            }

            BindAudioViews();
            await UniTask.WhenAll(views.Select(view => view.ShowAsync(ct)));
        }

        void RegisterViews()
        {
            views.Clear();
            views.Add(audioSlider);
            views.Add(audioButton);
        }

        void BindAudioViews()
        {
            var audioPlayer = AudioPlayerView.Instance;

            audioSlider.ValueChanged
                .Subscribe(audioPlayer.SetVolume)
                .AddTo(this);
            audioSlider.DoubleClicked
                .Subscribe(_ => audioPlayer.ResetVolume())
                .AddTo(this);
            audioButton.VolumeRequested
                .Subscribe(audioPlayer.SetVolume)
                .AddTo(this);
            audioPlayer.Volume
                .Subscribe(value =>
                {
                    audioSlider.SetValue(value);
                    audioButton.SetVolume(value);
                })
                .AddTo(this);
        }
    }
}
