namespace UniT.Data.Blueprint
{
    using System;
    using Cysharp.Threading.Tasks;
    using UniT.Addressables;
    using UnityEngine;

    public class AddressableBlueprintJsonDataHandler : BlueprintJsonDataHandler
    {
        private readonly AddressableManager addressableManager;

        public AddressableBlueprintJsonDataHandler(AddressableManager addressableManager)
        {
            this.addressableManager = addressableManager;
        }

        protected override UniTask<string> GetJson(Type type)
        {
            return this.addressableManager.Load<TextAsset>(type).ContinueWith(blueprint => blueprint.text);
        }

        protected override UniTask SaveJson(string json, Type type)
        {
            return UniTask.CompletedTask;
        }

        public override UniTask Flush()
        {
            return UniTask.CompletedTask;
        }
    }
}