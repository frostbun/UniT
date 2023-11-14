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

        private readonly ReadOnlyDictionary<Type, IReadOnlyData>    _datas;
        private readonly ReadOnlyDictionary<Type, IReadOnlyStorage> _storages;
        private readonly ReadOnlyDictionary<Type, ISerializer>      _serializers;
        private readonly ILogger                                    _logger;

        [Preserve]
        public DataManager(
            IReadOnlyData[]    datas,
            IReadOnlyStorage[] storages,
            ISerializer[]      serializers,
            ILogger            logger = null
        )
        {
            this._datas = datas.ToDictionary(data => data.GetType()).AsReadOnly();
            this._storages = datas.ToDictionary(
                data => data.GetType(),
                data => storages.LastOrDefault(storage => storage.CanStore(data.GetType())) ?? throw new InvalidOperationException($"No storage found for {data.GetType().Name}")
            ).AsReadOnly();
            this._serializers = datas.ToDictionary(
                data => data.GetType(),
                data => serializers.LastOrDefault(serializer => serializer.CanSerialize(data.GetType())) ?? throw new InvalidOperationException($"No serializer found for {data.GetType().Name}")
            ).AsReadOnly();

            this._logger = logger ?? ILogger.Default(this.GetType().Name);
            this._datas.Keys.ForEach(type => this._logger.Debug($"Found {type.Name} - {this._storages[type].GetType().Name} - {this._serializers[type].GetType().Name}"));
            this._logger.Debug("Constructed");
        }

        #endregion

        #region Interface Implementations

        LogConfig IDataManager.LogConfig => this._logger.Config;

        IReadOnlyData IDataManager.Get(Type type) => this._datas[type];

        UniTask IDataManager.Populate(params Type[] types) => this.Populate(types);

        UniTask IDataManager.Save(params Type[] types) => this.Save(types);

        UniTask IDataManager.Flush(params Type[] types) => this.Flush(types);

        UniTask IDataManager.PopulateAll() => this.Populate(this._datas.Keys);

        UniTask IDataManager.SaveAll() => this.Save(this._datas.Keys);

        UniTask IDataManager.FlushAll() => this.Flush(this._datas.Keys);

        #endregion

        #region Private

        private UniTask Populate(IEnumerable<Type> types) => UniTask.WhenAll(
            types.GroupBy(type => this._storages[type])
                .Select(group =>
                {
                    var keys    = group.Select(type => type.GetKey()).ToArray();
                    var storage = group.Key;
                    return storage.Load(keys).ContinueWith(rawDatas =>
                    {
                        this._logger.Debug($"Loaded {keys.ToJson()}");
                        IterTools.Zip(group, rawDatas).ForEach((type, rawData) => this._serializers[type].Populate(this._datas[type], rawData));
                        this._logger.Debug($"Populated {keys.ToJson()}");
                    });
                })
        );

        private UniTask Save(IEnumerable<Type> types) => UniTask.WhenAll(
            types.GroupBy(type => this._storages[type])
                .Select(group =>
                {
                    var keys     = group.Select(type => type.GetKey()).ToArray();
                    var rawDatas = group.Select(type => this._serializers[type].Serialize(this._datas[type])).ToArray();
                    this._logger.Debug($"Serialized {keys.ToJson()}");
                    var storage = (IStorage)group.Key;
                    return storage.Save(keys, rawDatas)
                        .ContinueWith(() => this._logger.Debug($"Saved {keys.ToJson()}"));
                })
        );

        private UniTask Flush(IEnumerable<Type> types) => UniTask.WhenAll(
            types.Select(type => (IStorage)this._storages[type])
                .Distinct()
                .Select(storage => storage.Flush()
                    .ContinueWith(() => this._logger.Debug($"Flushed {storage.GetType().Name}"))
                )
        );

        #endregion
    }
}