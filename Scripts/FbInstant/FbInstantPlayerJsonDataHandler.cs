#if UNIT_FBINSTANT
namespace UniT.Data.Json
{
    using System;
    using Cysharp.Threading.Tasks;
    using FbInstant.Player;
    using UniT.Extensions;
    using UniT.Logging;
    using UnityEngine.Scripting;

    public class FbInstantPlayerJsonDataHandler : BaseJsonDataHandler
    {
        private readonly FbInstantPlayer _player;

        [Preserve]
        public FbInstantPlayerJsonDataHandler(FbInstantPlayer player, ILogger logger = null) : base(logger)
        {
            this._player = player;
        }

        protected override bool CanHandle(Type type)
        {
            return base.CanHandle(type) && typeof(IPlayerData).IsAssignableFrom(type);
        }

        protected override UniTask<string[]> LoadRawData(string[] keys)
        {
            return this._player.LoadData(keys).ContinueWith((rawDatas, error) =>
            {
                if (error is not null)
                {
                    this._logger.Critical($"Ignoring load {keys.ToJson()} error: {error}");
                }
                return rawDatas;
            });
        }

        protected override UniTask SaveRawData(string[] keys, string[] rawDatas)
        {
            return this._player.SaveData(keys, rawDatas).ContinueWith(error =>
            {
                if (error is not null)
                {
                    this._logger.Critical($"Ignoring save {keys.ToJson()} error: {error}");
                }
            });
        }

        protected override UniTask Flush()
        {
            return this._player.FlushData().ContinueWith(error =>
            {
                if (error is not null)
                {
                    this._logger.Critical($"Ignoring flush error: {error}");
                }
            });
        }
    }
}
#endif