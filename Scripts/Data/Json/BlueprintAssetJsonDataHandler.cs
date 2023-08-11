namespace UniT.Data.Json
{
    using System;
    using Cysharp.Threading.Tasks;
    using UniT.ResourceManager;
    using UnityEngine;
    using UnityEngine.Scripting;
    using ILogger = UniT.Logging.ILogger;

    public class BlueprintAssetJsonDataHandler : BaseJsonDataHandler
    {
        private readonly IAssetManager _assetManager;

        [Preserve]
        public BlueprintAssetJsonDataHandler(IAssetManager assetManager = null, ILogger logger = null) : base(logger)
        {
            this._assetManager = assetManager ?? IAssetManager.Default();
        }

        protected override bool CanHandle(Type type)
        {
            return base.CanHandle(type) && typeof(IBlueprintData).IsAssignableFrom(type);
        }

        protected override UniTask<string[]> LoadRawData(string[] keys)
        {
            return UniTask.WhenAll(keys.Select(key =>
            {
                return this._assetManager.Load<TextAsset>(key).ContinueWith(blueprint =>
                {
                    var text = blueprint.text;
                    this._assetManager.Unload(key);
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