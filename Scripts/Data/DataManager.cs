namespace UniT.Data
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using UniT.Data.Serializers;
    using UniT.Data.Storages;
    using UniT.Extensions;
    using UniT.Logging;
    using UnityEngine.Scripting;

    public sealed class DataManager : IDataManager
    {
        #region Constructor

        private readonly ReadOnlyDictionary<Type, IReadOnlyData>    datas;
        private readonly ReadOnlyDictionary<Type, IReadOnlyStorage> storages;
        private readonly ReadOnlyDictionary<Type, ISerializer>      serializers;
        private readonly ILogger                                    logger;

        [Preserve]
        public DataManager(
            IReadOnlyData[]    datas,
            IReadOnlyStorage[] storages,
            ISerializer[]      serializers,
            ILogger            logger = null
        )
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

            this.logger = logger ?? ILogger.Default(this.GetType().Name);
            this.datas.Keys.ForEach(type => this.logger.Debug($"Found {type.Name} - {this.storages[type].GetType().Name} - {this.serializers[type].GetType().Name}"));
            this.logger.Debug("Constructed");
        }

        #endregion

        #region Interface Implementations

        LogConfig IDataManager.LogConfig => this.logger.Config;

        IReadOnlyData IDataManager.Get(Type type) => this.datas.GetOrDefault(type);

        UniTask IDataManager.Populate(params Type[] types) => this.Populate(types);

        UniTask IDataManager.Save(params Type[] types) => this.Save(types);

        UniTask IDataManager.Flush(params Type[] types) => this.Flush(types);

        UniTask IDataManager.PopulateAll() => this.Populate(this.datas.Keys);

        UniTask IDataManager.SaveAll() => this.Save(this.datas.Keys);

        UniTask IDataManager.FlushAll() => this.Flush(this.datas.Keys);

        #endregion

        #region Private

        private UniTask Populate(IEnumerable<Type> types) => UniTask.WhenAll(
            types.GroupBy(type => this.storages.GetOrDefault(type) ?? throw new InvalidOperationException($"{type.Name} not found"))
                .Select(group =>
                {
                    var keys    = group.Select(type => type.GetKey()).ToArray();
                    var storage = group.Key;
                    return storage.Load(keys).ContinueWith(rawDatas =>
                    {
                        this.logger.Debug($"Loaded {keys.ToJson()}");
                        IterTools.StrictZip(group, rawDatas).ForEach((type, rawData) => this.serializers[type].Populate(this.datas[type], rawData));
                        this.logger.Debug($"Populated {keys.ToJson()}");
                    });
                })
        );

        private UniTask Save(IEnumerable<Type> types) => UniTask.WhenAll(
            types.GroupBy(type => this.storages.GetOrDefault(type) ?? throw new InvalidOperationException($"{type.Name} not found"))
                .Where(group => group.Key is IStorage)
                .Select(group =>
                {
                    var keys     = group.Select(type => type.GetKey()).ToArray();
                    var rawDatas = group.Select(type => this.serializers[type].Serialize(this.datas[type])).ToArray();
                    this.logger.Debug($"Serialized {keys.ToJson()}");
                    var storage = (IStorage)group.Key;
                    return storage.Save(keys, rawDatas)
                        .ContinueWith(() => this.logger.Debug($"Saved {keys.ToJson()}"));
                })
        );

        private UniTask Flush(IEnumerable<Type> types) => UniTask.WhenAll(
            types.GroupBy(type => this.storages.GetOrDefault(type) ?? throw new InvalidOperationException($"{type.Name} not found"))
                .Where(group => group.Key is IStorage)
                .Select(group =>
                {
                    var keys    = group.Select(type => type.GetKey()).ToArray();
                    var storage = (IStorage)group.Key;
                    return storage.Flush()
                        .ContinueWith(() => this.logger.Debug($"Flushed {keys.ToJson()}"));
                })
        );

        #endregion
    }
}