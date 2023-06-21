namespace UniT.Data.Json.Base
{
    using System;
    using Newtonsoft.Json;
    using UniT.Data.Base;

    public abstract class BaseJsonDataHandler : BaseDataHandler
    {
        protected override bool CanHandle(Type type)
        {
            return base.CanHandle(type) && typeof(IJsonData).IsAssignableFrom(type);
        }

        protected override void PopulateData_Internal(string rawData, IData data)
        {
            JsonConvert.PopulateObject(rawData, data);
        }

        protected override string SerializeData_Internal(IData data)
        {
            return JsonConvert.SerializeObject(data);
        }
    }
}