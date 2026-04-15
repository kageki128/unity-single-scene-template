using System;
using MyProject.Core;

namespace MyProject.Infrastructure
{
    [Serializable]
    public class PlayerPrefsSaveData
    {
        public static PlayerPrefsSaveData FromCore(SaveDataCore core)
        {
            // SaveDataCoreからPlayerPrefsSaveDataへの変換する処理を書く

            throw new NotImplementedException();
        }

        public SaveDataCore ToCore()
        {
            // PlayerPrefsSaveDataからSaveDataCoreへの変換する処理を書く

            throw new NotImplementedException();
        }
    }
}
