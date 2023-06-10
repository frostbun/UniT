namespace BlueprintData
{
    using UniT.Data.Csv.Base;
    using UniT.Data.Csv.Blueprint;

    public class LevelBlueprint : BlueprintCsvData<LevelRecord>
    {
    }

    [CsvKey(nameof(Id))]
    public class LevelRecord
    {
        public int    Id;
        public string Name;
    }
}