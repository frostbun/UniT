namespace UniT.Data.Base
{
    using System;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using UniT.Extensions;

    public abstract class BaseDataHandler : IDataHandler
    {
        bool IDataHandler.CanHandle(Type type) => this.CanHandle(type);

        UniTask IDataHandler.Populate(IData[] datas) =>
            this.GetRawData(datas.Select(data => data.GetType().GetKeyAttribute()).ToArray())
                .ContinueWith(rawDatas => IterTools.Zip(rawDatas, datas).Where((rawData, data) => !rawData.IsNullOrWhitespace()).ForEach(this.PopulateData));

        UniTask IDataHandler.Save(IData[] datas) =>
            this.SaveRawData(
                datas.Select(data => data.GetType().GetKeyAttribute()).ToArray(),
                datas.Select(this.SerializeData).ToArray()
            );

        UniTask IDataHandler.Flush() => this.Flush();

        protected virtual bool CanHandle(Type type) => typeof(IData).IsAssignableFrom(type);

        protected abstract UniTask Flush();

        protected abstract UniTask<string[]> GetRawData(string[] keys);

        protected abstract UniTask SaveRawData(string[] keys, string[] rawDatas);

        protected abstract void PopulateData(string rawData, IData data);

        protected abstract string SerializeData(IData data);
    }
}