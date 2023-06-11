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
            using var reader = CsvDataReader.Create(new StringReader(rawData), new() { Delimiter = ',' });
            var       parser = new CsvParser((ICsvData)data, reader);
            while (reader.Read()) parser.Parse();
        }

        protected override string SerializeData_Internal(IData data)
        {
            throw new NotImplementedException();
        }
    }
}