#if UNIT_FBINSTANT
namespace UniT.Data.Json
{
    using System;
    using Cysharp.Threading.Tasks;
    using FbInstant;
    using UniT.Extensions;
    using UniT.Logging;
    using UnityEngine.Scripting;

    public sealed class FbInstantPlayerJsonDataHandler : BaseJsonDataHandler
    {
        [Preserve]
        public FbInstantPlayerJsonDataHandler(ILogger logger = null) : base(logger)
        {
        }

        protected override bool CanHandle(Type type)
        {
            return base.CanHandle(type) && typeof(IPlayerData).IsAssignableFrom(type);
        }

        protected override UniTask<string[]> LoadRawData(string[] keys)
        {
            return FbInstant.Player.LoadData(keys).ContinueWith(result =>
            {
                if (result.IsError)
                    this._logger.Critical($"Ignoring load {keys.ToJson()} error: {result.Error}");
                return result.Data;
            });
        }

        protected override UniTask SaveRawData(string[] keys, string[] rawDatas)
        {
            return FbInstant.Player.SaveData(keys, rawDatas).ContinueWith(result =>
            {
                if (result.IsError)
                    this._logger.Critical($"Ignoring save {keys.ToJson()} error: {result.Error}");
            });
        }

        protected override UniTask Flush()
        {
            return FbInstant.Player.FlushData().ContinueWith(result =>
            {
                if (result.IsError)
                    this._logger.Critical($"Ignoring flush error: {result.Error}");
            });
        }
    }
}
#endif