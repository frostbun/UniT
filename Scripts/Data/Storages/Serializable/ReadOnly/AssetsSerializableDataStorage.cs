namespace UniT.Data.Storages
{
    using System;
    using System.Linq;
    using UniT.ResourcesManager;
    using UnityEngine;
    using UnityEngine.Scripting;
    #if UNIT_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using UniT.Extensions;
    #else
    using System.Collections;
    using System.Collections.Generic;
    #endif

    public sealed class AssetsSerializableDataStorage : ReadOnlySerializableDataStorage
    {
        private readonly IAssetsManager assetsManager;

        [Preserve]
        public AssetsSerializableDataStorage(IAssetsManager assetsManager)
        {
            this.assetsManager = assetsManager;
        }

        protected override string[] Load(string[] keys)
        {
            return keys.Select(key =>
            {
                var text = this.assetsManager.Load<TextAsset>(key).text;
                this.assetsManager.Unload(key);
                return text;
            }).ToArray();
        }

        #if UNIT_UNITASK
        protected override UniTask<string[]> LoadAsync(string[] keys, IProgress<float> progress, CancellationToken cancellationToken)
        {
            return keys.SelectAsync(
                (key, progress, cancellationToken) =>
                    this.assetsManager.LoadAsync<TextAsset>(key, progress, cancellationToken)
                        .ContinueWith(asset =>
                        {
                            var text = asset.text;
                            this.assetsManager.Unload(key);
                            return text;
                        }),
                progress,
                cancellationToken
            ).ToArrayAsync();
        }
        #else
        protected override IEnumerator LoadAsync(string[] keys, Action<string[]> callback, IProgress<float> progress)
        {
            // TODO: make it run concurrently
            var rawDatas = new List<string>();
            foreach (var key in keys)
            {
                yield return this.assetsManager.LoadAsync<TextAsset>(key, asset =>
                {
                    rawDatas.Add(asset.text);
                    this.assetsManager.Unload(key);
                });
            }
            progress?.Report(1);
            callback(rawDatas.ToArray());
        }
        #endif
    }
}