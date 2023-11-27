namespace UniT.Data
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using UniT.Data.Serializers;
    using UniT.Data.Storages;
    using UniT.Extensions;
    using UniT.Logging;
    using UnityEngine.Scripting;
    #if UNIT_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #endif

    public sealed class DataManager : IDataManager
    {
        #region Constructor

        private readonly ReadOnlyDictionary<Type, IData>       datas;
        private readonly ReadOnlyDictionary<Type, IStorage>    storages;
        private readonly ReadOnlyDictionary<Type, ISerializer> serializers;
        private readonly ILogger                               logger;

        [Preserve]
        public DataManager(IData[] datas, IStorage[] storages, ISerializer[] serializers, ILogger logger)
        {
            this.datas = datas.ToDictionary(data => data.GetType()).AsReadOnly();
            this.storages = datas.ToDictionary(
                data => data.GetType(),
                data => storages.LastOrDefault(storage => storage.CanStore(data.GetType())) ?? throw new InvalidOperationException($"No storage found for {data.GetType().Name}")
            ).AsReadOnly();
            this.serializers = datas.ToDictionary(
                data => data.GetType(),
                data => serializers.LastOrDefault(serializer => serializer.CanSerialize(data.GetType())) ?? throw new InvalidOperationException($"No serializer found for {data.GetType().Name}")
            ).AsReadOnly();

            this.logger = logger;
            this.datas.Keys.ForEach(type => this.logger.Debug($"Found {type.Name} - {this.storages[type].GetType().Name} - {this.serializers[type].GetType().Name}"));
            this.logger.Debug("Constructed");
        }

        #endregion

        LogConfig IDataManager.LogConfig => this.logger.Config;

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
                    var keys     = group.Select(type => type.GetKey()).ToArray();
                    var storage  = group.Key;
                    var rawDatas = storage.Load(keys);
                    this.logger.Debug($"Loaded {keys.ToArrayString()}");
                    IterTools.StrictZip(group, rawDatas).ForEach((type, rawData) => this.serializers[type].Populate(this.datas[type], rawData));
                    this.logger.Debug($"Populated {keys.ToArrayString()}");
                });
        }

        private void Save(IEnumerable<Type> types)
        {
            types.GroupBy(type => this.storages.GetOrDefault(type) ?? throw new InvalidOperationException($"{type.Name} not found"))
                .Where(group => group.Key is IReadWriteStorage)
                .ForEach(group =>
                {
                    var keys     = group.Select(type => type.GetKey()).ToArray();
                    var rawDatas = group.Select(type => this.serializers[type].Serialize(this.datas[type])).ToArray();
                    this.logger.Debug($"Serialized {keys.ToArrayString()}");
                    var storage = (IReadWriteStorage)group.Key;
                    storage.Save(keys, rawDatas);
                    this.logger.Debug($"Saved {keys.ToArrayString()}");
                });
        }

        private void Flush(IEnumerable<Type> types)
        {
            types.GroupBy(type => this.storages.GetOrDefault(type) ?? throw new InvalidOperationException($"{type.Name} not found"))
                .Where(group => group.Key is IReadWriteStorage)
                .ForEach(group =>
                {
                    var keys    = group.Select(type => type.GetKey()).ToArray();
                    var storage = (IReadWriteStorage)group.Key;
                    storage.Flush();
                    this.logger.Debug($"Flushed {keys.ToArrayString()}");
                });
        }

        #endregion

        #region Async

        #if UNIT_UNITASK
        UniTask IDataManager.PopulateAsync(Type type, IProgress<float> progress, CancellationToken cancellationToken) => this.PopulateAsync(new[] { type }, progress, cancellationToken);

        UniTask IDataManager.SaveAsync(Type type, IProgress<float> progress, CancellationToken cancellationToken) => this.SaveAsync(new[] { type }, progress, cancellationToken);

        UniTask IDataManager.FlushAsync(Type type, IProgress<float> progress, CancellationToken cancellationToken) => this.FlushAsync(new[] { type }, progress, cancellationToken);

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
                    (group, progress, cancellationToken) =>
                    {
                        var keys    = group.Select(type => type.GetKey()).ToArray();
                        var storage = group.Key;
                        return storage.LoadAsync(keys, progress, cancellationToken)
                            .ContinueWith(rawDatas =>
                            {
                                this.logger.Debug($"Loaded {keys.ToArrayString()}");
                                IterTools.StrictZip(group, rawDatas).ForEach((type, rawData) => this.serializers[type].Populate(this.datas[type], rawData));
                                this.logger.Debug($"Populated {keys.ToArrayString()}");
                            });
                    },
                    progress,
                    cancellationToken
                );
        }

        private UniTask SaveAsync(IEnumerable<Type> types, IProgress<float> progress, CancellationToken cancellationToken)
        {
            return types.GroupBy(type => this.storages.GetOrDefault(type) ?? throw new InvalidOperationException($"{type.Name} not found"))
                .Where(group => group.Key is IReadWriteStorage)
                .ForEachAsync(
                    (group, progress, cancellationToken) =>
                    {
                        var keys     = group.Select(type => type.GetKey()).ToArray();
                        var rawDatas = group.Select(type => this.serializers[type].Serialize(this.datas[type])).ToArray();
                        this.logger.Debug($"Serialized {keys.ToArrayString()}");
                        var storage = (IReadWriteStorage)group.Key;
                        return storage.SaveAsync(keys, rawDatas, progress, cancellationToken)
                            .ContinueWith(() => this.logger.Debug($"Saved {keys.ToArrayString()}"));
                    },
                    progress,
                    cancellationToken
                );
        }

        private UniTask FlushAsync(IEnumerable<Type> types, IProgress<float> progress, CancellationToken cancellationToken)
        {
            return types.GroupBy(type => this.storages.GetOrDefault(type) ?? throw new InvalidOperationException($"{type.Name} not found"))
                    .Where(group => group.Key is IReadWriteStorage)
                    .ForEachAsync(
                        (group, progress, cancellationToken) =>
                        {
                            var keys    = group.Select(type => type.GetKey()).ToArray();
                            var storage = (IReadWriteStorage)group.Key;
                            return storage.FlushAsync(progress, cancellationToken)
                                .ContinueWith(() => this.logger.Debug($"Flushed {keys.ToArrayString()}"));
                        },
                        progress,
                        cancellationToken
                    )
                ;
        }
        #endif

        #endregion
    }
}