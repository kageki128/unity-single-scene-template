using System.Threading;
using Cysharp.Threading.Tasks;

namespace MyProject.Model
{
    public interface IRankingRegisterer
    {
        UniTask RegisterAsync(ResultModel result, CancellationToken ct);
    }
}
