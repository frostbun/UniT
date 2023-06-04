namespace UniT.Data.Base
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using UniT.Extensions;

    public class DataManager
    {
        private readonly ReadOnlyDictionary<Type, IData>         dataCache;
        private readonly ReadOnlyDictionary<Type, IDataHandler>  handlerCache;
        private readonly ReadOnlyDictionary<IData, IDataHandler> dataToHandler;

        public DataManager(List<IData> dataCache, List<IDataHandler> handlerCache)
        {
            this.dataCache     = dataCache.ToDictionary(data => data.GetType(), data => data).AsReadOnly();
            this.handlerCache  = handlerCache.ToDictionary(handler => handler.GetType(), handler => handler).AsReadOnly();
            this.dataToHandler = this.dataCache.Values.ToDictionary(data => data, data => this.handlerCache.Values.Last(handler => handler.CanHandle(data.GetType()))).AsReadOnly();
        }

        public UniTask PopulateData(Type type)
        {
            var data    = this.dataCache[type];
            var handler = this.dataToHandler[data];
            return handler.Populate(data);
        }

        public UniTask SaveData(Type type)
        {
            var data    = this.dataCache[type];
            var handler = this.dataToHandler[data];
            return handler.Save(data);
        }

        public UniTask FlushHandler(Type type)
        {
            return this.handlerCache[type].Flush();
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