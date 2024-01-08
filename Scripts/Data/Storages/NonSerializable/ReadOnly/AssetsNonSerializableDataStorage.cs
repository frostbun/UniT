namespace UniT.Data.Storages
{
    using System.Linq;
    using UniT.Data.Types;
    using UniT.ResourcesManager;
    using UnityEngine.Scripting;
    using Object = UnityEngine.Object;
    #if UNIT_UNITASK
    using System;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using UniT.Extensions;
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
        #endif
    }
}