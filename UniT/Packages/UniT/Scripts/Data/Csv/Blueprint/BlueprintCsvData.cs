namespace UniT.Data.Csv.Blueprint
{
    using UniT.Data.Base;
    using UniT.Data.Csv.Base;

    public class BlueprintCsvData<T> : BaseCsvData<T>, IBlueprintData
    {
    }

    public class BlueprintCsvData<TKey, TValue> : BaseCsvData<TKey, TValue>, IBlueprintData
    {
    }
}