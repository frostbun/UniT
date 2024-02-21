namespace UniT.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UniT.Data.Serializers;
    using UniT.Data.Storages;
    using UniT.Data.Types;
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

        void IDataManager.PopulateAll() => this.Populate(this.datas.Keys);

        void IDataManager.SaveAll() => this.Save(this.datas.Keys);

        void IDataManager.FlushAll() => this.Flush(this.datas.Keys);

        private void Populate(IEnumerable<Type> types)
        {
            types.GroupBy(type => this.storages.GetOrDefault(type) ?? throw new InvalidOperationException($"{type.Name} not found"))
                .ForEach(group =>
                {
                    var keys = group.Select(type => type.GetKey()).ToArray();
                    switch (group.Key)
                    {
                        case IReadOnlySerializableDataStorage storage:
                        {
                            var rawDatas = storage.Load(keys);
                            IterTools.StrictZip(group, rawDatas).ForEach((type, rawData) => this.serializers[type].Populate(this.datas[type], rawData));
                            break;
                        }
                        case IReadOnlyNonSerializableDataStorage storage:
                        {
                            var datas = storage.Load(keys);
                            IterTools.StrictZip(group, datas).ForEach((type, data) => data.CopyTo(this.datas[type]));
                            break;
                        }
                    }
                    this.logger.Debug($"Populated {keys.ToArrayString()}");
                });
        }

        private void Save(IEnumerable<Type> types)
        {
            types.GroupBy(type => this.storages.GetOrDefault(type) ?? throw new InvalidOperationException($"{type.Name} not found"))
                .ForEach(group =>
                {
                    var keys = group.Select(type => type.GetKey()).ToArray();
                    switch (group.Key)
                    {
                        case IReadWriteSerializableDataStorage storage:
                        {
                            var rawDatas = group.Select(type => this.serializers[type].Serialize(this.datas[type])).ToArray();
                            storage.Save(keys, rawDatas);
                            break;
                        }
                        case IReadWriteNonSerializableDataStorage storage:
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
            types.GroupBy(type => this.storages.GetOrDefault(type) ?? throw new InvalidOperationException($"{type.Name} not found"))
                .Where(group => group.Key is IFlushableDataStorage)
                .ForEach(group =>
                {
                    var keys    = group.Select(type => type.GetKey()).ToArray();
                    var storage = (IFlushableDataStorage)group.Key;
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

        UniTask IDataManager.PopulateAllAsync(IProgress<float> progress, CancellationToken cancellationToken) => this.PopulateAsync(this.datas.Keys, progress, cancellationToken);

        UniTask IDataManager.SaveAllAsync(IProgress<float> progress, CancellationToken cancellationToken) => this.SaveAsync(this.datas.Keys, progress, cancellationToken);

        UniTask IDataManager.FlushAllAsync(IProgress<float> progress, CancellationToken cancellationToken) => this.FlushAsync(this.datas.Keys, progress, cancellationToken);

        private UniTask PopulateAsync(IEnumerable<Type> types, IProgress<float> progress, CancellationToken cancellationToken)
        {
            return types.GroupBy(type => this.storages.GetOrDefault(type) ?? throw new InvalidOperationException($"{type.Name} not found"))
                .ForEachAsync(
                    async (group, progress, cancellationToken) =>
                    {
                        var keys = group.Select(type => type.GetKey()).ToArray();
                        switch (group.Key)
                        {
                            case IReadOnlySerializableDataStorage storage:
                            {
                                var rawDatas = await storage.LoadAsync(keys, progress, cancellationToken);
                                this.logger.Debug($"Loaded {keys.ToArrayString()}");
                                IterTools.StrictZip(group, rawDatas).ForEach((type, rawData) => this.serializers[type].Populate(this.datas[type], rawData));
                                this.logger.Debug($"Populated {keys.ToArrayString()}");
                                break;
                            }
                            case IReadOnlyNonSerializableDataStorage storage:
                            {
                                var datas = await storage.LoadAsync(keys, progress, cancellationToken);
                                this.logger.Debug($"Loaded {keys.ToArrayString()}");
                                IterTools.StrictZip(group, datas).ForEach((type, data) => data.CopyTo(this.datas[type]));
                                this.logger.Debug($"Populated {keys.ToArrayString()}");
                                break;
                            }
                        }
                    },
                    progress,
                    cancellationToken
                );
        }

        private UniTask SaveAsync(IEnumerable<Type> types, IProgress<float> progress, CancellationToken cancellationToken)
        {
            return types.GroupBy(type => this.storages.GetOrDefault(type) ?? throw new InvalidOperationException($"{type.Name} not found"))
                .Where(group => group.Key is IReadWriteSerializableDataStorage)
                .ForEachAsync(
                    async (group, progress, cancellationToken) =>
                    {
                        var keys = group.Select(type => type.GetKey()).ToArray();
                        switch (group.Key)
                        {
                            case IReadWriteSerializableDataStorage storage:
                            {
                                var rawDatas = group.Select(type => this.serializers[type].Serialize(this.datas[type])).ToArray();
                                await storage.SaveAsync(keys, rawDatas, progress, cancellationToken);
                                break;
                            }
                            case IReadWriteNonSerializableDataStorage storage:
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
            return types.GroupBy(type => this.storages.GetOrDefault(type) ?? throw new InvalidOperationException($"{type.Name} not found"))
                .Where(group => group.Key is IFlushableDataStorage)
                .ForEachAsync(
                    async (group, progress, cancellationToken) =>
                    {
                        var keys    = group.Select(type => type.GetKey()).ToArray();
                        var storage = (IFlushableDataStorage)group.Key;
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

        IEnumerator IDataManager.PopulateAllAsync(Action callback, IProgress<float> progress) => this.PopulateAsync(this.datas.Keys, callback, progress);

        IEnumerator IDataManager.SaveAllAsync(Action callback, IProgress<float> progress) => this.SaveAsync(this.datas.Keys, callback, progress);

        IEnumerator IDataManager.FlushAllAsync(Action callback, IProgress<float> progress) => this.FlushAsync(this.datas.Keys, callback, progress);

        private IEnumerator PopulateAsync(IEnumerable<Type> types, Action callback, IProgress<float> progress)
        {
            // TODO: make it run concurrently
            var groups = types.GroupBy(type => this.storages.GetOrDefault(type) ?? throw new InvalidOperationException($"{type.Name} not found"));
            foreach (var group in groups)
            {
                var keys = group.Select(type => type.GetKey()).ToArray();
                switch (group.Key)
                {
                    case IReadOnlySerializableDataStorage storage:
                    {
                        yield return storage.LoadAsync(keys, rawDatas =>
                        {
                            this.logger.Debug($"Loaded {keys.ToArrayString()}");
                            IterTools.StrictZip(group, rawDatas).ForEach((type, rawData) => this.serializers[type].Populate(this.datas[type], rawData));
                            this.logger.Debug($"Populated {keys.ToArrayString()}");
                        });
                        break;
                    }
                    case IReadOnlyNonSerializableDataStorage storage:
                    {
                        yield return storage.LoadAsync(keys, datas =>
                        {
                            this.logger.Debug($"Loaded {keys.ToArrayString()}");
                            IterTools.StrictZip(group, datas).ForEach((type, data) => data.CopyTo(this.datas[type]));
                            this.logger.Debug($"Populated {keys.ToArrayString()}");
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
            var groups = types.GroupBy(type => this.storages.GetOrDefault(type) ?? throw new InvalidOperationException($"{type.Name} not found"))
                .Where(group => group.Key is IReadWriteSerializableDataStorage);
            foreach (var group in groups)
            {
                var keys = group.Select(type => type.GetKey()).ToArray();
                switch (group.Key)
                {
                    case IReadWriteSerializableDataStorage storage:
                    {
                        var rawDatas = group.Select(type => this.serializers[type].Serialize(this.datas[type])).ToArray();
                        yield return storage.SaveAsync(keys, rawDatas);
                        break;
                    }
                    case IReadWriteNonSerializableDataStorage storage:
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
            var groups = types.GroupBy(type => this.storages.GetOrDefault(type) ?? throw new InvalidOperationException($"{type.Name} not found"))
                .Where(group => group.Key is IFlushableDataStorage);
            foreach (var group in groups)
            {
                var keys = group.Select(type => type.GetKey()).ToArray();
                var storage = (IFlushableDataStorage)group.Key;
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