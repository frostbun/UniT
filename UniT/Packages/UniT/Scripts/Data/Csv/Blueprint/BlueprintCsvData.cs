namespace UniT.Data.Csv.Blueprint
{
    using UniT.Data.Csv.Base;

    public class BlueprintCsvData : CsvData, IBlueprintCsvData
    {
    }

    public class BlueprintCsvData<T> : CsvData<T>, IBlueprintCsvData
    {
    }

    public class BlueprintCsvData<TKey, TValue> : CsvData<TKey, TValue>, IBlueprintCsvData
    {
    }
}