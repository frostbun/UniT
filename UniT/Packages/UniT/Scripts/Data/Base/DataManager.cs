namespace UniT.Data.Base
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using UniT.Extensions;
    using UniT.Logging;
    using UnityEngine;
    using ILogger = UniT.Logging.ILogger;

    public class DataManager : IDataManager
    {
        private readonly ILogger                                 logger;
        private readonly ReadOnlyDictionary<Type, IData>         dataCache;
        private readonly ReadOnlyDictionary<Type, IDataHandler>  handlerCache;
        private readonly ReadOnlyDictionary<IData, IDataHandler> dataToHandler;

        public DataManager(IData[] dataCache, IDataHandler[] handlerCache)
        {
            this.dataCache     = dataCache.ToDictionary(data => data.GetType(), data => data).AsReadOnly();
            this.handlerCache  = handlerCache.ToDictionary(handler => handler.GetType(), handler => handler).AsReadOnly();
            this.dataToHandler = dataCache.ToDictionary(data => data, data => handlerCache.Last(handler => handler.CanHandle(data.GetType()))).AsReadOnly();
            this.logger        = LoggerManager.Instance.Get<IDataManager>();
            this.dataToHandler.ForEach((data, handler) => this.logger.Info($"Found {data.GetType().Name} - {handler.GetType().Name}", Color.green));
            this.logger.Info($"{nameof(DataManager)} instantiated with {this.dataCache.Count} data and {this.handlerCache.Count} handlers", Color.green);
        }

        public UniTask PopulateData(Type type)
        {
            var data    = this.dataCache[type];
            var handler = this.dataToHandler[data];
            return handler.Populate(data)
                          .ContinueWith(() => this.logger.Debug($"Loaded {type.Name}"));
        }

        public UniTask SaveData(Type type)
        {
            var data    = this.dataCache[type];
            var handler = this.dataToHandler[data];
            return handler.Save(data)
                          .ContinueWith(() => this.logger.Debug($"Saved {type.Name}"));
        }

        public UniTask FlushHandler(Type type)
        {
            return this.handlerCache[type].Flush()
                       .ContinueWith(() => this.logger.Debug($"Flushed {type.Name}"));
        }

        public UniTask PopulateData<TData>() where TData : IData
        {
            return this.PopulateData(typeof(TData));
        }

        public UniTask SaveData<TData>() where TData : IData
        {
            return this.SaveData(typeof(TData));
        }

        public UniTask FlushHandler<THandler>() where THandler : IDataHandler
        {
            return this.FlushHandler(typeof(THandler));
        }

        public UniTask PopulateAllData()
        {
            return UniTask.WhenAll(this.dataCache.Keys.Select(this.PopulateData));
        }

        public UniTask SaveAllData()
        {
            return UniTask.WhenAll(this.dataCache.Keys.Select(this.SaveData));
        }

        public UniTask FlushAllHandlers()
        {
            return UniTask.WhenAll(this.handlerCache.Keys.Select(this.FlushHandler));
        }
    }
}