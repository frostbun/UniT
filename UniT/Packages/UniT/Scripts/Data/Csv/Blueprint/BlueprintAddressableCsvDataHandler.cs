namespace UniT.Data.Csv.Blueprint
{
    using System;
    using Cysharp.Threading.Tasks;
    using UniT.Addressables;
    using UniT.Data.Base;
    using UniT.Data.Csv.Base;
    using UnityEngine;
    using ILogger = UniT.Logging.ILogger;

    public class BlueprintAddressableCsvDataHandler : BaseCsvDataHandler
    {
        private readonly IAddressableManager addressableManager;

        public BlueprintAddressableCsvDataHandler(IAddressableManager addressableManager, ILogger logger = null) : base(logger)
        {
            this.addressableManager = addressableManager;
        }

        protected override bool CanHandle(Type type)
        {
            return base.CanHandle(type) && typeof(IBlueprintData).IsAssignableFrom(type);
        }

        protected override UniTask<string[]> LoadRawData(string[] keys)
        {
            return UniTask.WhenAll(keys.Select(key =>
            {
                return this.addressableManager.Load<TextAsset>(key).ContinueWith(blueprint =>
                {
                    var text = blueprint.text;
                    this.addressableManager.Unload(key);
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