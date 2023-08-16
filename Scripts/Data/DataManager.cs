namespace UniT.Data
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using UniT.Extensions;
    using UniT.Logging;
    using UnityEngine.Scripting;

    public class DataManager : IDataManager
    {
        private readonly ReadOnlyDictionary<Type, IData>        _dataCache;
        private readonly ReadOnlyDictionary<Type, IDataHandler> _handlerCache;
        private readonly ReadOnlyDictionary<Type, Type>         _dataTypeToHandlerType;
        private readonly ILogger                                _logger;

        [Preserve]
        public DataManager(IData[] dataCache, IDataHandler[] handlerCache, ILogger logger = null)
        {
            this._dataCache    = dataCache.ToDictionary(data => data.GetType(), data => data).AsReadOnly();
            this._handlerCache = handlerCache.ToDictionary(handler => handler.GetType(), handler => handler).AsReadOnly();
            this._dataTypeToHandlerType = dataCache.ToDictionary(
                data => data.GetType(),
                data => handlerCache.LastOrDefault(handler => handler.CanHandle(data.GetType()))?.GetType()
                    ?? throw new ArgumentException($"No handler found for type {data.GetType().Name}")
            ).AsReadOnly();

            this._logger = logger ?? ILogger.Default(this.GetType().Name);
            this._dataTypeToHandlerType.ForEach((dataType, handlerType) => this._logger.Debug($"Found {dataType.Name} - {handlerType.Name}"));
            this._logger.Info("Constructed");
        }

        public LogConfig LogConfig => this._logger.Config;

        public T Get<T>() where T : IData
        {
            return (T)this._dataCache.GetOrDefault(typeof(T));
        }

        public UniTask Populate<T>() where T : IData
        {
            return this.Populate(typeof(T));
        }

        public UniTask Save<T>() where T : IData
        {
            return this.Save(typeof(T));
        }

        public UniTask Flush<T>() where T : IData
        {
            return this.Flush(typeof(T));
        }

        public UniTask Populate(params Type[] dataTypes)
        {
            return UniTask.WhenAll(dataTypes.GroupBy(dataType => this._dataTypeToHandlerType[dataType]).Select(group => this._handlerCache[group.Key].Populate(group.Select(dataType => this._dataCache[dataType]).ToArray())));
        }

        public UniTask Save(params Type[] dataTypes)
        {
            return UniTask.WhenAll(dataTypes.GroupBy(dataType => this._dataTypeToHandlerType[dataType]).Select(group => this._handlerCache[group.Key].Save(group.Select(dataType => this._dataCache[dataType]).ToArray())));
        }

        public UniTask Flush(params Type[] dataTypes)
        {
            return UniTask.WhenAll(dataTypes.Select(dataType => this._dataTypeToHandlerType[dataType]).Distinct().Select(handlerType => this._handlerCache[handlerType].Flush()));
        }

        public UniTask PopulateAll()
        {
            return this.Populate(this._dataCache.Keys!.ToArray());
        }

        public UniTask SaveAll()
        {
            return this.Save(this._dataCache.Keys!.ToArray());
        }

        public UniTask FlushAll()
        {
            return this.Flush(this._dataCache.Keys!.ToArray());
        }
    }
}