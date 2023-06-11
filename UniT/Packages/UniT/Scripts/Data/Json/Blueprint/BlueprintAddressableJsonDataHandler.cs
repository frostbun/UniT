namespace UniT.Data.Json.Blueprint
{
    using System;
    using Cysharp.Threading.Tasks;
    using UniT.Addressables;
    using UniT.Data.Base;
    using UniT.Data.Json.Base;
    using UnityEngine;

    public class BlueprintAddressableJsonDataHandler : BaseJsonDataHandler
    {
        private readonly IAddressableManager addressableManager;

        public BlueprintAddressableJsonDataHandler(IAddressableManager addressableManager)
        {
            this.addressableManager = addressableManager;
        }

        public override bool CanHandle(Type type)
        {
            return typeof(IBlueprintData).IsAssignableFrom(type);
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
            throw new NotImplementedException();
        }

        public override UniTask Flush()
        {
            return UniTask.CompletedTask;
        }
    }
}