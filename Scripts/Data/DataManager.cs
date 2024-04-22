namespace UniT.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UniT.Extensions;
    using UniT.Logging;
    using UniT.Utilities;
    using UnityEngine.Scripting;
    #if UNIT_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #else
    using System.Collections;
    #endif

    public sealed class DataManager : IDataManager, IHasLogger
    {
        #region Constructor

        private readonly Dictionary<Type, IData>        datas;
        private readonly Dictionary<Type, IDataStorage> storages;
        private readonly Dictionary<Type, ISerializer>  serializers;
        private readonly ILogger                        logger;

        [Preserve]
        public DataManager(
            IEnumerable<IData>        datas,
            IEnumerable<IDataStorage> storages,
            IEnumerable<ISerializer>  serializers,
            ILogger.IFactory          loggerFactory
        )
        {
            datas      = datas as ICollection<IData> ?? datas.ToArray();
            this.datas = datas.ToDictionary(data => data.GetType());
            this.storages = datas.ToDictionary(
                data => data.GetType(),
                data => storages.LastOrDefault(storage => storage.CanStore(data.GetType())) ?? throw new InvalidOperationException($"No storage found for {data.GetType().Name}")
            );
            this.serializers = datas.Where(data => data is ISerializableData).ToDictionary(
                data => data.GetType(),
                data => serializers.LastOrDefault(serializer => serializer.CanSerialize(data.GetType())) ?? throw new InvalidOperationException($"No serializer found for {data.GetType().Name}")
            );

            this.logger = loggerFactory.Create(this);
            this.datas.Keys.ForEach(type => this.logger.Debug($"Found {type.Name} - {this.storages[type].GetType().Name} - {this.serializers.GetOrDefault(type)?.GetType().Name}"));
            this.logger.Debug("Constructed");
        }

        #endregion

        LogConfig IHasLogger.LogConfig => this.logger.Config;

        IData IDataManager.Get(Type type) => this.datas.GetOrDefault(type);

        #region Sync

        void IDataManager.Populate(params Type[] types) => this.Populate(types);

        void IDataManager.Save(params Type[] types) => this.Save(types);

        void IDataManager.Flush(params Type[] types) => this.Flush(types);

        void IDataManager.PopulateAll() => this.Populate(this.datas.Keys.Where(type => typeof(IReadableData).IsAssignableFrom(type)));

        void IDataManager.SaveAll() => this.Save(this.datas.Keys.Where(type => typeof(IWritableData).IsAssignableFrom(type)));

        void IDataManager.FlushAll() => this.Flush(this.datas.Keys.Where(type => typeof(IWritableData).IsAssignableFrom(type)));

        private void Populate(IEnumerable<Type> types)
        {
            types.GroupBy(type => this.storages.GetOrDefault(type) as IReadableDataStorage ?? throw new InvalidOperationException($"{type.Name} not found or not readable"))
                .ForEach(group =>
                {
                    var keys = group.Select(type => type.GetKey()).ToArray();
                    switch (group.Key)
                    {
                        case IReadableSerializableDataStorage storage:
                        {
                            group.GroupBy(type => this.serializers[type])
                                .ForEach(group =>
                                {
                                    var keys = group.Select(type => type.GetKey()).ToArray();
                                    switch (group.Key)
                                    {
                                        case IStringSerializer serializer:
                                        {
                                            var rawDatas = storage.ReadStrings(keys);
                                            IterTools.StrictZip(group, rawDatas)
                                                .Where((_,      rawData) => !rawData.IsNullOrWhitespace())
                                                .ForEach((type, rawData) => serializer.Populate(this.datas[type], rawData));
                                            break;
                                        }
                                        case IBinarySerializer serializer:
                                        {
                                            var rawDatas = storage.ReadBytes(keys);
                                            IterTools.StrictZip(group, rawDatas)
                                                .Where((_,      rawData) => rawData is { Length: > 0 })
                                                .ForEach((type, rawData) => serializer.Populate(this.datas[type], rawData));
                                            break;
                                        }
                                    }
                                });
                            break;
                        }
                        case IReadableNonSerializableDataStorage storage:
                        {
                            var datas = storage.Read(keys);
                            IterTools.StrictZip(group, datas)
                                .ForEach((type, data) => data.CopyTo(this.datas[type]));
                            break;
                        }
                    }
                    this.logger.Debug($"Populated {keys.ToArrayString()}");
                });
        }

        private void Save(IEnumerable<Type> types)
        {
            types.GroupBy(type => this.storages.GetOrDefault(type) as IWritableDataStorage ?? throw new InvalidOperationException($"{type.Name} not found or not writable"))
                .ForEach(group =>
                {
                    var keys = group.Select(type => type.GetKey()).ToArray();
                    switch (group.Key)
                    {
                        case IWritableSerializableDataStorage storage:
                        {
                            group.GroupBy(type => this.serializers[type])
                                .ForEach(group =>
                                {
                                    var keys = group.Select(type => type.GetKey()).ToArray();
                                    switch (group.Key)
                                    {
                                        case IStringSerializer serializer:
                                        {
                                            var rawDatas = group.Select(type => serializer.Serialize(this.datas[type])).ToArray();
                                            storage.WriteStrings(keys, rawDatas);
                                            break;
                                        }
                                        case IBinarySerializer serializer:
                                        {
                                            var rawDatas = group.Select(type => serializer.Serialize(this.datas[type])).ToArray();
                                            storage.WriteBytes(keys, rawDatas);
                                            break;
                                        }
                                    }
                                });
                            break;
                        }
                        case IWritableNonSerializableDataStorage storage:
                        {
                            var datas = group.Select(type => this.datas[type]).ToArray();
                            storage.Save(keys, datas);
                            break;
                        }
                    }
                    this.logger.Debug($"Saved {keys.ToArrayString()}");
                });
        }

        private void Flush(IEnumerable<Type> types)
        {
            types.GroupBy(type => this.storages.GetOrDefault(type) as IWritableDataStorage ?? throw new InvalidOperationException($"{type.Name} not found or not writable"))
                .ForEach(group =>
                {
                    var keys    = group.Select(type => type.GetKey()).ToArray();
                    var storage = group.Key;
                    storage.Flush();
                    this.logger.Debug($"Flushed {keys.ToArrayString()}");
                });
        }

        #endregion

        #region Async

        #if UNIT_UNITASK
        UniTask IDataManager.PopulateAsync(Type[] types, IProgress<float> progress, CancellationToken cancellationToken) => this.PopulateAsync(types, progress, cancellationToken);

        UniTask IDataManager.SaveAsync(Type[] types, IProgress<float> progress, CancellationToken cancellationToken) => this.SaveAsync(types, progress, cancellationToken);

        UniTask IDataManager.FlushAsync(Type[] types, IProgress<float> progress, CancellationToken cancellationToken) => this.FlushAsync(types, progress, cancellationToken);

        UniTask IDataManager.PopulateAllAsync(IProgress<float> progress, CancellationToken cancellationToken) => this.PopulateAsync(this.datas.Keys.Where(type => typeof(IReadableData).IsAssignableFrom(type)), progress, cancellationToken);

        UniTask IDataManager.SaveAllAsync(IProgress<float> progress, CancellationToken cancellationToken) => this.SaveAsync(this.datas.Keys.Where(type => typeof(IWritableData).IsAssignableFrom(type)), progress, cancellationToken);

        UniTask IDataManager.FlushAllAsync(IProgress<float> progress, CancellationToken cancellationToken) => this.FlushAsync(this.datas.Keys.Where(type => typeof(IWritableData).IsAssignableFrom(type)), progress, cancellationToken);

        private UniTask PopulateAsync(IEnumerable<Type> types, IProgress<float> progress, CancellationToken cancellationToken)
        {
            return types.GroupBy(type => this.storages.GetOrDefault(type) as IReadableDataStorage ?? throw new InvalidOperationException($"{type.Name} not found or not readable"))
                .ForEachAsync(
                    async (group, progress, cancellationToken) =>
                    {
                        var keys = group.Select(type => type.GetKey()).ToArray();
                        switch (group.Key)
                        {
                            case IReadableSerializableDataStorage storage:
                            {
                                await group.GroupBy(type => this.serializers[type])
                                    .ForEachAsync(
                                        async (group, progress, cancellationToken) =>
                                        {
                                            var keys = group.Select(type => type.GetKey()).ToArray();
                                            switch (group.Key)
                                            {
                                                case IStringSerializer serializer:
                                                {
                                                    var rawDatas = await storage.ReadStringsAsync(keys, progress, cancellationToken);
                                                    await IterTools.StrictZip(group, rawDatas)
                                                        .Where((_,           rawData) => !rawData.IsNullOrWhitespace())
                                                        .ForEachAsync((type, rawData) => serializer.PopulateAsync(this.datas[type], rawData));
                                                    break;
                                                }
                                                case IBinarySerializer serializer:
                                                {
                                                    var rawDatas = await storage.ReadBytesAsync(keys, progress, cancellationToken);
                                                    await IterTools.StrictZip(group, rawDatas)
                                                        .Where((_,           rawData) => rawData is { Length: > 0 })
                                                        .ForEachAsync((type, rawData) => serializer.PopulateAsync(this.datas[type], rawData));
                                                    break;
                                                }
                                            }
                                        },
                                        progress,
                                        cancellationToken
                                    );
                                break;
                            }
                            case IReadableNonSerializableDataStorage storage:
                            {
                                var datas = await storage.ReadAsync(keys, progress, cancellationToken);
                                IterTools.StrictZip(group, datas)
                                    .ForEach((type, data) => data.CopyTo(this.datas[type]));
                                break;
                            }
                        }
                        this.logger.Debug($"Populated {keys.ToArrayString()}");
                    },
                    progress,
                    cancellationToken
                );
        }

        private UniTask SaveAsync(IEnumerable<Type> types, IProgress<float> progress, CancellationToken cancellationToken)
        {
            return types.GroupBy(type => this.storages.GetOrDefault(type) as IWritableDataStorage ?? throw new InvalidOperationException($"{type.Name} not found or not writable"))
                .ForEachAsync(
                    async (group, progress, cancellationToken) =>
                    {
                        var keys = group.Select(type => type.GetKey()).ToArray();
                        switch (group.Key)
                        {
                            case IWritableSerializableDataStorage storage:
                            {
                                await group.GroupBy(type => this.serializers[type])
                                    .ForEachAsync(
                                        async (group, progress, cancellationToken) =>
                                        {
                                            var keys = group.Select(type => type.GetKey()).ToArray();
                                            switch (group.Key)
                                            {
                                                case IStringSerializer serializer:
                                                {
                                                    var rawDatas = await group.Select(type => serializer.SerializeAsync(this.datas[type]));
                                                    await storage.WriteStringsAsync(keys, rawDatas, progress, cancellationToken);
                                                    break;
                                                }
                                                case IBinarySerializer serializer:
                                                {
                                                    var rawDatas = await group.Select(type => serializer.SerializeAsync(this.datas[type]));
                                                    await storage.WriteBytesAsync(keys, rawDatas, progress, cancellationToken);
                                                    break;
                                                }
                                            }
                                        },
                                        progress,
                                        cancellationToken
                                    );
                                break;
                            }
                            case IWritableNonSerializableDataStorage storage:
                            {
                                var datas = group.Select(type => this.datas[type]).ToArray();
                                await storage.SaveAsync(keys, datas, progress, cancellationToken);
                                break;
                            }
                        }
                        this.logger.Debug($"Saved {keys.ToArrayString()}");
                    },
                    progress,
                    cancellationToken
                );
        }

        private UniTask FlushAsync(IEnumerable<Type> types, IProgress<float> progress, CancellationToken cancellationToken)
        {
            return types.GroupBy(type => this.storages.GetOrDefault(type) as IWritableDataStorage ?? throw new InvalidOperationException($"{type.Name} not found or not writable"))
                .ForEachAsync(
                    async (group, progress, cancellationToken) =>
                    {
                        var keys    = group.Select(type => type.GetKey()).ToArray();
                        var storage = group.Key;
                        await storage.FlushAsync(progress, cancellationToken);
                        this.logger.Debug($"Flushed {keys.ToArrayString()}");
                    },
                    progress,
                    cancellationToken
                );
        }
        #else
        IEnumerator IDataManager.PopulateAsync(Type[] types, Action callback, IProgress<float> progress) => this.PopulateAsync(types, callback, progress);

        IEnumerator IDataManager.SaveAsync(Type[] types, Action callback, IProgress<float> progress) => this.SaveAsync(types, callback, progress);

        IEnumerator IDataManager.FlushAsync(Type[] types, Action callback, IProgress<float> progress) => this.FlushAsync(types, callback, progress);

        IEnumerator IDataManager.PopulateAllAsync(Action callback, IProgress<float> progress) => this.PopulateAsync(this.datas.Keys.Where(type => typeof(IReadableData).IsAssignableFrom(type)), callback, progress);

        IEnumerator IDataManager.SaveAllAsync(Action callback, IProgress<float> progress) => this.SaveAsync(this.datas.Keys.Where(type => typeof(IWritableData).IsAssignableFrom(type)), callback, progress);

        IEnumerator IDataManager.FlushAllAsync(Action callback, IProgress<float> progress) => this.FlushAsync(this.datas.Keys.Where(type => typeof(IWritableData).IsAssignableFrom(type)), callback, progress);

        private IEnumerator PopulateAsync(IEnumerable<Type> types, Action callback, IProgress<float> progress)
        {
            // TODO: make it run concurrently
            foreach (var group in types.GroupBy(type => this.storages.GetOrDefault(type) as IReadableDataStorage ?? throw new InvalidOperationException($"{type.Name} not found or not readable")))
            {
                var keys = group.Select(type => type.GetKey()).ToArray();
                switch (group.Key)
                {
                    case IReadableSerializableDataStorage storage:
                    {
                        foreach (var group1 in group.GroupBy(type => this.serializers[type]))
                        {
                            var keys1 = group1.Select(type => type.GetKey()).ToArray();
                            switch (group1.Key)
                            {
                                case IStringSerializer serializer:
                                {
                                    string[] rawDatas = null;
                                    yield return storage.ReadStringsAsync(keys1, result => rawDatas = result);
                                    foreach (var (type, rawData) in IterTools.StrictZip(group1, rawDatas).Where((_, rawData) => !rawData.IsNullOrWhitespace()))
                                    {
                                        yield return serializer.PopulateAsync(this.datas[type], rawData);
                                    }
                                    break;
                                }
                                case IBinarySerializer serializer:
                                {
                                    byte[][] rawDatas = null;
                                    yield return storage.ReadBytesAsync(keys1, result => rawDatas = result);
                                    foreach (var (type, rawData) in IterTools.StrictZip(group1, rawDatas).Where((_, rawData) => rawData is { Length: > 0 }))
                                    {
                                        yield return serializer.PopulateAsync(this.datas[type], rawData);
                                    }
                                    break;
                                }
                            }
                        }
                        break;
                    }
                    case IReadableNonSerializableDataStorage storage:
                    {
                        yield return storage.ReadAsync(keys, datas =>
                        {
                            IterTools.StrictZip(group, datas)
                                .ForEach((type, data) => data.CopyTo(this.datas[type]));
                        });
                        break;
                    }
                }
                this.logger.Debug($"Populated {keys.ToArrayString()}");
            }
            progress?.Report(1);
            callback?.Invoke();
        }

        private IEnumerator SaveAsync(IEnumerable<Type> types, Action callback, IProgress<float> progress)
        {
            // TODO: make it run concurrently
            foreach (var group in types.GroupBy(type => this.storages.GetOrDefault(type) as IWritableDataStorage ?? throw new InvalidOperationException($"{type.Name} not found or not writable")))
            {
                var keys = group.Select(type => type.GetKey()).ToArray();
                switch (group.Key)
                {
                    case IWritableSerializableDataStorage storage:
                    {
                        foreach (var group1 in group.GroupBy(type => this.serializers[type]))
                        {
                            var keys1 = group1.Select(type => type.GetKey()).ToArray();
                            switch (group1.Key)
                            {
                                case IStringSerializer serializer:
                                {
                                    var rawDatas = new List<string>();
                                    foreach (var type in group1)
                                    {
                                        yield return serializer.SerializeAsync(this.datas[type], result => rawDatas.Add(result));
                                    }
                                    yield return storage.WriteStringsAsync(keys1, rawDatas.ToArray());
                                    break;
                                }
                                case IBinarySerializer serializer:
                                {
                                    var rawDatas = new List<byte[]>();
                                    foreach (var type in group1)
                                    {
                                        yield return serializer.SerializeAsync(this.datas[type], result => rawDatas.Add(result));
                                    }
                                    yield return storage.WriteBytesAsync(keys1, rawDatas.ToArray());
                                    break;
                                }
                            }
                        }
                        break;
                    }
                    case IWritableNonSerializableDataStorage storage:
                    {
                        var datas = group.Select(type => this.datas[type]).ToArray();
                        yield return storage.SaveAsync(keys, datas);
                        break;
                    }
                }
                this.logger.Debug($"Saved {keys.ToArrayString()}");
            }
            progress?.Report(1);
            callback?.Invoke();
        }

        private IEnumerator FlushAsync(IEnumerable<Type> types, Action callback, IProgress<float> progress)
        {
            // TODO: make it run concurrently
            foreach (var group in types.GroupBy(type => this.storages.GetOrDefault(type) as IWritableDataStorage ?? throw new InvalidOperationException($"{type.Name} not found or not writable")))
            {
                var keys = group.Select(type => type.GetKey()).ToArray();
                var storage = group.Key;
                yield return storage.FlushAsync();
                this.logger.Debug($"Flushed {keys.ToArrayString()}");
            }
            progress?.Report(1);
            callback?.Invoke();
        }
        #endif

        #endregion
    }
}