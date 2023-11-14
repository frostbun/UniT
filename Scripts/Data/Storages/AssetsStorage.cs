namespace UniT.Data.Storages
{
    using System;
    using Cysharp.Threading.Tasks;
    using UniT.Assets;
    using UnityEngine;
    using UnityEngine.Scripting;

    public sealed class AssetsStorage : IReadOnlyStorage
    {
        private readonly IAssetsManager _assetsManager;

        [Preserve]
        public AssetsStorage(IAssetsManager assetsManager)
        {
            this._assetsManager = assetsManager;
        }

        public bool CanStore(Type type)
        {
            return typeof(IReadOnlyData).IsAssignableFrom(type);
        }

        public UniTask<string[]> Load(string[] keys)
        {
            return UniTask.WhenAll(keys.Select(key =>
                this._assetsManager.Load<TextAsset>(key).ContinueWith(asset =>
                {
                    var text = asset.text;
                    this._assetsManager.Unload(key);
                    return text;
                })
            ));
        }
    }
}