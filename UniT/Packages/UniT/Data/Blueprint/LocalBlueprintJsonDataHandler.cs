namespace UniT.Data.Blueprint
{
    using System;
    using Cysharp.Threading.Tasks;
    using UnityEngine;
    using UnityEngine.AddressableAssets;

    public class LocalBlueprintJsonDataHandler : BlueprintJsonDataHandler
    {
        protected override UniTask<string> GetJson(Type type)
        {
            return Addressables.LoadAssetAsync<TextAsset>(type.Name).ToUniTask().ContinueWith(blueprint => blueprint.text);
        }
    }
}