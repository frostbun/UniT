#nullable enable
namespace UniT.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UniT.Data.Serialization;
    using UniT.Data.Storage;
    using UniT.Extensions;
    using UniT.Logging;
    using UnityEngine.Scripting;
    #if UNIT_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #else
    using System.Collections;
    #endif

    public sealed class DataManager : IDataManager
    {
        #region Constructor

        private readonly IReadOnlyDictionary<Type, (IData Data, ISerializer Serializer, IDataStorage Storage)> cache;
        private readonly ILogger                                                                               logger;

        [Preserve]
        public DataManager(IEnumerable<IData> datas, IEnumerable<ISerializer> serializers, IEnumerable<IDataStorage> storages, ILoggerManager loggerManager)
        {
            this.cache = datas.ToDictionary(
                data => data.GetType(),
                data => (
                    data,
                    serializers.LastOrDefault(serializer => serializer.CanSerialize(data.GetType())) ?? throw new InvalidOperationException($"No serializer found for {data.GetType().Name}"),
                    storages.LastOrDefault(storage => storage.CanStore(data.GetType())) ?? throw new InvalidOperationException($"No storage found for {data.GetType().Name}")
                )
            );
            this.logger = loggerManager.GetLogger(this);
            this.cache.Values.ForEach(entry => this.logger.Debug($"Found {entry.Data.GetType().Name} - {entry.Serializer.GetType().Name} - {entry.Storage.GetType().Name}"));
            this.logger.Debug("Constructed");
        }

        #endregion

        IData IDataManager.Get(Type type) => this.cache[type].Data;

        #region Sync

        void IDataManager.Populate(params Type[] types) => this.Populate(types);

        void IDataManager.Save(params Type[] types) => this.Save(types);

        void IDataManager.Flush(params Type[] types) => this.Flush(types);

        void IDataManager.PopulateAll() => this.Populate(this.cache.Keys.Where(type => typeof(IReadableData).IsAssignableFrom(type)));

        void IDataManager.SaveAll() => this.Save(this.cache.Keys.Where(type => typeof(IWritableData).IsAssignableFrom(type)));

        void IDataManager.FlushAll() => this.Flush(this.cache.Keys.Where(type => typeof(IWritableData).IsAssignableFrom(type)));

        private void Populate(IEnumerable<Type> types)
        {
            types.GroupBy(type =>
                {
                    if (!this.cache.TryGetValue(type, out var entry)) throw new InvalidOperationException($"{type.Name} not found");
                    if (entry.Storage is not IReadableDataStorage) throw new InvalidOperationException($"{type.Name} not readable");
                    return (entry.Serializer, entry.Storage);
                })
                .ForEach(group =>
                {
                    var keys = group.Select(type => type.GetKey()).ToArray();
                    switch (group.Key)
                    {
                        case (IBinarySerializer, IReadableBinaryStorage):
                        {
                            var (serializer, storage) = ((IBinarySerializer, IReadableBinaryStorage))group.Key;
                            var rawDatas = storage.Read(keys);
                            IterTools.Zip(group, rawDatas)
                                .Where((_,      rawData) => rawData.Length > 0)
                                .ForEach((type, rawData) => serializer.Populate(this.cache[type].Data, rawData));
                            break;
                        }
                        case (IStringSerializer, IReadableStringStorage):
                        {
                            var (serializer, storage) = ((IStringSerializer, IReadableStringStorage))group.Key;
                            var rawDatas = storage.Read(keys);
                            IterTools.Zip(group, rawDatas)
                                .Where((_,      rawData) => rawData.Length > 0)
                                .ForEach((type, rawData) => serializer.Populate(this.cache[type].Data, rawData));
                            break;
                        }
                        case (IObjectSerializer, IReadableObjectStorage):
                        {
                            var (serializer, storage) = ((IObjectSerializer, IReadableObjectStorage))group.Key;
                            var rawDatas = storage.Read(keys);
                            IterTools.Zip(group, rawDatas)
                                .ForEach((type, rawData) => serializer.Populate(this.cache[type].Data, rawData));
                            break;
                        }
                        default: throw new InvalidOperationException();
                    }
                    this.logger.Debug($"Populated {keys.Join(", ")}");
                });
        }

        private void Save(IEnumerable<Type> types)
        {
            types.GroupBy(type =>
                {
                    if (!this.cache.TryGetValue(type, out var entry)) throw new InvalidOperationException($"{type.Name} not found");
                    if (entry.Storage is not IWritableDataStorage) throw new InvalidOperationException($"{type.Name} not writable");
                    return (entry.Serializer, entry.Storage);
                })
                .ForEach(group =>
                {
                    var keys = group.Select(type => type.GetKey()).ToArray();
                    switch (group.Key)
                    {
                        case (IBinarySerializer, IWritableBinaryStorage):
                        {
                            var (serializer, storage) = ((IBinarySerializer, IWritableBinaryStorage))group.Key;
                            var rawDatas = group.Select(type => serializer.Serialize(this.cache[type].Data)).ToArray();
                            storage.Write(keys, rawDatas);
                            break;
                        }
                        case (IStringSerializer, IWritableStringStorage):
                        {
                            var (serializer, storage) = ((IStringSerializer, IWritableStringStorage))group.Key;
                            var rawDatas = group.Select(type => serializer.Serialize(this.cache[type].Data)).ToArray();
                            storage.Write(keys, rawDatas);
                            break;
                        }
                        case (IObjectSerializer, IWritableObjectStorage):
                        {
                            var (serializer, storage) = ((IObjectSerializer, IWritableObjectStorage))group.Key;
                            var rawDatas = group.Select(type => serializer.Serialize(this.cache[type].Data)).ToArray();
                            storage.Write(keys, rawDatas);
                            break;
                        }
                        default: throw new InvalidOperationException();
                    }
                    this.logger.Debug($"Saved {keys.Join(", ")}");
                });
        }

        private void Flush(IEnumerable<Type> types)
        {
            types.GroupBy(type =>
                {
                    if (!this.cache.TryGetValue(type, out var entry)) throw new InvalidOperationException($"{type.Name} not found");
                    return entry.Storage as IWritableDataStorage ?? throw new InvalidOperationException($"{type.Name} not writable");
                })
                .ForEach(group =>
                {
                    var keys    = group.Select(type => type.GetKey()).ToArray();
                    var storage = group.Key;
                    storage.Flush();
                    this.logger.Debug($"Flushed {keys.Join(", ")}");
                });
        }

        #endregion

        #region Async

        #if UNIT_UNITASK
        UniTask IDataManager.PopulateAsync(Type[] types, IProgress<float>? progress, CancellationToken cancellationToken) => this.PopulateAsync(types, progress, cancellationToken);

        UniTask IDataManager.SaveAsync(Type[] types, IProgress<float>? progress, CancellationToken cancellationToken) => this.SaveAsync(types, progress, cancellationToken);

        UniTask IDataManager.FlushAsync(Type[] types, IProgress<float>? progress, CancellationToken cancellationToken) => this.FlushAsync(types, progress, cancellationToken);

        UniTask IDataManager.PopulateAllAsync(IProgress<float>? progress, CancellationToken cancellationToken) => this.PopulateAsync(this.cache.Keys.Where(type => typeof(IReadableData).IsAssignableFrom(type)), progress, cancellationToken);

        UniTask IDataManager.SaveAllAsync(IProgress<float>? progress, CancellationToken cancellationToken) => this.SaveAsync(this.cache.Keys.Where(type => typeof(IWritableData).IsAssignableFrom(type)), progress, cancellationToken);

        UniTask IDataManager.FlushAllAsync(IProgress<float>? progress, CancellationToken cancellationToken) => this.FlushAsync(this.cache.Keys.Where(type => typeof(IWritableData).IsAssignableFrom(type)), progress, cancellationToken);

        private UniTask PopulateAsync(IEnumerable<Type> types, IProgress<float>? progress, CancellationToken cancellationToken)
        {
            return types.GroupBy(type =>
                {
                    if (!this.cache.TryGetValue(type, out var entry)) throw new InvalidOperationException($"{type.Name} not found");
                    if (entry.Storage is not IReadableDataStorage) throw new InvalidOperationException($"{type.Name} not readable");
                    return (entry.Serializer, entry.Storage);
                })
                .ForEachAsync(
                    async (group, progress, cancellationToken) =>
                    {
                        var keys = group.Select(type => type.GetKey()).ToArray();
                        switch (group.Key)
                        {
                            case (IBinarySerializer, IReadableBinaryStorage):
                            {
                                var (serializer, storage) = ((IBinarySerializer, IReadableBinaryStorage))group.Key;
                                var rawDatas = await storage.ReadAsync(keys, progress, cancellationToken);
                                await IterTools.Zip(group, rawDatas)
                                    .Where((_,           rawData) => rawData.Length > 0)
                                    .ForEachAsync((type, rawData) => serializer.PopulateAsync(this.cache[type].Data, rawData));
                                break;
                            }
                            case (IStringSerializer, IReadableStringStorage):
                            {
                                var (serializer, storage) = ((IStringSerializer, IReadableStringStorage))group.Key;
                                var rawDatas = await storage.ReadAsync(keys, progress, cancellationToken);
                                await IterTools.Zip(group, rawDatas)
                                    .Where((_,           rawData) => rawData.Length > 0)
                                    .ForEachAsync((type, rawData) => serializer.PopulateAsync(this.cache[type].Data, rawData));
                                break;
                            }
                            case (IObjectSerializer, IReadableObjectStorage):
                            {
                                var (serializer, storage) = ((IObjectSerializer, IReadableObjectStorage))group.Key;
                                var rawDatas = await storage.ReadAsync(keys, progress, cancellationToken);
                                await IterTools.Zip(group, rawDatas)
                                    .ForEachAsync((type, rawDatas) => serializer.PopulateAsync(this.cache[type].Data, rawDatas));
                                break;
                            }
                            default: throw new InvalidOperationException();
                        }
                        this.logger.Debug($"Populated {keys.Join(", ")}");
                    },
                    progress,
                    cancellationToken
                );
        }

        private UniTask SaveAsync(IEnumerable<Type> types, IProgress<float>? progress, CancellationToken cancellationToken)
        {
            return types.GroupBy(type =>
                {
                    if (!this.cache.TryGetValue(type, out var entry)) throw new InvalidOperationException($"{type.Name} not found");
                    if (entry.Storage is not IWritableDataStorage) throw new InvalidOperationException($"{type.Name} not writable");
                    return (entry.Serializer, entry.Storage);
                })
                .ForEachAsync(
                    async (group, progress, cancellationToken) =>
                    {
                        var keys = group.Select(type => type.GetKey()).ToArray();
                        switch (group.Key)
                        {
                            case (IBinarySerializer, IWritableBinaryStorage):
                            {
                                var (serializer, storage) = ((IBinarySerializer, IWritableBinaryStorage))group.Key;
                                var rawDatas = await group.Select(type => serializer.SerializeAsync(this.cache[type].Data));
                                await storage.WriteAsync(keys, rawDatas, progress, cancellationToken);
                                break;
                            }
                            case (IStringSerializer, IWritableStringStorage):
                            {
                                var (serializer, storage) = ((IStringSerializer, IWritableStringStorage))group.Key;
                                var rawDatas = await group.Select(type => serializer.SerializeAsync(this.cache[type].Data));
                                await storage.WriteAsync(keys, rawDatas, progress, cancellationToken);
                                break;
                            }
                            case (IObjectSerializer, IWritableObjectStorage):
                            {
                                var (serializer, storage) = ((IObjectSerializer, IWritableObjectStorage))group.Key;
                                var rawDatas = await group.Select(type => serializer.SerializeAsync(this.cache[type].Data));
                                await storage.WriteAsync(keys, rawDatas, progress, cancellationToken);
                                break;
                            }
                            default: throw new InvalidOperationException();
                        }
                        this.logger.Debug($"Saved {keys.Join(", ")}");
                    },
                    progress,
                    cancellationToken
                );
        }

        private UniTask FlushAsync(IEnumerable<Type> types, IProgress<float>? progress, CancellationToken cancellationToken)
        {
            return types.GroupBy(type =>
                {
                    if (!this.cache.TryGetValue(type, out var entry)) throw new InvalidOperationException($"{type.Name} not found");
                    return entry.Storage as IWritableDataStorage ?? throw new InvalidOperationException($"{type.Name} not writable");
                })
                .ForEachAsync(
                    async (group, progress, cancellationToken) =>
                    {
                        var keys    = group.Select(type => type.GetKey()).ToArray();
                        var storage = group.Key;
                        await storage.FlushAsync(progress, cancellationToken);
                        this.logger.Debug($"Flushed {keys.Join(", ")}");
                    },
                    progress,
                    cancellationToken
                );
        }
        #else
        IEnumerator IDataManager.PopulateAsync(Type[] types, Action? callback, IProgress<float>? progress) => this.PopulateAsync(types, callback, progress);

        IEnumerator IDataManager.SaveAsync(Type[] types, Action? callback, IProgress<float>? progress) => this.SaveAsync(types, callback, progress);

        IEnumerator IDataManager.FlushAsync(Type[] types, Action? callback, IProgress<float>? progress) => this.FlushAsync(types, callback, progress);

        IEnumerator IDataManager.PopulateAllAsync(Action? callback, IProgress<float>? progress) => this.PopulateAsync(this.cache.Keys.Where(type => typeof(IReadableData).IsAssignableFrom(type)), callback, progress);

        IEnumerator IDataManager.SaveAllAsync(Action? callback, IProgress<float>? progress) => this.SaveAsync(this.cache.Keys.Where(type => typeof(IWritableData).IsAssignableFrom(type)), callback, progress);

        IEnumerator IDataManager.FlushAllAsync(Action? callback, IProgress<float>? progress) => this.FlushAsync(this.cache.Keys.Where(type => typeof(IWritableData).IsAssignableFrom(type)), callback, progress);

        private IEnumerator PopulateAsync(IEnumerable<Type> types, Action? callback, IProgress<float>? progress)
        {
            yield return types.GroupBy(type =>
                {
                    if (!this.cache.TryGetValue(type, out var entry)) throw new InvalidOperationException($"{type.Name} not found");
                    if (entry.Storage is not IReadableDataStorage) throw new InvalidOperationException($"{type.Name} not readable");
                    return (entry.Serializer, entry.Storage);
                })
                .Select(LoadAsync)
                .Gather();
            progress?.Report(1);
            callback?.Invoke();

            IEnumerator LoadAsync(IGrouping<(ISerializer Serializer, IDataStorage Storage), Type> group)
            {
                var keys = group.Select(type => type.GetKey()).ToArray();
                switch (group.Key)
                {
                    case (IBinarySerializer, IReadableBinaryStorage):
                    {
                        var (serializer, storage) = ((IBinarySerializer, IReadableBinaryStorage))group.Key;
                        var rawDatas = default(byte[][])!;
                        yield return storage.ReadAsync(keys, result => rawDatas = result);
                        yield return IterTools.Zip(group, rawDatas)
                            .Where((_,     rawData) => rawData.Length > 0)
                            .Select((type, rawData) => serializer.PopulateAsync(this.cache[type].Data, rawData))
                            .Gather();
                        break;
                    }
                    case (IStringSerializer, IReadableStringStorage):
                    {
                        var (serializer, storage) = ((IStringSerializer, IReadableStringStorage))group.Key;
                        var rawDatas = default(string[])!;
                        yield return storage.ReadAsync(keys, result => rawDatas = result);
                        yield return IterTools.Zip(group, rawDatas)
                            .Where((_,     rawData) => rawData.Length > 0)
                            .Select((type, rawData) => serializer.PopulateAsync(this.cache[type].Data, rawData))
                            .Gather();
                        break;
                    }
                    case (IObjectSerializer, IReadableObjectStorage):
                    {
                        var (serializer, storage) = ((IObjectSerializer, IReadableObjectStorage))group.Key;
                        var rawDatas = default(object[])!;
                        yield return storage.ReadAsync(keys, result => rawDatas = result);
                        yield return IterTools.Zip(group, rawDatas)
                            .Select((type, rawData) => serializer.PopulateAsync(this.cache[type].Data, rawData))
                            .Gather();
                        break;
                    }
                    default: throw new InvalidOperationException();
                }
                this.logger.Debug($"Populated {keys.Join(", ")}");
            }
        }

        private IEnumerator SaveAsync(IEnumerable<Type> types, Action? callback, IProgress<float>? progress)
        {
            yield return types.GroupBy(type =>
                {
                    if (!this.cache.TryGetValue(type, out var entry)) throw new InvalidOperationException($"{type.Name} not found");
                    if (entry.Storage is not IWritableDataStorage) throw new InvalidOperationException($"{type.Name} not writable");
                    return (entry.Serializer, entry.Storage);
                })
                .Select(SaveAsync)
                .Gather();
            progress?.Report(1);
            callback?.Invoke();

            IEnumerator SaveAsync(IGrouping<(ISerializer Serializer, IDataStorage Storage), Type> group)
            {
                var keys = group.Select(type => type.GetKey()).ToArray();
                switch (group.Key)
                {
                    case (IBinarySerializer, IWritableBinaryStorage):
                    {
                        var (serializer, storage) = ((IBinarySerializer, IWritableBinaryStorage))group.Key;
                        var rawDatas = new Dictionary<Type, byte[]>();
                        yield return group.Select(type => serializer.SerializeAsync(this.cache[type].Data, result => rawDatas.Add(type, result))).Gather();
                        yield return storage.WriteAsync(keys, group.Select(type => rawDatas[type]).ToArray());
                        break;
                    }
                    case (IStringSerializer, IWritableStringStorage):
                    {
                        var (serializer, storage) = ((IStringSerializer, IWritableStringStorage))group.Key;
                        var rawDatas = new Dictionary<Type, string>();
                        yield return group.Select(type => serializer.SerializeAsync(this.cache[type].Data, result => rawDatas.Add(type, result))).Gather();
                        yield return storage.WriteAsync(keys, group.Select(type => rawDatas[type]).ToArray());
                        break;
                    }
                    case (IObjectSerializer, IWritableObjectStorage):
                    {
                        var (serializer, storage) = ((IObjectSerializer, IWritableObjectStorage))group.Key;
                        var rawDatas = new Dictionary<Type, object>();
                        yield return group.Select(type => serializer.SerializeAsync(this.cache[type].Data, result => rawDatas.Add(type, result))).Gather();
                        yield return storage.WriteAsync(keys, group.Select(type => rawDatas[type]).ToArray());
                        break;
                    }
                    default: throw new InvalidOperationException();
                }
                this.logger.Debug($"Saved {keys.Join(", ")}");
            }
        }

        private IEnumerator FlushAsync(IEnumerable<Type> types, Action? callback, IProgress<float>? progress)
        {
            yield return types.GroupBy(type =>
                {
                    if (!this.cache.TryGetValue(type, out var entry)) throw new InvalidOperationException($"{type.Name} not found");
                    return entry.Storage as IWritableDataStorage ?? throw new InvalidOperationException($"{type.Name} not writable");
                })
                .Select(FlushAsync)
                .Gather();
            progress?.Report(1);
            callback?.Invoke();

            IEnumerator FlushAsync(IGrouping<IWritableDataStorage, Type> group)
            {
                var keys    = group.Select(type => type.GetKey()).ToArray();
                var storage = group.Key;
                yield return storage.FlushAsync();
                this.logger.Debug($"Flushed {keys.Join(", ")}");
            }
        }
        #endif

        #endregion
    }
}