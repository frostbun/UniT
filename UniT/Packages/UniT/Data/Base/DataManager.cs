namespace UniT.Data.Base
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using UniT.Extensions;
    using UniT.Logging;

    public class DataManager
    {
        private readonly Logger                                  logger;
        private readonly ReadOnlyDictionary<Type, IData>         dataCache;
        private readonly ReadOnlyDictionary<Type, IDataHandler>  handlerCache;
        private readonly ReadOnlyDictionary<IData, IDataHandler> dataToHandler;

        public DataManager(List<IData> dataCache, List<IDataHandler> handlerCache, Logger logger = null)
        {
            this.logger        = logger;
            this.dataCache     = dataCache.ToDictionary(data => data.GetType(), data => data).AsReadOnly();
            this.handlerCache  = handlerCache.ToDictionary(handler => handler.GetType(), handler => handler).AsReadOnly();
            this.dataToHandler = this.dataCache.Values.ToDictionary(data => data, data => this.handlerCache.Values.Last(handler => handler.CanHandle(data.GetType()))).AsReadOnly();
            this.logger?.Log($"DataManager initialized with {this.dataCache.Count} data and {this.handlerCache.Count} handlers");
        }

        public UniTask PopulateData(Type type)
        {
            var data    = this.dataCache[type];
            var handler = this.dataToHandler[data];
            return handler.Populate(data)
                          .ContinueWith(() => this.logger?.Log($"Loaded {type.Name}"));
        }

        public UniTask SaveData(Type type)
        {
            var data    = this.dataCache[type];
            var handler = this.dataToHandler[data];
            return handler.Save(data)
                          .ContinueWith(() => this.logger?.Log($"Saved {type.Name}"));
        }

        public UniTask FlushHandler(Type type)
        {
            return this.handlerCache[type].Flush()
                       .ContinueWith(() => this.logger?.Log($"Flushed {type.Name}"));
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