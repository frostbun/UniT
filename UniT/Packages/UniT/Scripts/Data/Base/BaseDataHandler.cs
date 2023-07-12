namespace UniT.Data.Base
{
    using System;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using UniT.Extensions;
    using UniT.Extensions.UniTask;
    using UniT.Logging;

    public abstract class BaseDataHandler : IDataHandler
    {
        public ILogger Logger { get; }

        protected BaseDataHandler(ILogger logger = null)
        {
            this.Logger = logger ?? ILogger.Factory.Default(this.GetType().Name);
        }

        bool IDataHandler.CanHandle(Type type) => this.CanHandle(type);

        UniTask IDataHandler.Populate(IData[] datas)
        {
            var keys = datas.Select(data => data.GetType().GetKey()).ToArray();
            return this.LoadRawData(keys)
                       .ContinueWith(rawDatas => IterTools.Zip(rawDatas, datas).Where((rawData, _) => !rawData.IsNullOrWhitespace()).ForEach(this.PopulateData))
                       .ContinueWith(() => this.Logger.Debug($"Loaded {keys.ToJson()}"))
                       .Catch(e => throw this.Logger.Exception(e));
        }

        UniTask IDataHandler.Save(IData[] datas)
        {
            var keys     = datas.Select(data => data.GetType().GetKey()).ToArray();
            var rawDatas = datas.Select(this.SerializeData).ToArray();
            return this.SaveRawData(keys, rawDatas)
                       .ContinueWith(() => this.Logger.Debug($"Saved {keys.ToJson()}"))
                       .Catch(e => throw this.Logger.Exception(e));
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