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

        public DataManager(IData[] dataCache, IDataHandler[] handlerCache, ILogger logger)
        {
            this.dataCache    = dataCache.ToDictionary(data => data.GetType(), data => data).AsReadOnly();
            this.handlerCache = handlerCache.ToDictionary(handler => handler.GetType(), handler => handler).AsReadOnly();
            this.dataTypeToHandlerType = dataCache.ToDictionary(
                data => data.GetType(),
                data => handlerCache.LastOrDefault(handler => handler.CanHandle(data.GetType()))?.GetType()
                        ?? throw new($"No handler found for type {data.GetType().Name}")
            ).AsReadOnly();

            this.Logger = logger;
            this.dataTypeToHandlerType.ForEach((dataType, handlerType) => this.Logger.Info($"Found {dataType.Name} - {handlerType.Name}"));
            this.Logger.Info($"{this.GetType().Name} instantiated with {this.dataCache.Count} data and {this.handlerCache.Count} handlers");
        }

        public UniTask PopulateData(params Type[] dataTypes)
        {
            return UniTask.WhenAll(dataTypes.Select(this.PopulateData_Internal));
        }

        public UniTask SaveData(params Type[] dataTypes)
        {
            return UniTask.WhenAll(dataTypes.Select(this.SaveData_Internal));
        }

        public UniTask FlushData(params Type[] dataTypes)
        {
            return UniTask.WhenAll(
                dataTypes.Select(dataType => this.dataTypeToHandlerType[dataType])
                         .Distinct()
                         .Select(this.FlushHandler_Internal)
            );
        }

        public UniTask PopulateAllData()
        {
            return this.PopulateData(this.dataCache.Keys.ToArray());
        }

        public UniTask SaveAllData()
        {
            return this.SaveData(this.dataCache.Keys.ToArray());
        }

        public UniTask FlushAllData()
        {
            return this.FlushData(this.dataCache.Keys.ToArray());
        }

        private UniTask PopulateData_Internal(Type dataType)
        {
            return this.handlerCache[this.dataTypeToHandlerType[dataType]]
                       .Populate(this.dataCache[dataType])
                       .ContinueWith(() => this.Logger.Debug($"Loaded {dataType.Name}"));
        }

        private UniTask SaveData_Internal(Type dataType)
        {
            return this.handlerCache[this.dataTypeToHandlerType[dataType]]
                       .Save(this.dataCache[dataType])
                       .ContinueWith(() => this.Logger.Debug($"Saved {dataType.Name}"));
        }

        private UniTask FlushHandler_Internal(Type handlerType)
        {
            return this.handlerCache[handlerType]
                       .Flush()
                       .ContinueWith(() => this.Logger.Debug($"Flushed {handlerType.Name}"));
        }
    }
}