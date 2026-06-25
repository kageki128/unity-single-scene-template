using System.Threading;
using Cysharp.Threading.Tasks;

namespace MyProject.Model
{
    public interface ISaveDataRepository
    {
        UniTask SaveAsync(SaveDataModel saveData, CancellationToken ct);
        UniTask<SaveDataModel> LoadAsync(CancellationToken ct);
    }
}
