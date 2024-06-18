#if UNIT_CSV
#nullable enable
namespace UniT.Data.Serialization
{
    using System;
    using System.Collections;

    public interface ICsvData
    {
        public Type RowType { get; }

        public void Add(object key, object value);

        public IEnumerator GetEnumerator();
    }
}
#endif