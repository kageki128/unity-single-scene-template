using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace MyProject.View
{
    [RequireComponent(typeof(StandardButtonView))]
    public class AudioButtonView : RootViewBase
    {
        public Observable<float> VolumeRequested => volumeRequested;
        readonly Subject<float> volumeRequested = new();

        readonly CompositeDisposable disposables = new();

        [SerializeField] Image iconImage;
        [SerializeField] Sprite volumeOnIcon;
        [SerializeField] Sprite volumeOffIcon;

        StandardButtonView standardButton;

        float currentVolume;
        float? savedVolume;

        public override void Initialize()
        {
            disposables.Clear();
            savedVolume = null;

            standardButton = GetComponent<StandardButtonView>();
            standardButton.Initialize();

            standardButton.Clicked
                .Subscribe(_ => HandleClicked())
                .AddTo(disposables);

            standardButton.ShowAsync(destroyCancellationToken).Forget();
        }

        public void SetVolume(float volume)
        {
            currentVolume = volume;
            iconImage.sprite = volume <= 0f ? volumeOffIcon : volumeOnIcon;

            if (savedVolume.HasValue && volume > 0f)
            {
                savedVolume = null;
            }
        }

        void HandleClicked()
        {
            if (savedVolume.HasValue)
            {
                var restoreVolume = savedVolume.Value;
                savedVolume = null;
                volumeRequested.OnNext(restoreVolume);
                return;
            }

            savedVolume = currentVolume;
            volumeRequested.OnNext(0f);
        }

        void OnDestroy()
        {
            disposables.Dispose();

            volumeRequested.OnCompleted();
            volumeRequested.Dispose();
        }
    }
}
