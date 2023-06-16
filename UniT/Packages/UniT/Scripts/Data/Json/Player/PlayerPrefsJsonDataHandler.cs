namespace UniT.Data.Json.Player
{
    using System;
    using Cysharp.Threading.Tasks;
    using UniT.Data.Base;
    using UniT.Data.Json.Base;
    using UnityEngine;

    public class PlayerPrefsJsonDataHandler : BaseJsonDataHandler
    {
        public override bool CanHandle(Type type)
        {
            return base.CanHandle(type) && typeof(IPlayerData).IsAssignableFrom(type);
        }

        public override UniTask Flush()
        {
            PlayerPrefs.Save();
            return UniTask.CompletedTask;
        }

        protected override UniTask<string> GetRawData_Internal(string key)
        {
            return UniTask.FromResult(PlayerPrefs.GetString(key));
        }

        protected override UniTask SaveRawData_Internal(string key, string json)
        {
            PlayerPrefs.SetString(key, json);
            return UniTask.CompletedTask;
        }
    }
}