namespace UniT.Data.Storages
{
    using System;
    using Cysharp.Threading.Tasks;
    using UniT.Assets;
    using UnityEngine;
    using UnityEngine.Scripting;

    public sealed class AssetsStorage : IReadOnlyStorage
    {
        private readonly IAssetsManager assetsManager;

        [Preserve]
        public AssetsStorage(IAssetsManager assetsManager)
        {
            this.assetsManager = assetsManager;
        }

        public bool CanStore(Type type)
        {
            return typeof(IReadOnlyData).IsAssignableFrom(type)
                && !typeof(IData).IsAssignableFrom(type);
        }

        public UniTask<string[]> Load(string[] keys)
        {
            return UniTask.WhenAll(keys.Select(key =>
                this.assetsManager.Load<TextAsset>(key).ContinueWith(asset =>
                {
                    var text = asset.text;
                    this.assetsManager.Unload(key);
                    return text;
                })
            ));
        }
    }
}