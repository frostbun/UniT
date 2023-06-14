namespace UniT.Data.Csv.Blueprint
{
    using UniT.Data.Base;
    using UniT.Data.Csv.Base;

    public class BlueprintCsvData : CsvData, IBlueprintData
    {
    }

    public class BlueprintCsvData<T> : CsvData<T>, IBlueprintData
    {
    }

    public class BlueprintCsvData<TKey, TValue> : CsvData<TKey, TValue>, IBlueprintData
    {
    }
}