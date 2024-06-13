#nullable enable
namespace UniT.Data.Serialization
{
    #if UNIT_UNITASK
    using Cysharp.Threading.Tasks;
    #else
    using System;
    using System.Collections;
    #endif

    public interface IStringSerializer : ISerializer
    {
        public void Populate(IData data, string rawData);

        public string Serialize(IData data);

        #if UNIT_UNITASK
        public UniTask PopulateAsync(IData data, string rawData);

        public UniTask<string> SerializeAsync(IData data);
        #else
        public IEnumerator PopulateAsync(IData data, string rawData, Action? callback = null);

        public IEnumerator SerializeAsync(IData data, Action<string> callback);
        #endif
    }
}