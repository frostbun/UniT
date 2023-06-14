namespace UniT.Data.Csv.Blueprint
{
    using System;
    using Cysharp.Threading.Tasks;
    using UniT.Addressables;
    using UniT.Data.Base;
    using UniT.Data.Csv.Base;
    using UnityEngine;

    public class BlueprintAddressableCsvDataHandler : BaseCsvDataHandler
    {
        private readonly IAddressableManager addressableManager;

        public BlueprintAddressableCsvDataHandler(IAddressableManager addressableManager)
        {
            this.addressableManager = addressableManager;
        }

        public override bool CanHandle(Type type)
        {
            return base.CanHandle(type) && typeof(IBlueprintData).IsAssignableFrom(type);
        }

        protected override UniTask<string> GetRawData_Internal(string key)
        {
            return this.addressableManager.Load<TextAsset>(key).ContinueWith(blueprint =>
            {
                var text = blueprint.text;
                this.addressableManager.Release(key);
                return text;
            });
        }

        protected override UniTask SaveRawData_Internal(string key, string rawData)
        {
            return UniTask.CompletedTask;
        }

        public override UniTask Flush()
        {
            return UniTask.CompletedTask;
        }
    }
}