using System.Threading;
using Cysharp.Threading.Tasks;
using MyProject.Model;
using unityroom.Api;

namespace MyProject.Infrastructure
{
    public class UnityroomRankingRegisterer : IRankingRegisterer
    {
        const int BoardNum = 1;
        const ScoreboardWriteMode WriteMode = ScoreboardWriteMode.HighScoreDesc;

        readonly IUnityroomApiClient unityroomApiClient;

        public UnityroomRankingRegisterer(IUnityroomApiClient unityroomApiClient)
        {
            this.unityroomApiClient = unityroomApiClient;
        }

        public UniTask RegisterAsync(RankingData rankingData, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            unityroomApiClient.SendScore(BoardNum, rankingData.Score, WriteMode);

            return UniTask.CompletedTask;
        }
    }
}
