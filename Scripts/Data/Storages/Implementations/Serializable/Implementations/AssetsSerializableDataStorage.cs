namespace UniT.Data
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

    public sealed class AssetsSerializableDataStorage : SerializableDataStorage, IReadableSerializableDataStorage
    {
        private readonly IAssetsManager assetsManager;

        [Preserve]
        public AssetsSerializableDataStorage(IAssetsManager assetsManager)
        {
            this.assetsManager = assetsManager;
        }

        protected override bool CanStore(Type type) => base.CanStore(type)
            && typeof(IReadableData).IsAssignableFrom(type)
            && !typeof(IWritableData).IsAssignableFrom(type);

        string[] IReadableSerializableDataStorage.ReadStrings(string[] keys)
        {
            return keys.Select(key =>
            {
                var text = this.assetsManager.Load<TextAsset>(key).text;
                this.assetsManager.Unload(key);
                return text;
            }).ToArray();
        }

        byte[][] IReadableSerializableDataStorage.ReadBytes(string[] keys)
        {
            return keys.Select(key =>
            {
                var bytes = this.assetsManager.Load<TextAsset>(key).bytes;
                this.assetsManager.Unload(key);
                return bytes;
            }).ToArray();
        }

        #if UNIT_UNITASK
        UniTask<string[]> IReadableSerializableDataStorage.ReadStringsAsync(string[] keys, IProgress<float> progress, CancellationToken cancellationToken)
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

        UniTask<byte[][]> IReadableSerializableDataStorage.ReadBytesAsync(string[] keys, IProgress<float> progress, CancellationToken cancellationToken)
        {
            return keys.SelectAsync(
                (key, progress, cancellationToken) =>
                    this.assetsManager.LoadAsync<TextAsset>(key, progress, cancellationToken)
                        .ContinueWith(asset =>
                        {
                            var bytes = asset.bytes;
                            this.assetsManager.Unload(key);
                            return bytes;
                        }),
                progress,
                cancellationToken
            ).ToArrayAsync();
        }
        #else
        IEnumerator IReadableSerializableDataStorage.ReadStringsAsync(string[] keys, Action<string[]> callback, IProgress<float> progress)
        {
            // TODO: make it run concurrently
            var rawDatas = new List<string>();
            foreach (var key in keys)
            {
                yield return this.assetsManager.LoadAsync<TextAsset>(key, asset => rawDatas.Add(asset.text));
                this.assetsManager.Unload(key);
            }
            progress?.Report(1);
            callback(rawDatas.ToArray());
        }

        IEnumerator IReadableSerializableDataStorage.ReadBytesAsync(string[] keys, Action<byte[][]> callback, IProgress<float> progress)
        {
            // TODO: make it run concurrently
            var rawDatas = new List<byte[]>();
            foreach (var key in keys)
            {
                yield return this.assetsManager.LoadAsync<TextAsset>(key, asset => rawDatas.Add(asset.bytes));
                this.assetsManager.Unload(key);
            }
            progress?.Report(1);
            callback(rawDatas.ToArray());
        }
        #endif
    }
}