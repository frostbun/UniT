namespace BlueprintData
{
    using UniT.Data.Csv.Blueprint;

    public class LevelBlueprint : BlueprintCsvData<LevelRecord>
    {
    }

    public class LevelRecord
    {
        public int    Id;
        public string Name;
    }
}