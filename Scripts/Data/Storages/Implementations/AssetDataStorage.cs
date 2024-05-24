#nullable enable
namespace UniT.Data
{
    using System;
    using System.Linq;
    using UniT.ResourcesManager;
    using UnityEngine;
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

    public sealed class AssetDataStorage : IReadableSerializableDataStorage, IReadableNonSerializableDataStorage
    {
        private readonly IAssetsManager assetsManager;

        [Preserve]
        public AssetDataStorage(IAssetsManager assetsManager)
        {
            this.assetsManager = assetsManager;
        }

        bool IDataStorage.CanStore(Type type) => typeof(IReadableData).IsAssignableFrom(type)
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

        IData[] IReadableNonSerializableDataStorage.Read(string[] keys)
        {
            return keys.Select(key =>
            {
                var data = this.assetsManager.Load<Object>(key);
                return (IData)data;
            }).ToArray();
        }

        #if UNIT_UNITASK
        UniTask<string[]> IReadableSerializableDataStorage.ReadStringsAsync(string[] keys, IProgress<float>? progress, CancellationToken cancellationToken)
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

        UniTask<byte[][]> IReadableSerializableDataStorage.ReadBytesAsync(string[] keys, IProgress<float>? progress, CancellationToken cancellationToken)
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

        UniTask<IData[]> IReadableNonSerializableDataStorage.ReadAsync(string[] keys, IProgress<float>? progress, CancellationToken cancellationToken)
        {
            return keys.SelectAsync(
                (key, progress, cancellationToken) =>
                    this.assetsManager.LoadAsync<Object>(key, progress, cancellationToken)
                        .ContinueWith(asset =>
                        {
                            return (IData)asset;
                        }),
                progress,
                cancellationToken
            ).ToArrayAsync();
        }
        #else
        IEnumerator IReadableSerializableDataStorage.ReadStringsAsync(string[] keys, Action<string[]> callback, IProgress<float>? progress)
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

        IEnumerator IReadableSerializableDataStorage.ReadBytesAsync(string[] keys, Action<byte[][]> callback, IProgress<float>? progress)
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

        IEnumerator IReadableNonSerializableDataStorage.ReadAsync(string[] keys, Action<IData[]> callback, IProgress<float>? progress)
        {
            // TODO: make it run concurrently
            var datas = new List<IData>();
            foreach (var key in keys)
            {
                yield return this.assetsManager.LoadAsync<Object>(key, asset => datas.Add((IData)asset));
            }
            progress?.Report(1);
            callback(datas.ToArray());
        }
        #endif
    }
}