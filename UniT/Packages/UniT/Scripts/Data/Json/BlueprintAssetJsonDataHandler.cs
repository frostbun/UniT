namespace UniT.Data.Json
{
    using System;
    using Cysharp.Threading.Tasks;
    using UniT.Assets;
    using UnityEngine;
    using UnityEngine.Scripting;
    using ILogger = UniT.Logging.ILogger;

    public class BlueprintAssetJsonDataHandler : BaseJsonDataHandler
    {
        private readonly IAssetsManager _assetsManager;

        [Preserve]
        public BlueprintAssetJsonDataHandler(IAssetsManager assetsManager = null, ILogger logger = null) : base(logger)
        {
            this._assetsManager = assetsManager ?? IAssetsManager.Default();
        }

        protected override bool CanHandle(Type type)
        {
            return base.CanHandle(type) && typeof(IBlueprintData).IsAssignableFrom(type);
        }

        protected override UniTask<string[]> LoadRawData(string[] keys)
        {
            return UniTask.WhenAll(keys.Select(key =>
            {
                return this._assetsManager.Load<TextAsset>(key).ContinueWith(blueprint =>
                {
                    var text = blueprint.text;
                    this._assetsManager.Unload(key);
                    return text;
                });
            }));
        }

        protected override UniTask SaveRawData(string[] keys, string[] rawDatas)
        {
            return UniTask.CompletedTask;
        }

        protected override UniTask Flush()
        {
            return UniTask.CompletedTask;
        }
    }
}