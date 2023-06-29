namespace UniT.Data.Json.Base
{
    using System;
    using Newtonsoft.Json;
    using UniT.Data.Base;
    using UniT.Logging;

    public abstract class BaseJsonDataHandler : BaseDataHandler
    {
        protected BaseJsonDataHandler(ILogger logger = null) : base(logger)
        {
        }

        protected override bool CanHandle(Type type)
        {
            return base.CanHandle(type) && typeof(IJsonData).IsAssignableFrom(type);
        }

        protected override void PopulateData(string rawData, IData data)
        {
            JsonConvert.PopulateObject(rawData, data);
        }

        protected override string SerializeData(IData data)
        {
            return JsonConvert.SerializeObject(data);
        }
    }
}