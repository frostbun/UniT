namespace UniT.Data.Csv
{
    using System;
    using System.IO;
    using Sylvan.Data.Csv;
    using UniT.Data.Csv.Reader;
    using UniT.Logging;

    public abstract class BaseCsvDataHandler : BaseDataHandler
    {
        protected BaseCsvDataHandler(ILogger logger = null) : base(logger)
        {
        }

        protected override bool CanHandle(Type type)
        {
            return base.CanHandle(type) && typeof(ICsvData).IsAssignableFrom(type);
        }

        protected override void PopulateData(string rawData, IData data)
        {
            using var reader = CsvDataReader.Create(
                new StringReader(rawData),
                new()
                {
                    Delimiter      = ',',
                    HeaderComparer = StringComparer.OrdinalIgnoreCase,
                }
            );
            var parser = new CsvParser((ICsvData)data, reader);
            while (reader.Read()) parser.Parse();
        }

        protected override string SerializeData(IData data)
        {
            // TODO: Implement this
            return "";
        }
    }
}