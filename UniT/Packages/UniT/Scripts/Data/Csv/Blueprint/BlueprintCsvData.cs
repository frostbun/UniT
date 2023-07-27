namespace UniT.Data.Csv.Blueprint
{
    using UniT.Data.Csv.Base;

    public abstract class BlueprintCsvData : CsvData, IBlueprintCsvData
    {
    }

    public abstract class BlueprintCsvData<T> : CsvData<T>, IBlueprintCsvData
    {
    }

    public abstract class BlueprintCsvData<TKey, TValue> : CsvData<TKey, TValue>, IBlueprintCsvData
    {
    }
}