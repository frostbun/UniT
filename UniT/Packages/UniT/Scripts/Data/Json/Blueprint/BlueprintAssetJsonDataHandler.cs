namespace UniT.Data.Json.Blueprint
{
    using System;
    using Cysharp.Threading.Tasks;
    using UniT.Assets;
    using UniT.Data.Base;
    using UniT.Data.Json.Base;
    using UnityEngine;
    using ILogger = UniT.Logging.ILogger;

    public class BlueprintAssetJsonDataHandler : BaseJsonDataHandler
    {
        private readonly IAssetsManager assetsManager;

        public BlueprintAssetJsonDataHandler(IAssetsManager assetsManager = null, ILogger logger = null) : base(logger)
        {
            this.assetsManager = assetsManager ?? IAssetsManager.Factory.Default();
        }

        protected override bool CanHandle(Type type)
        {
            return base.CanHandle(type) && typeof(IBlueprintData).IsAssignableFrom(type);
        }

        protected override UniTask<string[]> LoadRawData(string[] keys)
        {
            return UniTask.WhenAll(keys.Select(key =>
            {
                return this.assetsManager.Load<TextAsset>(key).ContinueWith(blueprint =>
                {
                    var text = blueprint.text;
                    this.assetsManager.Unload(key);
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