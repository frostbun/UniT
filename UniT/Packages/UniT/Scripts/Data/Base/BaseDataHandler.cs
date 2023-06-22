namespace UniT.Data.Base
{
    using System;
    using Cysharp.Threading.Tasks;
    using UniT.Extensions;

    public abstract class BaseDataHandler : IDataHandler
    {
        bool IDataHandler.CanHandle(Type type) => this.CanHandle(type);

        UniTask IDataHandler.Populate(IData data) =>
            this.GetRawData(data.GetType().GetKeyAttribute())
                .ContinueWith(rawData =>
                {
                    if (rawData.IsNullOrWhitespace()) return;
                    this.PopulateData(rawData, data);
                });

        UniTask IDataHandler.Save(IData data) =>
            this.SaveRawData(
                data.GetType().GetKeyAttribute(),
                this.SerializeData(data)
            );

        UniTask IDataHandler.Flush() => this.Flush();

        protected virtual bool CanHandle(Type type) => typeof(IData).IsAssignableFrom(type);

        protected abstract UniTask Flush();

        protected abstract UniTask<string> GetRawData(string key);

        protected abstract UniTask SaveRawData(string key, string rawData);

        protected abstract void PopulateData(string rawData, IData data);

        protected abstract string SerializeData(IData data);
    }
}