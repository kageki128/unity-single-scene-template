using R3;

namespace MyProject.Model
{
    public class ScoreModel
    {
        public ReadOnlyReactiveProperty<int> Value => value;
        readonly ReactiveProperty<int> value = new(0);

        public void Add(int amount)
        {
            value.Value += amount;
        }

        public void Reset()
        {
            value.Value = 0;
        }
    }
}