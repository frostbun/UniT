namespace UniT.Data.Csv.Base
{
    using System;
    using System.IO;
    using Sylvan.Data.Csv;
    using UniT.Data.Base;

    public abstract class BaseCsvDataHandler : BaseDataHandler
    {
        public override bool CanHandle(Type type)
        {
            return typeof(ICsvData).IsAssignableFrom(type);
        }

        protected override void PopulateData_Internal(string rawData, IData data)
        {
            var       parser    = new CsvParser((ICsvData)data);
            using var csvReader = CsvDataReader.Create(new StringReader(rawData), new() { Delimiter = ',' });
            while (csvReader.Read())
            {
                parser.ParseRow(csvReader);
            }
        }

        protected override string SerializeData_Internal(IData data)
        {
            throw new NotImplementedException();
        }
    }
}