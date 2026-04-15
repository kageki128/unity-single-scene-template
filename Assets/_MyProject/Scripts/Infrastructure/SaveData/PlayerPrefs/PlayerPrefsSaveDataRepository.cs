using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MyProject.Core;
using UnityEngine;

namespace MyProject.Infrastructure
{
    public class PlayerPrefsSaveDataRepository : ISaveDataRepository
    {
        const string SaveDataKey = "save_data";

        public UniTask SaveAsync(SaveDataCore saveData, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            var jsonData = PlayerPrefsSaveData.FromCore(saveData);
            var json = JsonUtility.ToJson(jsonData);
            PlayerPrefs.SetString(SaveDataKey, json);
            PlayerPrefs.Save();

            Debug.Log($"[PlayerPrefsSaveDataRepository] Saved data: {json}");

            return UniTask.CompletedTask;
        }

        public UniTask<SaveDataCore> LoadAsync(CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            if (!PlayerPrefs.HasKey(SaveDataKey))
            {
                return UniTask.FromResult(new SaveDataCore());
            }

            var json = PlayerPrefs.GetString(SaveDataKey);
            var jsonData = JsonUtility.FromJson<PlayerPrefsSaveData>(json);
            if (jsonData == null)
            {
                throw new InvalidOperationException($"Failed to deserialize save data. key={SaveDataKey}");
            }

            Debug.Log($"[PlayerPrefsSaveDataRepository] Loaded data: {json}");

            return UniTask.FromResult(jsonData.ToCore());
        }
    }
}
