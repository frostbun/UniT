namespace UniT.Data.Json.Player
{
    using System;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using UniT.Data.Base;
    using UniT.Data.Json.Base;
    using UniT.Extensions;
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

        protected override UniTask<string[]> GetRawData(string[] keys)
        {
            return UniTask.FromResult(keys.Select(PlayerPrefs.GetString).ToArray());
        }

        protected override UniTask SaveRawData(string[] keys, string[] rawDatas)
        {
            IterTools.Zip(keys, rawDatas).ForEach(PlayerPrefs.SetString);
            return UniTask.CompletedTask;
        }
    }
}