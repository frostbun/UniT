namespace UniT.Data.Csv
{
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