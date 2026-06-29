using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

namespace MyProject.View
{
    [RequireComponent(typeof(StandardSliderView))]
    public class AudioSliderView : RootViewBase
    {
        public Observable<float> ValueChanged => standardSlider.ValueChanged;
        public Observable<Unit> DoubleClicked => standardSlider.DoubleClicked;

        StandardSliderView standardSlider;

        public override void Initialize()
        {
            standardSlider = GetComponent<StandardSliderView>();
            standardSlider.Initialize();
            standardSlider.ShowAsync(destroyCancellationToken).Forget();
        }

        public void SetValue(float value) => standardSlider.SetValue(value);
    }
}
