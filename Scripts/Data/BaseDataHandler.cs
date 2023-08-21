namespace UniT.Data
{
    using System;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using UniT.Extensions;
    using UniT.Logging;

    public abstract class BaseDataHandler : IDataHandler
    {
        public LogConfig LogConfig => this._logger.Config;

        protected readonly ILogger _logger;

        protected BaseDataHandler(ILogger logger = null)
        {
            this._logger = logger ?? ILogger.Default(this.GetType().Name);
            this._logger.Debug("Constructed");
        }

        bool IDataHandler.CanHandle(Type type) => this.CanHandle(type);

        UniTask IDataHandler.Populate(IData[] datas)
        {
            var keys = datas.Select(data => data.GetType().GetKey()).ToArray();
            return this.LoadRawData(keys)
                .ContinueWith(
                    rawDatas => IterTools.Zip(rawDatas, datas)
                        .Where((rawData, _) => !rawData.IsNullOrWhitespace())
                        .ForEach(this.PopulateData)
                )
                .ContinueWith(() => this._logger.Debug($"Loaded {keys.ToJson()}"));
        }

        UniTask IDataHandler.Save(IData[] datas)
        {
            var keys     = datas.Select(data => data.GetType().GetKey()).ToArray();
            var rawDatas = datas.Select(this.SerializeData).ToArray();
            return this.SaveRawData(keys, rawDatas)
                .ContinueWith(() => this._logger.Debug($"Saved {keys.ToJson()}"));
        }

        UniTask IDataHandler.Flush() => this.Flush().ContinueWith(() => this._logger.Debug("Flushed"));

        protected virtual bool CanHandle(Type type) => typeof(IData).IsAssignableFrom(type);

        protected abstract UniTask<string[]> LoadRawData(string[] keys);

        protected abstract UniTask SaveRawData(string[] keys, string[] rawDatas);

        protected abstract UniTask Flush();

        protected abstract void PopulateData(string rawData, IData data);

        protected abstract string SerializeData(IData data);
    }
}