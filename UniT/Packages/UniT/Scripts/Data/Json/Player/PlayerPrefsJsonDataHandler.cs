namespace UniT.Data.Json.Player
{
    using System;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using UniT.Data.Base;
    using UniT.Data.Json.Base;
    using UniT.Extensions;
    using UnityEngine;
    using ILogger = UniT.Logging.ILogger;

    public class PlayerPrefsJsonDataHandler : BaseJsonDataHandler
    {
        public PlayerPrefsJsonDataHandler(ILogger logger = null) : base(logger)
        {
        }

        protected override bool CanHandle(Type type)
        {
            return base.CanHandle(type) && typeof(IPlayerData).IsAssignableFrom(type);
        }

        protected override UniTask<string[]> LoadRawData(string[] keys)
        {
            return UniTask.FromResult(keys.Select(PlayerPrefs.GetString).ToArray());
        }

        protected override UniTask SaveRawData(string[] keys, string[] rawDatas)
        {
            IterTools.Zip(keys, rawDatas).ForEach(PlayerPrefs.SetString);
            return UniTask.CompletedTask;
        }

        protected override UniTask Flush()
        {
            PlayerPrefs.Save();
            return UniTask.CompletedTask;
        }
    }
}