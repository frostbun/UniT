namespace UniT.Data.Base
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using UniT.Extensions;
    using UniT.Logging;

    public class DataManager : IDataManager
    {
        public ILogger Logger { get; }

        private readonly ReadOnlyDictionary<Type, IData>        dataCache;
        private readonly ReadOnlyDictionary<Type, IDataHandler> handlerCache;
        private readonly ReadOnlyDictionary<Type, Type>         dataTypeToHandlerType;

        public DataManager(IData[] dataCache, IDataHandler[] handlerCache, ILogger logger = null)
        {
            this.dataCache    = dataCache.ToDictionary(data => data.GetType(), data => data).AsReadOnly();
            this.handlerCache = handlerCache.ToDictionary(handler => handler.GetType(), handler => handler).AsReadOnly();
            this.dataTypeToHandlerType = dataCache.ToDictionary(
                data => data.GetType(),
                data => handlerCache.LastOrDefault(handler => handler.CanHandle(data.GetType()))?.GetType()
                        ?? throw new($"No handler found for type {data.GetType().Name}")
            ).AsReadOnly();

            this.Logger = logger ?? ILogger.Factory.Default(this.GetType().Name);
            this.dataTypeToHandlerType.ForEach((dataType, handlerType) => this.Logger.Debug($"Found {dataType.Name} - {handlerType.Name}"));
        }

        public T Get<T>() where T : IData
        {
            return (T)this.dataCache.GetOrDefault(typeof(T));
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
            return UniTask.WhenAll(dataTypes.GroupBy(dataType => this.dataTypeToHandlerType[dataType]).Select(group => this.handlerCache[group.Key].Populate(group.Select(dataType => this.dataCache[dataType]).ToArray())));
        }

        public UniTask Save(params Type[] dataTypes)
        {
            return UniTask.WhenAll(dataTypes.GroupBy(dataType => this.dataTypeToHandlerType[dataType]).Select(group => this.handlerCache[group.Key].Save(group.Select(dataType => this.dataCache[dataType]).ToArray())));
        }

        public UniTask Flush(params Type[] dataTypes)
        {
            return UniTask.WhenAll(dataTypes.Select(dataType => this.dataTypeToHandlerType[dataType]).Distinct().Select(handlerType => this.handlerCache[handlerType].Flush()));
        }

        public UniTask PopulateAll()
        {
            return this.Populate(this.dataCache.Keys!.ToArray());
        }

        public UniTask SaveAll()
        {
            return this.Save(this.dataCache.Keys!.ToArray());
        }

        public UniTask FlushAll()
        {
            return this.Flush(this.dataCache.Keys!.ToArray());
        }
    }
}