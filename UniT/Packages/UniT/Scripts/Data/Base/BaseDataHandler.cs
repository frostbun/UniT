namespace UniT.Data.Base
{
    using System;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using UniT.Extensions;
    using UniT.Logging;
    using UniT.Utilities;

    public abstract class BaseDataHandler : IDataHandler
    {
        public ILogger Logger { get; }

        protected BaseDataHandler(ILogger logger = null)
        {
            this.Logger = logger ?? ILogger.Factory.CreateDefault(this.GetType().Name);
        }

        bool IDataHandler.CanHandle(Type type) => this.CanHandle(type);

        UniTask IDataHandler.Populate(IData[] datas)
        {
            var keys = datas.Select(data => data.GetType().GetKeyAttribute()).ToArray();
            return this.LoadRawData(keys)
                       .ContinueWith(rawDatas => IterTools.Zip(rawDatas, datas).Where((rawData, data) => !rawData.IsNullOrWhitespace()).ForEach(this.PopulateData))
                       .ContinueWith(() => this.Logger.Debug($"Loaded {keys.ToJson()}"));
        }

        UniTask IDataHandler.Save(IData[] datas)
        {
            var keys     = datas.Select(data => data.GetType().GetKeyAttribute()).ToArray();
            var rawDatas = datas.Select(this.SerializeData).ToArray();
            return this.SaveRawData(keys, rawDatas)
                       .ContinueWith(() => this.Logger.Debug($"Saved {keys.ToJson()}"));
        }

        UniTask IDataHandler.Flush() => this.Flush().ContinueWith(() => this.Logger.Debug("Flushed"));

        protected virtual bool CanHandle(Type type) => typeof(IData).IsAssignableFrom(type);

        protected abstract UniTask<string[]> LoadRawData(string[] keys);

        protected abstract UniTask SaveRawData(string[] keys, string[] rawDatas);

        protected abstract UniTask Flush();

        protected abstract void PopulateData(string rawData, IData data);

        protected abstract string SerializeData(IData data);
    }
}