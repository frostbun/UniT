namespace UniT.Data.Json.Player
{
    using System;
    using Cysharp.Threading.Tasks;
    using UniT.Data.Base;
    using UniT.Data.Json.Base;
    using UnityEngine;

    public class PlayerPrefsJsonDataHandler : BaseJsonDataHandler
    {
        protected override bool CanHandle(Type type)
        {
            return base.CanHandle(type) && typeof(IPlayerData).IsAssignableFrom(type);
        }

        protected override UniTask Flush()
        {
            PlayerPrefs.Save();
            return UniTask.CompletedTask;
        }

        protected override UniTask<string> GetRawData(string key)
        {
            return UniTask.FromResult(PlayerPrefs.GetString(key));
        }

        protected override UniTask SaveRawData(string key, string json)
        {
            PlayerPrefs.SetString(key, json);
            return UniTask.CompletedTask;
        }
    }
}