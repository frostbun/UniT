namespace UniT.Data.Json.Base
{
    using Newtonsoft.Json;
    using UniT.Data.Base;

    public abstract class BaseJsonDataHandler : BaseDataHandler
    {
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