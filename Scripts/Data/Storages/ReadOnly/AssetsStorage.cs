namespace UniT.Data.Storages
{
    using Cysharp.Threading.Tasks;
    using UniT.ResourcesManager;
    using UnityEngine;
    using UnityEngine.Scripting;

    public sealed class AssetsStorage : BaseReadOnlyStorage
    {
        private readonly IAssetsManager assetsManager;

        [Preserve]
        public AssetsStorage(IAssetsManager assetsManager = null)
        {
            this.assetsManager = assetsManager ?? IAssetsManager.Default();
        }

        protected override UniTask<string[]> Load(string[] keys)
        {
            return UniTask.WhenAll(keys.Select(key =>
                this.assetsManager.LoadAsync<TextAsset>(key).ContinueWith(asset =>
                {
                    var text = asset.text;
                    this.assetsManager.Unload(key);
                    return text;
                })
            ));
        }
    }
}