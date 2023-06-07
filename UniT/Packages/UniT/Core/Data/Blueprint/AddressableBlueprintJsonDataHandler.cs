namespace UniT.Core.Data.Blueprint
{
    using Cysharp.Threading.Tasks;
    using UniT.Core.Addressables;
    using UnityEngine;

    public class AddressableBlueprintJsonDataHandler : BlueprintJsonDataHandler
    {
        private readonly IAddressableManager addressableManager;

        public AddressableBlueprintJsonDataHandler(IAddressableManager addressableManager)
        {
            this.addressableManager = addressableManager;
        }

        protected override UniTask<string> GetJson(string key)
        {
            return this.addressableManager.Load<TextAsset>(key).ContinueWith(blueprint => blueprint.text);
        }

        protected override UniTask SaveJson(string key, string json)
        {
            return UniTask.CompletedTask;
        }

        public override UniTask Flush()
        {
            return UniTask.CompletedTask;
        }
    }
}