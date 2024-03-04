namespace UniT.Data
{
    using System;
    using System.Linq;
    using UniT.ResourcesManager;
    using UnityEngine.Scripting;
    using Object = UnityEngine.Object;
    #if UNIT_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using UniT.Extensions;
    #else
    using System.Collections;
    using System.Collections.Generic;
    #endif

    public sealed class AssetsNonSerializableDataStorage : ReadOnlyNonSerializableDataStorage
    {
        private readonly IAssetsManager assetsManager;

        [Preserve]
        public AssetsNonSerializableDataStorage(IAssetsManager assetsManager)
        {
            this.assetsManager = assetsManager;
        }

        protected override IData[] Load(string[] keys)
        {
            return keys.Select(key => (IData)this.assetsManager.Load<Object>(key)).ToArray();
        }

        #if UNIT_UNITASK
        protected override UniTask<IData[]> LoadAsync(string[] keys, IProgress<float> progress, CancellationToken cancellationToken)
        {
            return keys.SelectAsync(
                (key, progress, cancellationToken) =>
                    this.assetsManager.LoadAsync<Object>(key, progress, cancellationToken)
                        .ContinueWith(asset => (IData)asset),
                progress,
                cancellationToken
            ).ToArrayAsync();
        }
        #else
        protected override IEnumerator LoadAsync(string[] keys, Action<IData[]> callback, IProgress<float> progress)
        {
            // TODO: make it run concurrently
            var datas = new List<IData>();
            foreach (var key in keys)
            {
                yield return this.assetsManager.LoadAsync<Object>(key, asset =>
                {
                    datas.Add((IData)asset);
                });
            }
            progress?.Report(1);
            callback(datas.ToArray());
        }
        #endif
    }
}